@echo off
echo Running 00175.annotation_with_params.tlg
if "%2"=="" (
    TinyLanguage.exe 00175.annotation_with_params.tlg output.txt
) else (
    TinyLanguage.exe 00175.annotation_with_params.tlg %2
)
