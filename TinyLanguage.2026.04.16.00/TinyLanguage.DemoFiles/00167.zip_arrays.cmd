@echo off
echo Running 00167.zip_arrays.tlg
if "%2"=="" (
    TinyLanguage.exe 00167.zip_arrays.tlg output.txt
) else (
    TinyLanguage.exe 00167.zip_arrays.tlg %2
)
