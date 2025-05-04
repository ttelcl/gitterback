// (c) 2025  ttelcl / ttelcl
module Usage

open CommonTools
open ColorPrint

let usage focus =
  if focus = "" then
    cp "\fogitterback\f0 is a tool for managing backups of GIT repositories"
    cp "as local bare repositories"
  if focus = "" || focus = "anchors" || focus = "anchor" then
    cp ""
    cp "\fogitterback anchors\f0"
    cp "   Lists your anchor folders (where the backup bare repos are)"
  if focus = "" || focus = "anchor-create" || focus = "anchor" then
    cp ""
    cp "\fogitterback \fyanchor create \fg-a \fc<anchor-name> \fg-f \fc<anchor-folder>\f0"
    cp "   Register a new anchor with the given alias name"
  if focus = "" || focus = "setup" then
    cp ""
    cp "\fogitterback \fysetup \fg-a \fc<anchor> \fg-n \fc<reponame>\f0 [\fg-r \fc<remote>\f0]"
    cp "   Create a new bare repository in the given \fc<anchor>\f0 folder,"
    cp "   with the given \fc<reponame>\f0 to backup the current GIT repository."
    cp "   The \fc<reponame>\f0 is the name of the bare repository, without the .git suffix."
    cp "   The \fc<anchor>\f0 folder must have been previously registered with "
    cp "   the \fyanchor create\f0 command."
    cp "   This command will also register anew remote named \fc<remote>\f0 in"
    cp "   your GIT repo pointing to the bare repository, unless that remote already exists."
    cp "   \fc<remote>\f0 defaults to \fc<anchor>\f0."
  if focus = "" then
    cp ""
    cp "\fg-v               \f0Verbose mode"



