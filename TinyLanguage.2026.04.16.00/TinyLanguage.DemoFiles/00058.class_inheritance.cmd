@echo off
echo Running 00058.class_inheritance.tlg
if "%2"=="" (
    TinyLanguage.exe 00058.class_inheritance.tlg output.txt
) else (
    TinyLanguage.exe 00058.class_inheritance.tlg %2
)
