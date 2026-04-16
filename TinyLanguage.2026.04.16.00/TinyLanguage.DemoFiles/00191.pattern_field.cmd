@echo off
echo Running 00191.pattern_field.tlg
if "%2"=="" (
    TinyLanguage.exe 00191.pattern_field.tlg output.txt
) else (
    TinyLanguage.exe 00191.pattern_field.tlg %2
)
