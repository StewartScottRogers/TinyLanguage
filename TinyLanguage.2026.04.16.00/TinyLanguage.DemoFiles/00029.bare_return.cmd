@echo off
echo Running 00029.bare_return.tlg
if "%2"=="" (
    TinyLanguage.exe 00029.bare_return.tlg output.txt
) else (
    TinyLanguage.exe 00029.bare_return.tlg %2
)
