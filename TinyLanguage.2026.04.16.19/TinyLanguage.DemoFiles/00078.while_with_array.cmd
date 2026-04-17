@echo off
echo Running 00078.while_with_array.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000078.while_with_array.tlg output.txt
) else (
    TinyLanguage.exe %~dp000078.while_with_array.tlg %2
)
