@echo off
echo Running 00285.class_builder_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000285.class_builder_pattern.tlg output.txt
) else (
    TinyLanguage.exe %~dp000285.class_builder_pattern.tlg %2
)
