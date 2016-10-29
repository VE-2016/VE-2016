
import-module .\commandlets.dll

get-module

$p = (Get-Item -Path ".\" -Verbose).FullName

Get-GetProject $p