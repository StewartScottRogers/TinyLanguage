@echo off
echo Running 00117.class_implements.tlg
if "%2"=="" (
    TinyLanguage.exe 00117.class_implements.tlg output.txt
) else (
    TinyLanguage.exe 00117.class_implements.tlg %2
)
