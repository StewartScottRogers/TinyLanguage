@echo off
echo Running 00213.annotated_function.tlg
if "%2"=="" (
    TinyLanguage.exe 00213.annotated_function.tlg output.txt
) else (
    TinyLanguage.exe 00213.annotated_function.tlg %2
)
