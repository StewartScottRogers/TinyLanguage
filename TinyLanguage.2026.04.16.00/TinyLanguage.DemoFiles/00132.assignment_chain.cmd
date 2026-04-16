@echo off
echo Running 00132.assignment_chain.tlg
if "%2"=="" (
    TinyLanguage.exe 00132.assignment_chain.tlg output.txt
) else (
    TinyLanguage.exe 00132.assignment_chain.tlg %2
)
