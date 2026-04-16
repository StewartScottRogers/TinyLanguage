@echo off
echo Running 00123.closure_like.tlg
if "%2"=="" (
    TinyLanguage.exe 00123.closure_like.tlg output.txt
) else (
    TinyLanguage.exe 00123.closure_like.tlg %2
)
