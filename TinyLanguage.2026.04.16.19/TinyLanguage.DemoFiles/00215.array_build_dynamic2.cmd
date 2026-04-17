@echo off
echo Running 00215.array_build_dynamic2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000215.array_build_dynamic2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000215.array_build_dynamic2.tlg %2
)
