@echo off
echo Running 00133.while_with_break.tlg
if "%2"=="" (
    TinyLanguage.exe 00133.while_with_break.tlg output.txt
) else (
    TinyLanguage.exe 00133.while_with_break.tlg %2
)
