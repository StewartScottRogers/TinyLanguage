@echo off
echo Running 00264.currying.tlg
if "%2"=="" (
    TinyLanguage.exe 00264.currying.tlg output.txt
) else (
    TinyLanguage.exe 00264.currying.tlg %2
)
