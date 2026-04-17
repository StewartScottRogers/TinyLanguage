@echo off
echo Running 00286.comprehension_evens.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000286.comprehension_evens.tlg output.txt
) else (
    TinyLanguage.exe %~dp000286.comprehension_evens.tlg %2
)
