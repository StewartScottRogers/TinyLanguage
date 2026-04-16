@echo off
echo Running 00092.scope_chain.tlg
if "%2"=="" (
    TinyLanguage.exe 00092.scope_chain.tlg output.txt
) else (
    TinyLanguage.exe 00092.scope_chain.tlg %2
)
