@echo off
echo Running 00235.annotation_multiple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000235.annotation_multiple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000235.annotation_multiple.tlg %2
)
