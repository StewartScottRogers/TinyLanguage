@echo off
echo Running 00159.max_min_fns.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000159.max_min_fns.tlg output.txt
) else (
    TinyLanguage.exe %~dp000159.max_min_fns.tlg %2
)
