module CmdAnchors

open System
open System.IO

open GitterbackLib.Configuration

open CommonTools
open ColorPrint

let run args =
  let store = SettingsStore()
  let settingsFile = Path.Combine(store.Folder, "settings.json")
  cp $"Using gitterback settings from \fg{settingsFile}\f0."
  let settings = store.GetSettings()
  let anchors = settings.Anchors
  if anchors.Count = 0 then
    cp $"\foNo anchors registered\f0 )"
  else
    for kvp in anchors do
      cp $"\fg{kvp.Key,15}\f0: \f0{kvp.Value.AnchorFolder}\f0 "
  1
