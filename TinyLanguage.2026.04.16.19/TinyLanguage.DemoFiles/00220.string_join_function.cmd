@echo off
echo Running 00220.string_join_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000220.string_join_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000220.string_join_function.tlg %2
)
