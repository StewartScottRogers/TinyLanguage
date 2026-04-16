@echo off
echo Running 00052.len_builtin.tlg
if "%2"=="" (
    TinyLanguage.exe 00052.len_builtin.tlg output.txt
) else (
    TinyLanguage.exe 00052.len_builtin.tlg %2
)
