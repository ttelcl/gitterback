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
/// Model for the gitterback per-user settings.
/// JSON serializable
/// </summary>
public class GitterbackSettings
{
  /// <summary>
  /// Create a new GitterbackSettings
  /// </summary>
  public GitterbackSettings(
    int version = GitterbackVersion,
    IDictionary<string, AnchorInfo>? anchors = null)
  {
    Version = version;
    if(anchors == null)
    {
      Anchors = new Dictionary<string, AnchorInfo>(
        StringComparer.OrdinalIgnoreCase);
    }
    else
    {
      Anchors = new Dictionary<string, AnchorInfo>(
        anchors,
        StringComparer.OrdinalIgnoreCase);
    }
  }

  /// <summary>
  /// The currently implemented version of the settings file format
  /// </summary>
  public const int GitterbackVersion = 1;

  /// <summary>
  /// The version of the settings file.
  /// </summary>
  [JsonProperty("version")]
  public int Version { get; }

  /// <summary>
  /// The anchors for the repositories. The key names are case insensitive.
  /// </summary>
  [JsonProperty("anchors")]
  public Dictionary<string, AnchorInfo> Anchors { get; }

}
