@echo off
echo Running 00200.empty_class_body2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000200.empty_class_body2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000200.empty_class_body2.tlg %2
)
