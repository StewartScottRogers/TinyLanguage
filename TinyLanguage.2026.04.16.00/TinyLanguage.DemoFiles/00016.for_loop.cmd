@echo off
echo Running 00016.for_loop.tlg
if "%2"=="" (
    TinyLanguage.exe 00016.for_loop.tlg output.txt
) else (
    TinyLanguage.exe 00016.for_loop.tlg %2
)
