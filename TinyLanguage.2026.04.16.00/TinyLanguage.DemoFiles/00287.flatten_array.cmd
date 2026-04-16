@echo off
echo Running 00287.flatten_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00287.flatten_array.tlg output.txt
) else (
    TinyLanguage.exe 00287.flatten_array.tlg %2
)
