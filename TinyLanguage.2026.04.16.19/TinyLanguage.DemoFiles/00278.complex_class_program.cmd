@echo off
echo Running 00278.complex_class_program.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000278.complex_class_program.tlg output.txt
) else (
    TinyLanguage.exe %~dp000278.complex_class_program.tlg %2
)
