@echo off
echo Running 00267.state_machine.tlg
if "%2"=="" (
    TinyLanguage.exe 00267.state_machine.tlg output.txt
) else (
    TinyLanguage.exe 00267.state_machine.tlg %2
)
