pause

REM set PATH="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow"

set PATH=%~dp0\\VSTestConsole

cd "C:\MSBuildProjects-beta\NUNIT\nunit-gui\bin\Debug"

vstest.console /ListTests:"C:\MSBuildProjects-beta\NUNIT\nunit-gui\\bin\Debug\\nunit-gui.exe"  > results.vs 

pause
