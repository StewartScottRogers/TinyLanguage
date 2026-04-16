@echo off
echo Running 00013.inline_conditional.tlg
if "%2"=="" (
    TinyLanguage.exe 00013.inline_conditional.tlg output.txt
) else (
    TinyLanguage.exe 00013.inline_conditional.tlg %2
)
