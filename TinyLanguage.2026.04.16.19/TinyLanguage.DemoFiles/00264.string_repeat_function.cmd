@echo off
echo Running 00264.string_repeat_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000264.string_repeat_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000264.string_repeat_function.tlg %2
)
