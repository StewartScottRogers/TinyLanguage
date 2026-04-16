@echo off
echo Running 00030.multiple_params.tlg
if "%2"=="" (
    TinyLanguage.exe 00030.multiple_params.tlg output.txt
) else (
    TinyLanguage.exe 00030.multiple_params.tlg %2
)
