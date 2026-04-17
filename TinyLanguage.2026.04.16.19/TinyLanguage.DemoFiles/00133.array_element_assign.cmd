@echo off
echo Running 00133.array_element_assign.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000133.array_element_assign.tlg output.txt
) else (
    TinyLanguage.exe %~dp000133.array_element_assign.tlg %2
)
