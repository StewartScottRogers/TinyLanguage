@echo off
echo Running 00237.function_composition.tlg
if "%2"=="" (
    TinyLanguage.exe 00237.function_composition.tlg output.txt
) else (
    TinyLanguage.exe 00237.function_composition.tlg %2
)
