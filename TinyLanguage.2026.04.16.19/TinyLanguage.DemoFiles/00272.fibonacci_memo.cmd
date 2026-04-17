@echo off
echo Running 00272.fibonacci_memo.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000272.fibonacci_memo.tlg output.txt
) else (
    TinyLanguage.exe %~dp000272.fibonacci_memo.tlg %2
)
