@echo off
echo Running 00209.complex_for_loop.tlg
if "%2"=="" (
    TinyLanguage.exe 00209.complex_for_loop.tlg output.txt
) else (
    TinyLanguage.exe 00209.complex_for_loop.tlg %2
)
