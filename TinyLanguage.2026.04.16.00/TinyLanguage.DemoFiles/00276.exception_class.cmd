@echo off
echo Running 00276.exception_class.tlg
if "%2"=="" (
    TinyLanguage.exe 00276.exception_class.tlg output.txt
) else (
    TinyLanguage.exe 00276.exception_class.tlg %2
)
