module CmdAnchorCreate

open System
open System.IO

open GitterbackLib.Configuration

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
      cp $"\frError: \foAnchor name '\fy{o.AnchorName}\fo' already exists\f0."
      None
    elif String.IsNullOrWhiteSpace(o.AnchorFolder) then
      cp "\frNo anchor folder given\f0."
      None
    else
      let o =
        if o.AnchorFolder = "." then
          { o with AnchorFolder = Environment.CurrentDirectory }
        else
          { o with AnchorFolder = Path.GetFullPath(o.AnchorFolder) }
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
  
  cp "\frNot yet implemented\f0."
  Usage.usage "anchor-create"
  1
