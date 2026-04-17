@echo off
echo Running 00175.annotation_with_params2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000175.annotation_with_params2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000175.annotation_with_params2.tlg %2
)
