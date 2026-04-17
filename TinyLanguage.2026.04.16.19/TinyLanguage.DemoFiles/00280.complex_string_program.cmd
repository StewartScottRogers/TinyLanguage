@echo off
echo Running 00280.complex_string_program.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000280.complex_string_program.tlg output.txt
) else (
    TinyLanguage.exe %~dp000280.complex_string_program.tlg %2
)
