@echo off
echo Running 00068.if_multiline_body.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000068.if_multiline_body.tlg output.txt
) else (
    TinyLanguage.exe %~dp000068.if_multiline_body.tlg %2
)
