// (c) 2025  ttelcl / ttelcl

open System

open ColorPrint
open CommonTools
open ExceptionTool
open Usage

let rec run arglist =
  // For subcommand based apps, split based on subcommand here
  match arglist with
  | "-v" :: rest ->
    verbose <- true
    rest |> run
  | "--help" :: _
  | "-help" :: _
  | "help" :: _
  | "-h" :: _
  | [] ->
    usage ""
    0  // program return status code to the operating system; 0 == "OK"
  | "anchors" :: rest ->
    rest |> CmdAnchors.run
  | "anchor" :: "create" :: rest
  | "anchor-create" :: rest ->
    rest |> CmdAnchorCreate.run
  | "anchor" :: rest ->
    Usage.usage "anchor"
    1
  | "setup" :: rest ->
    rest |> CmdSetup.run
  | "devtest" :: rest ->
    rest |> CmdDev.run
  | x :: _ ->
    cp $"\foUnknown command\f0: \fy{x}\f0."
    1

[<EntryPoint>]
let main args =
  try
    args |> Array.toList |> run
  with
  | ex ->
    ex |> fancyExceptionPrint verbose
    resetColor ()
    1



