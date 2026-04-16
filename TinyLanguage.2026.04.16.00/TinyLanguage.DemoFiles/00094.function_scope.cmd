@echo off
echo Running 00094.function_scope.tlg
if "%2"=="" (
    TinyLanguage.exe 00094.function_scope.tlg output.txt
) else (
    TinyLanguage.exe 00094.function_scope.tlg %2
)
