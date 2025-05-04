/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GitterbackLib.Configuration;

/// <summary>
/// Represents the folder holding the per-machine, per-user
/// Gitterback settings.
/// </summary>
public class SettingsStore
{
  private GitterbackSettings? _settings;

  /// <summary>
  /// Create a new SettingsStore, creating the folder and
  /// settings file if they do not exist yet.
  /// </summary>
  public SettingsStore(
    string? folder = null)
  {
    Folder = Path.GetFullPath(folder ?? DefaultFolder);
    if(!Directory.Exists(Folder))
    {
      Directory.CreateDirectory(Folder);
    }
    GetSettings(); // Create the settings if they do not exist
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

  private void SaveSettings()
  {
    var settingsFile = Path.Combine(Folder, "settings.json");
    var json = JsonConvert.SerializeObject(
      _settings,
      Formatting.Indented);
    File.WriteAllText(settingsFile, json);
  }

}
