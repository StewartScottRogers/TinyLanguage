@echo off
echo Running 00214.annotated_class.tlg
if "%2"=="" (
    TinyLanguage.exe 00214.annotated_class.tlg output.txt
) else (
    TinyLanguage.exe 00214.annotated_class.tlg %2
)
