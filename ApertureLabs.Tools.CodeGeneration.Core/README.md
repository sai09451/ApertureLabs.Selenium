# ApertureLabs.Tools.CodeGenerator.Core

> A dotnet core tool built for executing custom code generators from a projects
> references (via NuGet, project ref, or assembly ref).

## What
Uses the code-file-models and compiled assembly of the'original' project and a
'target' project and passes these to all avialable code-generators found in the
compiled assembly. These code-generators are then executed and are trusted to
make the correct adjustments.

## How
Will either create or use the 'target' project. Then from the 'original'
project it will:
* Compile and load the built assembly (the original project must compile or
  this will fail).
* Retrieve all code-file-models of each document.
* Scan the built assembly for all code-generators.

Then for each code-generator found in the built assembly will be passed the
compiled assembly, all files included in the project, and the target project.