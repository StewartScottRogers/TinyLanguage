@echo off
echo Running 00051.array_element_assign.tlg
if "%2"=="" (
    TinyLanguage.exe 00051.array_element_assign.tlg output.txt
) else (
    TinyLanguage.exe 00051.array_element_assign.tlg %2
)
