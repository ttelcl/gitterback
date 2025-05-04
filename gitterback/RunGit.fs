module RunGit

open System
open System.IO

open GitterbackLib

type Result<'a> =
    | Success of 'a
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
    GitRunner.RunToLines(args, workingDirectory) |> Success
  with
  | ex ->
    Failure ex
  
    