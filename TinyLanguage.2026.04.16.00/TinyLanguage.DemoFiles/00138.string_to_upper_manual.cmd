@echo off
echo Running 00138.string_to_upper_manual.tlg
if "%2"=="" (
    TinyLanguage.exe 00138.string_to_upper_manual.tlg output.txt
) else (
    TinyLanguage.exe 00138.string_to_upper_manual.tlg %2
)
