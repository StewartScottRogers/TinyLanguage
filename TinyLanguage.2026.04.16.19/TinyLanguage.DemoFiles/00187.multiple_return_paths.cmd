@echo off
echo Running 00187.multiple_return_paths.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000187.multiple_return_paths.tlg output.txt
) else (
    TinyLanguage.exe %~dp000187.multiple_return_paths.tlg %2
)
