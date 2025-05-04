module CmdDev

// test scratchpad

open System
open System.IO

open RunGit
open ColorPrint

let run args =
  let result =
    runGit [| "log" |] None
    //runGit [| "remote"; "-v" |] None

  match result with
  | Success lines ->
    cp $"\fgReceived these \fb{lines.Count}\fg lines\f0:"
    for line in lines do
      cp $"\fG[\f0{line}\fG]\f0"
    0
  | Failure ex ->
    cp $"\frError: \fy{ex.Message}\f0."
    1
