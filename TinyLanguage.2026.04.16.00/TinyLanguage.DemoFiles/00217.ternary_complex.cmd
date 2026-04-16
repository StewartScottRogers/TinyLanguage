@echo off
echo Running 00217.ternary_complex.tlg
if "%2"=="" (
    TinyLanguage.exe 00217.ternary_complex.tlg output.txt
) else (
    TinyLanguage.exe 00217.ternary_complex.tlg %2
)
