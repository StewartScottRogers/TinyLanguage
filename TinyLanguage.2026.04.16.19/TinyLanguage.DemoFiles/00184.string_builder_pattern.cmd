@echo off
echo Running 00184.string_builder_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000184.string_builder_pattern.tlg output.txt
) else (
    TinyLanguage.exe %~dp000184.string_builder_pattern.tlg %2
)
