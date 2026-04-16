@echo off
echo Running 00227.multiline_strings.tlg
if "%2"=="" (
    TinyLanguage.exe 00227.multiline_strings.tlg output.txt
) else (
    TinyLanguage.exe 00227.multiline_strings.tlg %2
)
