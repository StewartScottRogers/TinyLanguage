@echo off
echo Running 00168.flatten_matrix.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000168.flatten_matrix.tlg output.txt
) else (
    TinyLanguage.exe %~dp000168.flatten_matrix.tlg %2
)
