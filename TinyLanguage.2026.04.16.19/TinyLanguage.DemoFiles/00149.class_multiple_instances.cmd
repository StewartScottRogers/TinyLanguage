@echo off
echo Running 00149.class_multiple_instances.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000149.class_multiple_instances.tlg output.txt
) else (
    TinyLanguage.exe %~dp000149.class_multiple_instances.tlg %2
)
