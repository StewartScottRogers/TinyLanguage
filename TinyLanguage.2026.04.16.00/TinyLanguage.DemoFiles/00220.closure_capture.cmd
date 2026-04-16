@echo off
echo Running 00220.closure_capture.tlg
if "%2"=="" (
    TinyLanguage.exe 00220.closure_capture.tlg output.txt
) else (
    TinyLanguage.exe 00220.closure_capture.tlg %2
)
