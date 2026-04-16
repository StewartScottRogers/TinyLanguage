@echo off
echo Running 00149.number_guessing_logic.tlg
if "%2"=="" (
    TinyLanguage.exe 00149.number_guessing_logic.tlg output.txt
) else (
    TinyLanguage.exe 00149.number_guessing_logic.tlg %2
)
