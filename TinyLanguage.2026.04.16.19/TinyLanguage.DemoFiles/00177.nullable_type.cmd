@echo off
echo Running 00177.nullable_type.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000177.nullable_type.tlg output.txt
) else (
    TinyLanguage.exe %~dp000177.nullable_type.tlg %2
)
