@echo off
echo Running 00169.rotate_array2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000169.rotate_array2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000169.rotate_array2.tlg %2
)
