@echo off
echo Running 00234.annotation_with_param.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000234.annotation_with_param.tlg output.txt
) else (
    TinyLanguage.exe %~dp000234.annotation_with_param.tlg %2
)
