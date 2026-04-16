@echo off
echo Running 00160.clamp_function.tlg
if "%2"=="" (
    TinyLanguage.exe 00160.clamp_function.tlg output.txt
) else (
    TinyLanguage.exe 00160.clamp_function.tlg %2
)
