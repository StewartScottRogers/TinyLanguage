@echo off
echo Running 00277.recursive_eval.tlg
if "%2"=="" (
    TinyLanguage.exe 00277.recursive_eval.tlg output.txt
) else (
    TinyLanguage.exe 00277.recursive_eval.tlg %2
)
