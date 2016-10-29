
$c = (Get-Item -Path "..\..\..\packages" -Verbose).FullName

$p = "install elmah -o " + $c

$p

#$r = Start-Process -FilePath "Nuget.exe" -ArgumentList $p -WorkingDirectory "." -WindowStyle Hidden

# Setup the Process startup info
$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = "Nuget.exe"
$pinfo.Arguments = $p
$pinfo.UseShellExecute = $false
$pinfo.CreateNoWindow = $false
$pinfo.RedirectStandardOutput = $true
$pinfo.RedirectStandardError = $true

# Create a process object using the startup info
$process = New-Object System.Diagnostics.Process
$process.StartInfo = $pinfo

# Start the process
$process.Start() | Out-Null

# get output from stdout and stderr
$stdout = $process.StandardOutput.ReadToEnd()
$stderr = $process.StandardError.ReadToEnd()

return $stdout

