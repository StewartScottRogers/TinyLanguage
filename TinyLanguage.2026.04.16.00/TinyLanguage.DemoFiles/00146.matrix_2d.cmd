@echo off
echo Running 00146.matrix_2d.tlg
if "%2"=="" (
    TinyLanguage.exe 00146.matrix_2d.tlg output.txt
) else (
    TinyLanguage.exe 00146.matrix_2d.tlg %2
)
