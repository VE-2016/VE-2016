pause

REM set PATH="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow"

set PATH=%~dp0\\VSTestConsole

cd %~dp0\\..\\..\\..\\..\\VE-Tests\\bin\\Debug

vstest.console VE-Tests.dll

pause
