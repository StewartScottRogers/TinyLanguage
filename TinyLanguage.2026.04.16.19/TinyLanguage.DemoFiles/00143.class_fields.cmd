@echo off
echo Running 00143.class_fields.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000143.class_fields.tlg output.txt
) else (
    TinyLanguage.exe %~dp000143.class_fields.tlg %2
)
