@echo off
setlocal
set "OUTPUT=%~1"
if "%OUTPUT%"=="" set "OUTPUT=%~2"
if "%OUTPUT%"=="" set "OUTPUT=%~dp0output.txt"
"%~dp0TinyLanguage.exe" "%~dp000090.escape_newline.tlg" "%OUTPUT%"
if errorlevel 1 (
    echo TinyLanguage.exe exited with code %ERRORLEVEL% 1>&2
    exit /b %ERRORLEVEL%
)
type "%OUTPUT%"
exit /b 0

