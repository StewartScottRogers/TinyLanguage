@echo off
echo Running 00160.clamp_fn.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000160.clamp_fn.tlg output.txt
) else (
    TinyLanguage.exe %~dp000160.clamp_fn.tlg %2
)
