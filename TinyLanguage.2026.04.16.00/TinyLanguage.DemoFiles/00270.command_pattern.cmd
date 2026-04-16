@echo off
echo Running 00270.command_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe 00270.command_pattern.tlg output.txt
) else (
    TinyLanguage.exe 00270.command_pattern.tlg %2
)
