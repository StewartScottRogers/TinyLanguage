@echo off
echo Running 00015.while_loop.tlg
if "%2"=="" (
    TinyLanguage.exe 00015.while_loop.tlg output.txt
) else (
    TinyLanguage.exe 00015.while_loop.tlg %2
)
