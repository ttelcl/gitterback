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
/// Content for an anchor, excluding its name.
/// </summary>
public class AnchorInfo
{
  /// <summary>
  /// Create a new AnchorInfo
  /// </summary>
  public AnchorInfo(
    string folder)
  {
    folder = Path.GetFullPath(folder);
    if(!Directory.Exists(folder))
    {
      Trace.TraceWarning(
        $"Anchor folder '{folder}' does not exist.");
    }
    AnchorFolder = folder;
  }

  /// <summary>
  /// The folder acting as anchor for repositories.
  /// </summary>
  [JsonProperty("folder")]
  public string AnchorFolder { get; }

}
