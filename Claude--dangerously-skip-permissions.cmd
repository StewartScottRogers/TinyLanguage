@echo off
setlocal

REM Save the original path
set "ORIG_DIR=%CD%"

REM Change to the directory where this script is located
pushd "%~dp0"

call claude --dangerously-skip-permissions

REM Return to the original directory
popd

endlocal