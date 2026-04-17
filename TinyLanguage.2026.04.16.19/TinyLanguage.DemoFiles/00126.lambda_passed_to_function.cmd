@echo off
echo Running 00126.lambda_passed_to_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000126.lambda_passed_to_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000126.lambda_passed_to_function.tlg %2
)
