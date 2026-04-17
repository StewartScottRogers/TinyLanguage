@echo off
echo Running 00058.string_build.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000058.string_build.tlg output.txt
) else (
    TinyLanguage.exe %~dp000058.string_build.tlg %2
)
