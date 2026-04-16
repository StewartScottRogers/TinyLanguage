@echo off
echo Running 00170.fibonacci_memo.tlg
if "%2"=="" (
    TinyLanguage.exe 00170.fibonacci_memo.tlg output.txt
) else (
    TinyLanguage.exe 00170.fibonacci_memo.tlg %2
)
