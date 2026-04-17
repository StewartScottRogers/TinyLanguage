@echo off
echo Running 00253.type_cast_int.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000253.type_cast_int.tlg output.txt
) else (
    TinyLanguage.exe %~dp000253.type_cast_int.tlg %2
)
