/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using GitterbackLib.GitThings;
using GitterbackLib.Utilities;

using Newtonsoft.Json;

namespace GitterbackLib.Configuration;

/// <summary>
/// Represents the folder holding the per-machine, per-user
/// Gitterback settings.
/// </summary>
public class SettingsStore
{
  private GitterbackSettings? _settings;
  private Dictionary<string, Anchor> _anchors;

  /// <summary>
  /// Create a new SettingsStore, creating the folder and
  /// settings file if they do not exist yet.
  /// </summary>
  public SettingsStore(
    string? folder = null)
  {
    _anchors = new Dictionary<string, Anchor>(
      StringComparer.OrdinalIgnoreCase);
    Folder = Path.GetFullPath(folder ?? DefaultFolder);
    if(!Directory.Exists(Folder))
    {
      Directory.CreateDirectory(Folder);
    }
    var settings = GetSettings(); // Create the settings if they do not exist
    foreach(var kvp in settings.Anchors)
    {
      var anchor = new Anchor(this, kvp.Key, kvp.Value);
      _anchors.Add(anchor.AnchorName, anchor);
    }
  }

  /// <summary>
  /// The default location for the settings store.
  /// </summary>
  public static string DefaultFolder =
    Path.Combine(
      Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData),
      "Gitterback");

  /// <summary>
  /// The folder where the settings are stored.
  /// </summary>
  public string Folder { get; }

  /// <summary>
  /// True if the settings have been modified but not yet saved.
  /// </summary>
  public bool IsDirty { get; private set; } = false;

  /// <summary>
  /// Mark the settings as dirty. This will mark them to be saved
  /// the next time <see cref="SaveIfDirty"/> is called.
  /// </summary>
  public void MarkAsDirty()
  {
    IsDirty = true;
  }

  /// <summary>
  /// Get the Gitterback settings. Initialized the first time
  /// this is called, or if <paramref name="reload"/> is true.
  /// Otherwise, the settings are cached.
  /// </summary>
  /// <param name="reload">
  /// Default false. If true, the settings are reloaded even
  /// if they were cached already.
  /// </param>
  /// <returns></returns>
  public GitterbackSettings GetSettings(bool reload = false)
  {
    if(_settings == null || reload)
    {
      var settingsFile = Path.Combine(Folder, "settings.json");
      if(File.Exists(settingsFile))
      {
        var json = File.ReadAllText(settingsFile);
        var settings = JsonConvert.DeserializeObject<GitterbackSettings>(json);
        if(settings == null)
        {
          Trace.TraceWarning(
            $"Settings file '{settingsFile}' is empty or invalid.");
          _settings = new GitterbackSettings();
          MarkAsDirty();
        }
        else
        {
          _settings = settings;
        }
      }
      else
      {
        _settings = new GitterbackSettings();
        MarkAsDirty();
      }
      SaveIfDirty();
    }
    return _settings;
  }

  /// <summary>
  /// Save the settings if they are dirty.
  /// </summary>
  /// <returns></returns>
  public bool SaveIfDirty()
  {
    if(IsDirty)
    {
      SaveSettings();
      IsDirty = false;
      return true;
    }
    return false;
  }

  /// <summary>
  /// Check if the given anchor name is deemed valid.
  /// Valid names are a sequence of one ore more identifier-like
  /// segments, separated by '-', '_', or '.'.
  /// </summary>
  public static bool IsValidAnchorName(string? anchorName)
  {
    if(String.IsNullOrEmpty(anchorName))
    {
      return false;
    }
    if(anchorName.Length > 30 || anchorName.Length < 3)
    {
      return false;
    }
    if(!Regex.IsMatch(
      anchorName,
      @"^[a-zA-Z][a-zA-Z0-9]*([-_.][a-zA-Z0-9]+)*$"))
    {
      return false;
    }
    return true;
  }

  private void SaveSettings()
  {
    var settingsFile = Path.Combine(Folder, "settings.json");
    var json = JsonConvert.SerializeObject(
      _settings,
      Formatting.Indented);
    File.WriteAllText(settingsFile, json);
  }

  /// <summary>
  /// Add a new anchor to the settings. The anchor name must
  /// meet the requirements of <see cref="IsValidAnchorName(string?)"/>.
  /// </summary>
  /// <param name="anchorName">
  /// The name of the anchor. Must be valid and must not exist yet.
  /// </param>
  /// <param name="anchorFolder">
  /// The folder to use as anchor. This folder will be created if it
  /// does not exist yet.
  /// </param>
  public Anchor AddAnchor(
    string anchorName,
    string anchorFolder)
  {
    if(!IsValidAnchorName(anchorName))
    {
      throw new ArgumentException(
        $"Invalid anchor name '{anchorName}'.");
    }
    anchorFolder = Path.GetFullPath(anchorFolder);
    var settings = GetSettings();
    if(settings.Anchors.ContainsKey(anchorName))
    {
      throw new ArgumentException(
        $"Anchor '{anchorName}' already exists.");
    }
    // assume the caller has checked against duplicate targets
    if(!Directory.Exists(anchorFolder))
    {
      Directory.CreateDirectory(anchorFolder);
    }
    var lastChar = anchorFolder[^1];
    if(lastChar == Path.DirectorySeparatorChar ||
       lastChar == Path.AltDirectorySeparatorChar)
    {
      throw new ArgumentException(
        $"Anchor folder '{anchorFolder}' must not end with a separator.");
    }
    var ainf = new AnchorInfo(anchorFolder);
    var anchor = new Anchor(this, anchorName, ainf);
    settings.Anchors.Add(
      anchorName,
      ainf);
    _anchors.Add(anchorName, anchor);
    MarkAsDirty();
    SaveIfDirty();
    anchor.InitTag();
    return anchor;
  }

  /// <summary>
  /// Find an anchor by name. The name is case insensitive.
  /// </summary>
  public Anchor? FindAnchor(string anchorName)
  {
    return _anchors.TryGetValue(anchorName, out var anchor) ? anchor : null;
  }

  /// <summary>
  /// Calculate the mappings for the repository containing the
  /// witness folder.
  /// </summary>
  /// <param name="witnessFolder">
  /// Any folder inside the repository. If null, the current
  /// working directory is used.
  /// </param>
  /// <param name="mode">
  /// The mode (fetch or push) to select the mappings for. If null,
  /// all modes are included.
  /// </param>
  /// <returns></returns>
  public IEnumerable<RemoteMapping> GetMappingsForRepo(
    string? witnessFolder = null,
    string? mode = null)
  {
    var repoRoot = GitRepoFolder.LocateRepoRootFrom(
      witnessFolder ?? Environment.CurrentDirectory);
    if(repoRoot == null)
    {
      yield break;
    }
    var remotes = GitRunner.GetRemotes(repoRoot.Folder, out var result);
    if(remotes == null)
    {
      yield break;
    }
    foreach(var remote in remotes.Remotes.Values)
    {
      foreach(var grt in remote.Targets)
      {
        if(mode != null && grt.Mode != mode)
        {
          continue;
        }
        var target = grt.Target;
        if(target.Length < 5 || target[1] != ':')
        {
          // not a windows filesystem path
          continue;
        }
        if(!Directory.Exists(target))
        {
          // not a valid target
          continue;
        }
        var anchorPart = Path.GetDirectoryName(target);
        if(anchorPart == null)
        {
          // not a valid target
          continue;
        }
        var bareRepoName = Path.GetFileName(target);
        if(bareRepoName == null)
        {
          // not a valid target
          continue;
        }
        var anchorId = FileIdentifier.FromPath(anchorPart);
        if(anchorId == null)
        {
          // not a valid target
          continue;
        }
        var settings = GetSettings();
        foreach(var kvp in settings.FindSameFolders(anchorId))
        {
          // TODO: missing FindSameFolder but for Anchors instead of AnchorInfo
          throw new NotImplementedException(
            "GetMappingsForRepo not implemented yet.");
        }
      }
      //
    }

    //
    throw new NotImplementedException(
      "GetMappingsForRepo not implemented yet.");
  }
}
