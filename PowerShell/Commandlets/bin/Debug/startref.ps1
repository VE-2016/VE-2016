
import-module .\commandlets.dll

get-module

$p = (Get-Item -Path ".\" -Verbose).FullName

$r = "F:\Application\MSBuildProjects-new\packages\elmah.corelibrary.1.2.2\lib\elmah.dll"

$n = "Commandlets"

$s = "F:\Application\MSBuildProjects-new\VStudio.sln"

Get-ReferenceNugetPackage $p $s $r $n