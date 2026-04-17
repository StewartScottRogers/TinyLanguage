@echo off
echo Running 00040.compare_chain_logic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000040.compare_chain_logic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000040.compare_chain_logic.tlg %2
)
