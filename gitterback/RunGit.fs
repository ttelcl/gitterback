module RunGit

open System
open System.IO

open GitterbackLib.GitThings

open ColorPrint

type Result<'a> =
    | SuccessClean of 'a
    | SuccessStatus of 'a * 'a * int
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
    let result = GitRunner.RunToLines(args, workingDirectory)
    if result.StatusCode = 0 then
      SuccessClean result.OutputLines
    else
      SuccessStatus (result.OutputLines, result.ErrorLines, result.StatusCode)
  with
  | ex ->
    Failure ex

let printCommandLine (result: GitRunResult) =
  cpx "\fb> \fo\vBgit\f0"
  for arg in result.Arguments do
    cpx $" \fw\vB{arg}\f0"
  cp "\f0"

