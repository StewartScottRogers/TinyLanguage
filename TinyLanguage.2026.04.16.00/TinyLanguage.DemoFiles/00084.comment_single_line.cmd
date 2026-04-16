@echo off
echo Running 00084.comment_single_line.tlg
if "%2"=="" (
    TinyLanguage.exe 00084.comment_single_line.tlg output.txt
) else (
    TinyLanguage.exe 00084.comment_single_line.tlg %2
)
