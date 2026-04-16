@echo off
echo Running 00151.bmi_calculator.tlg
if "%2"=="" (
    TinyLanguage.exe 00151.bmi_calculator.tlg output.txt
) else (
    TinyLanguage.exe 00151.bmi_calculator.tlg %2
)
