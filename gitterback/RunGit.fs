module RunGit

open System
open System.IO

open GitterbackLib.GitThings

type Result<'a> =
    | SuccessClean of 'a
    | SuccessStatus of 'a * int
    | Failure of Exception

let runGit args workDir =
  let workingDirectory =
    match workDir with
    | None
    | Some("")
    | Some(null) ->
      Environment.CurrentDirectory
    | Some dir ->
      dir
  let args =
    args
    |> Seq.toArray
  try
    let lines, status = GitRunner.RunToLines(args, workingDirectory)
    if status = 0 then
      SuccessClean lines
    else
      SuccessStatus (lines, status)
  with
  | ex ->
    Failure ex
  
    