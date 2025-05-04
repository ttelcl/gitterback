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
/// Static utilities for running git commands.
/// </summary>
public static class GitRunner
{
  /// <summary>
  /// Run a (git) command and return the output as a list of lines.
  /// </summary>
  /// <param name="args">
  /// Arguments to pass to the command.
  /// </param>
  /// <param name="workingDirectory">
  /// Working directory (default: current directory).
  /// </param>
  /// <param name="status">
  /// The exit status of the command.
  /// </param>
  /// <param name="command">
  /// The command to run (default: "git").
  /// </param>
  /// <returns></returns>
  public static List<string> RunToLines(
    IEnumerable<string> args,
    string? workingDirectory,
    out int status,
    string command = "git")
  {
    var startInfo = new ProcessStartInfo {
      FileName = command,
      RedirectStandardOutput = true,
      //RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
    };
    foreach(var arg in args)
    {
      startInfo.ArgumentList.Add(arg);
    }
    var outputLines = new List<string>();
    using(var process = new Process { StartInfo = startInfo })
    {
      process.OutputDataReceived += (sender, e) => {
        if(e.Data != null)
        {
          outputLines.Add(e.Data);
        }
      };
      process.Start();
      process.BeginOutputReadLine();
      process.WaitForExit();
      // The process is closed next, which may cause extra lines
      // to be flushed. Only after that we can be sure that all
      // output has been received.
      status = process.ExitCode;
    }
    return outputLines;
  }

  /// <summary>
  /// Retrieve the git remotes for the current repository
  /// </summary>
  /// <param name="workingDirectory">
  /// The folder to derive the repository from. If null, the
  /// current directory is used.
  /// </param>
  /// <param name="status">
  /// Status of the command. 0 if successful, otherwise
  /// another number.
  /// </param>
  /// <returns>
  /// The created GitRemotes object, or null if the command
  /// failed with non-zero status.
  /// </returns>
  public static GitRemotes? GetRemotes(
    string? workingDirectory,
    out int status)
  {
    var lines = RunToLines(
      ["remote", "-v"],
      workingDirectory,
      out status);
    if(status != 0)
    {
      return null;
    }
    return GitRemotes.FromLines(lines);
  }
}
