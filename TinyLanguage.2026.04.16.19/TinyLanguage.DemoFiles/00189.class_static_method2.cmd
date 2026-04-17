@echo off
echo Running 00189.class_static_method2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000189.class_static_method2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000189.class_static_method2.tlg %2
)
