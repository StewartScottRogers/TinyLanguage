@echo off
echo Running 00263.string_trim_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000263.string_trim_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000263.string_trim_function.tlg %2
)
