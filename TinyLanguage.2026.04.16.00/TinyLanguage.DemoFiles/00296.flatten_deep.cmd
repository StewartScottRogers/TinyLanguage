@echo off
echo Running 00296.flatten_deep.tlg
if "%2"=="" (
    TinyLanguage.exe 00296.flatten_deep.tlg output.txt
) else (
    TinyLanguage.exe 00296.flatten_deep.tlg %2
)
