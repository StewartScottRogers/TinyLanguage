@echo off
echo Running 00042.increment_decrement.tlg
if "%2"=="" (
    TinyLanguage.exe 00042.increment_decrement.tlg output.txt
) else (
    TinyLanguage.exe 00042.increment_decrement.tlg %2
)
