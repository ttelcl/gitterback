module CmdInfo

open System
open System.IO

open GitterbackLib
open GitterbackLib.Configuration
open GitterbackLib.GitThings

open ColorPrint
open CommonTools


let run args =
  let store = SettingsStore()
  let settings = store.GetSettings()
  let witness = Environment.CurrentDirectory
  let root = GitRepoFolder.LocateRepoRootFrom(witness)
  if root = null then
    cp "\foNot in a git repository\f0."
    1
  else
    cp $"Git root folder: \fg{root.Folder}\f0."
    let mappings =
      store.GetMappingsForRepo(root.Folder, null)
      |> Seq.toArray
    if mappings.Length = 0 then
      cp "\foGitterback has not been set up for this repository\f0."
    for mapping in mappings do
      cpx $"\fb{mapping.Mode,6}\f0: remote=\fy{mapping.RemoteName}\f0"
      cp $" anchor=\fg{mapping.AnchorName}\f0 (\fc{mapping.TargetFolder}\f0)."
    0
