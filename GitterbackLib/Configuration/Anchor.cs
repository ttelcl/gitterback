/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitterbackLib.Configuration;

/// <summary>
/// Runtime anchor model
/// </summary>
public class Anchor
{
  /// <summary>
  /// Create a new Anchor
  /// </summary>
  internal Anchor(
    SettingsStore store,
    string name,
    AnchorInfo ainf)
  {
    Store = store;
    _info = ainf;
    AnchorName = name;
  }

  /// <summary>
  /// The store where the settings this anchor is part of are stored.
  /// </summary>
  public SettingsStore Store { get; }

  /// <summary>
  /// The settings that this anchor is part of. Volatile! So the
  /// actual Info setting may be a different instance.
  /// </summary>
  public GitterbackSettings Settings { get => Store.GetSettings(); }

  /// <summary>
  /// The name of the anchor.
  /// </summary>
  public string AnchorName { get; }

  /// <summary>
  /// The anchor information (currently just the folder).
  /// Setting this will mark the store as dirty, but not save it.
  /// </summary>
  public AnchorInfo Info { 
    get => _info;
    set {
      _info = value;
      Store.MarkAsDirty();
    }
  }
  private AnchorInfo _info;

  /// <summary>
  /// Initialize the tag folder for this anchor.
  /// </summary>
  public void InitTag()
  { 
    var tagFolder = Path.Combine(
      Info.AnchorFolder,
      ".gitterback");
    if(!Directory.Exists(tagFolder))
    {
      Directory.CreateDirectory(tagFolder);
    }
    var metadataFileName = Path.Combine(
      tagFolder,
      "metadata.json");
    if(!File.Exists(metadataFileName))
    {
      File.WriteAllText(
        metadataFileName,
        "{}");
    }
  }
}
