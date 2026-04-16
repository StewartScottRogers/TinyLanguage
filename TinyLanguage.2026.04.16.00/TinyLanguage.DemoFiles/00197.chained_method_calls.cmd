@echo off
echo Running 00197.chained_method_calls.tlg
if "%2"=="" (
    TinyLanguage.exe 00197.chained_method_calls.tlg output.txt
) else (
    TinyLanguage.exe 00197.chained_method_calls.tlg %2
)
