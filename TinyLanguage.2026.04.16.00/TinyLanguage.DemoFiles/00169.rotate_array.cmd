@echo off
echo Running 00169.rotate_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00169.rotate_array.tlg output.txt
) else (
    TinyLanguage.exe 00169.rotate_array.tlg %2
)
