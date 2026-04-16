@echo off
echo Running 00091.float_literals.tlg
if "%2"=="" (
    TinyLanguage.exe 00091.float_literals.tlg output.txt
) else (
    TinyLanguage.exe 00091.float_literals.tlg %2
)
