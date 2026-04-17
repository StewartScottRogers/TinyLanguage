@echo off
echo Running 00250.matrix_multiply.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000250.matrix_multiply.tlg output.txt
) else (
    TinyLanguage.exe %~dp000250.matrix_multiply.tlg %2
)
