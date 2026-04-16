@echo off
echo Running 00057.class_with_methods.tlg
if "%2"=="" (
    TinyLanguage.exe 00057.class_with_methods.tlg output.txt
) else (
    TinyLanguage.exe 00057.class_with_methods.tlg %2
)
