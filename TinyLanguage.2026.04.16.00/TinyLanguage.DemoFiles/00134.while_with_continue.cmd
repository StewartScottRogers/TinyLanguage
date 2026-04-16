@echo off
echo Running 00134.while_with_continue.tlg
if "%2"=="" (
    TinyLanguage.exe 00134.while_with_continue.tlg output.txt
) else (
    TinyLanguage.exe 00134.while_with_continue.tlg %2
)
