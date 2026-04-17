@echo off
echo Running 00256.scope_let_in_block.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000256.scope_let_in_block.tlg output.txt
) else (
    TinyLanguage.exe %~dp000256.scope_let_in_block.tlg %2
)
