module CmdDev

// test scratchpad

open System
open System.IO

open GitterbackLib.GitThings

open RunGit
open ColorPrint

let run args =
  let result =
    //runGit [| "log" |] None
    runGit [| "remote"; "-v" |] None

  let status =
    match result with
    | SuccessStatus (lines, errors, status) ->
      let color = if status = 0 then "\fg" else "\fr"
      cp $"\fyReceived these \fb{lines.Count}\fy lines, with status {color}{status}\f0:"
      for line in lines do
        cp $"\fR[\f0{line}\fR]\f0"
      cp $"\frReceived these \fb{errors.Count}\fr error lines\f0:"
      for line in errors do
        cp $"\fR[\fy{line}\fR]\f0"
      1
    | SuccessClean lines ->
      cp $"Received these \fb{lines.Count}\f0 lines (status \fgOK\f0):"
      for line in lines do
        cp $"\fG[\f0{line}\fG]\f0"
      0
    | Failure ex ->
      cp $"\frError: \fy{ex.Message}\f0."
      1
  if status <> 0 then
    status
  else
    cp "Now via a different way"
    let remotes, result =
      GitRunner.GetRemotes(null)
    if result.StatusCode = 0 then
      cp "Received these remotes:"
      for kvp in remotes.Remotes do
        let remote = kvp.Value
        cp $"\fg{remote.Name}\f0:"
        for target in remote.Targets do
          cp $"\fb{target.Mode,8}\f0  {target.Target}\f0"
      0
    else
      cp $"\frError: \fy{status}\f0."
      if result.ErrorLines.Count > 0 then
        cp $"\foError lines\f0:"
        for line in result.ErrorLines do
          cp $"  \fy{line}\f0 "
      status
    
