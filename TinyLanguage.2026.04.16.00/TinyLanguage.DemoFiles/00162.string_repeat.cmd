@echo off
echo Running 00162.string_repeat.tlg
if "%2"=="" (
    TinyLanguage.exe 00162.string_repeat.tlg output.txt
) else (
    TinyLanguage.exe 00162.string_repeat.tlg %2
)
