@echo off
echo Running 00288.zip_unzip.tlg
if "%2"=="" (
    TinyLanguage.exe 00288.zip_unzip.tlg output.txt
) else (
    TinyLanguage.exe 00288.zip_unzip.tlg %2
)
