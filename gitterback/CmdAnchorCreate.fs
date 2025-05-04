module CmdAnchorCreate

open System
open System.IO

open GitterbackLib.Configuration
open GitterbackLib.Utilities

open ColorPrint
open CommonTools

type private Options = {
  AnchorName: string
  AnchorFolder: string 
}

let run args =
  let store = SettingsStore()
  let settingsFile = Path.Combine(store.Folder, "settings.json")
  cp $"Using gitterback settings from \fg{settingsFile}\f0."
  let settings = store.GetSettings()
  let anchors = settings.Anchors
  
  let validateOptions o =
    if String.IsNullOrWhiteSpace(o.AnchorName) then
      cp "\frNo anchor name given\f0."
      None
    elif o.AnchorName |> SettingsStore.IsValidAnchorName |> not then
      cp $"\frError: \foAnchor name '\fy{o.AnchorName}\fo' is not valid\f0."
      cp "Expecting a sequence of one or more identifiers, separated by '-', '.' or '_'"
      None
    elif o.AnchorName |> anchors.ContainsKey then
      let ainf = anchors.[o.AnchorName]
      cp $"\frError: \foAnchor name '\fy{o.AnchorName}\fo' already exists\f0 ({ainf.AnchorFolder})"
      None
    elif String.IsNullOrWhiteSpace(o.AnchorFolder) then
      cp "\frNo anchor folder given\f0."
      None
    else
      let anchorFolder = o.AnchorFolder
      let anchorFolder =
        if anchorFolder = "." then
          Environment.CurrentDirectory
        else
          Path.GetFullPath(o.AnchorFolder)
      let anchorFolder =
        anchorFolder.TrimEnd(
          Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)   
      let o = { o with AnchorFolder = anchorFolder }
      if Directory.Exists(o.AnchorFolder) |> not then
        cp $"\frError: \foAnchor folder '\fy{o.AnchorFolder}\fo' does not exist\f0."
        None
      else
        let fid = o.AnchorFolder |> FileIdentifier.FromPath
        if fid = null then
          cp $"\frError: \foAnchor folder '\fy{o.AnchorFolder}\fo' is not accessible\f0."
          None
        else
          let duplicates =
            settings.FindSameFolders(fid)
            |> Seq.toArray
          if duplicates.Length > 0 then
            cp $"\frError: \foAnchor folder '\fy{o.AnchorFolder}\fo' is already used by these anchors\f0:"
            for d in duplicates do
              cp $"\fy{d.Key,-15}\f0 ({d.Value.AnchorFolder})"
            None
          else
            Some(o)
  
  let rec parseMore o args =
    match args with
    | [] ->
      validateOptions(o)
    | "-v" :: rest ->
      verbose <- true
      parseMore o rest
    | "-h" :: _ ->
      None
    | "-a" :: name :: rest ->
      parseMore { o with AnchorName = name } rest
    | "-f" :: folder :: rest ->
      let folder = folder |> Path.GetFullPath
      parseMore { o with AnchorFolder = folder } rest
    | _ ->
      cp $"\foUnknown option\f0: \fy{args}\f0."
      None
  
  let oo = args |> parseMore {
    AnchorName = null;
    AnchorFolder = null
  }

  match oo with
  | None ->
    Usage.usage "anchor-create"
    1
  | Some o ->
    cp $"Adding anchor \fy{o.AnchorName}\f0 = \fg{o.AnchorFolder}\f0."
    let anchor = store.AddAnchor(o.AnchorName, o.AnchorFolder)
    0
