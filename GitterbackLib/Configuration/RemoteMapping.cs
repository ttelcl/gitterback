/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GitterbackLib.GitThings;

namespace GitterbackLib.Configuration;

/// <summary>
/// Connects GIT remote information with anchor information.
/// </summary>
public class RemoteMapping
{
  /// <summary>
  /// Create a new RemoteMapping
  /// </summary>
  public RemoteMapping(
    string repoRoot,
    Anchor anchor,
    GitRemoteTarget target)
  {
    RepoRoot = repoRoot;
    Anchor = anchor;
    Target = target;
  }

  /// <summary>
  /// The main repository folder
  /// </summary>
  public string RepoRoot { get; }

  /// <summary>
  /// The anchor housing the remote
  /// </summary>
  public Anchor Anchor { get; }

  /// <summary>
  /// Target information: the name of the remote, the full
  /// target folder and the access mode (push/fetch).
  /// </summary>
  public GitRemoteTarget Target { get; }

  // accessor shortcuts

  /// <summary>
  /// The logical name of the anchor.
  /// </summary>
  public string AnchorName => Anchor.AnchorName;

  /// <summary>
  /// The parent folder for the bare repository (target)
  /// </summary>
  public string AnchorFolder => Anchor.Info.AnchorFolder;

  /// <summary>
  /// The target folder. If all went well this should be
  /// the combination of AnchorFolder and AnchorName.
  /// </summary>
  public string TargetFolder => Target.Target;

  /// <summary>
  /// The name of the remote. May or may not be equal to
  /// <see cref="AnchorName"/>.
  /// </summary>
  public string RemoteName => Target.RemoteName;

  /// <summary>
  /// The direction of the remote. Usually there are two
  /// instances of the same remote, one for push and one
  /// for fetch.
  /// </summary>
  public string Mode => Target.Mode;
}
