pause

REM set PATH="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow"

set PATH=%~dp0\\VSTestConsole

cd "C:\Users\me\Documents\Visual Studio 2015\Projects\WindowsFormsApplication7-1\UnitTestProject-test\bin\Debug"

vstest.console /ListTests:"C:\Users\me\Documents\Visual Studio 2015\Projects\WindowsFormsApplication7-1\UnitTestProject-test\\bin\Debug\\UnitTestProject-test.dll"  > results.vs 

pause
