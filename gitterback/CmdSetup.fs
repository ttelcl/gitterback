module CmdSetup

open System
open System.IO

open GitterbackLib
open GitterbackLib.Configuration
open GitterbackLib.Utilities
open GitterbackLib.GitThings

open ColorPrint
open CommonTools

type private Options = {
  AnchorName: string
  RepoName: string
  RemoteName: string
  // the remaining values are created during validation of the above
  TargetFolder: string
  Root: GitRepoFolder
}

let run args =
  
  let root = GitRepoFolder.LocateRepoRootFrom(Environment.CurrentDirectory)
  if root = null then
    cp "\frError: \foNot in a git repository\f0."
    1
  else
    let store = SettingsStore()
    let settings = store.GetSettings()
    let validateOptions o =
      let repoName =
        if String.IsNullOrWhiteSpace(o.RepoName) then
          Path.GetFileName(root.Folder)
        else
          o.RepoName
      let anchorName =
          o.AnchorName
      let remoteName =
        if String.IsNullOrWhiteSpace(o.RemoteName) then
          anchorName
        else
          o.RemoteName
      if String.IsNullOrWhiteSpace(anchorName) then
        cp "\frNo anchor name given\f0."
        None
      elif anchorName |> SettingsStore.IsValidAnchorName |> not then
        cp $"\frError: \foAnchor name '\fy{anchorName}\fo' is not valid\f0."
        cp "Expecting a sequence of one or more identifiers, separated by '-', '.' or '_'"
        None
      else
        let anchor = store.FindAnchor anchorName
        if anchor = null then
          cp $"\frError: \foAnchor name '\fy{anchorName}\fo' does not exist\f0."
          None
        else
          let anchorFolder = anchor.Info.AnchorFolder
          let targetName = Path.Combine(anchorFolder, repoName)
          let targetFolder = targetName + ".git"
          if targetFolder |> Directory.Exists then
            cp $"\frError: \foTarget folder '\fy{targetFolder}\fo' already exists\f0."
            None
          elif remoteName |> SettingsStore.IsValidAnchorName |> not then
            cp $"\foRemote name '\fy{repoName}\fo' is not valid\f0."
            None
          else
            let remotes, status = GitRunner.GetRemotes(root.Folder)
            if status <> 0 || remotes = null then
              cp $"\frError: \foCannot read remotes from git repository\f0 Status \fr{status}\f0."
              None
            else
              let existing = remotes[remoteName]
              if existing = null then
                let o = { 
                  AnchorName = anchorName
                  RepoName = repoName
                  RemoteName = remoteName
                  TargetFolder = targetFolder
                  Root = root
                }
                Some o
              else
                cp $"\frError: \foRemote name '\fy{remoteName}\fo' already is in use\f0."
                for kvp in existing.Targets do
                  cp $" \fy{kvp.Mode,8}\f0: {kvp.Target}\f0"
                None
      
    let rec parseMore o args =
      match args with
      | [] ->
        o |> validateOptions
      | "-v" :: rest ->
        verbose <- true
        rest |> parseMore o
      | "-h" :: _ ->
        None
      | "-a" :: name :: rest ->
        rest |> parseMore { o with AnchorName = name }
      | "-n" :: name :: rest ->
        rest |> parseMore { o with RepoName = name }
      | "-r" :: name :: rest ->
        rest |> parseMore { o with RemoteName = name }
      | x :: _ ->
        cp $"\frError: \foUnknown option '\fy{x}\fo' for setup\f0."
        None
    
    let oo = args |> parseMore {
      AnchorName = null
      RepoName = null
      RemoteName = null
      TargetFolder = null
      Root = null
    }
    match oo with
    | None ->
      Usage.usage "setup"
      1
    | Some o ->
      cp $"\fgCreating backup for git repository at \fy{root.Folder}\f0."
      cp $"\fgUsing anchor \fy{o.AnchorName}\f0."
      cp $"\fgUsing backup repo name \fy{o.RepoName}\f0."
      cp $"   -> \fc{o.TargetFolder}\f0."
      cp $"\fgUsing remote name \fy{o.RemoteName}\f0."

      cp "\frNot yet implemented\f0."
      Usage.usage "setup"
      1
