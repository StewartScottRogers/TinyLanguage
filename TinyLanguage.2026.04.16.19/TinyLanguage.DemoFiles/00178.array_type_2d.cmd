@echo off
echo Running 00178.array_type_2d.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000178.array_type_2d.tlg output.txt
) else (
    TinyLanguage.exe %~dp000178.array_type_2d.tlg %2
)
