@echo off
echo Running 00274.clamp_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000274.clamp_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000274.clamp_function.tlg %2
)
