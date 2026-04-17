@echo off
echo Running 00301.scope_chain_demo.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000301.scope_chain_demo.tlg output.txt
) else (
    TinyLanguage.exe %~dp000301.scope_chain_demo.tlg %2
)
