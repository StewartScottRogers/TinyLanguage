@echo off
setlocal
set "OUTPUT=%~1"
if "%OUTPUT%"=="" set "OUTPUT=%~2"
if "%OUTPUT%"=="" set "OUTPUT=%~dp0output.txt"
"%~dp0TinyLanguage.exe" "%~dp000033.if_stmt_no_else.tlg" "%OUTPUT%"
if errorlevel 1 (
    echo TinyLanguage.exe exited with code %ERRORLEVEL% 1>&2
    exit /b %ERRORLEVEL%
)
type "%OUTPUT%"
exit /b 0

