@echo off
echo Running 00207.type_casting.tlg
if "%2"=="" (
    TinyLanguage.exe 00207.type_casting.tlg output.txt
) else (
    TinyLanguage.exe 00207.type_casting.tlg %2
)
