@echo off
echo Running 00297.matrix_transpose.tlg
if "%2"=="" (
    TinyLanguage.exe 00297.matrix_transpose.tlg output.txt
) else (
    TinyLanguage.exe 00297.matrix_transpose.tlg %2
)
