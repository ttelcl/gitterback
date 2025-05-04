/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitterbackLib.GitThings;

/// <summary>
/// Description of GitRemoteTarget
/// </summary>
public class GitRemoteTarget
{
  /// <summary>
  /// Create a new GitRemoteTarget
  /// </summary>
  public GitRemoteTarget(
    string mode,
    string target)
  {
    Mode = mode;
    Target = target;
  }

  /// <summary>
  /// The access mode for the remote target
  /// (e.g. "push" or "fetch").
  /// </summary>
  public string Mode { get; }

  /// <summary>
  /// The target of the remote. A URL or a local path.
  /// </summary>
  public string Target { get; }

}
