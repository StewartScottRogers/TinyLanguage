@echo off
echo Running 00253.matrix_multiply.tlg
if "%2"=="" (
    TinyLanguage.exe 00253.matrix_multiply.tlg output.txt
) else (
    TinyLanguage.exe 00253.matrix_multiply.tlg %2
)
