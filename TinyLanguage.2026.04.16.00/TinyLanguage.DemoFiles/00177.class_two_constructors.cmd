@echo off
echo Running 00177.class_two_constructors.tlg
if "%2"=="" (
    TinyLanguage.exe 00177.class_two_constructors.tlg output.txt
) else (
    TinyLanguage.exe 00177.class_two_constructors.tlg %2
)
