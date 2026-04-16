@echo off
echo Running 00131.unary_minus.tlg
if "%2"=="" (
    TinyLanguage.exe 00131.unary_minus.tlg output.txt
) else (
    TinyLanguage.exe 00131.unary_minus.tlg %2
)
