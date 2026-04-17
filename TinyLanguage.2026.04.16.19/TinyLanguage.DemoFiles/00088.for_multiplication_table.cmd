@echo off
echo Running 00088.for_multiplication_table.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000088.for_multiplication_table.tlg output.txt
) else (
    TinyLanguage.exe %~dp000088.for_multiplication_table.tlg %2
)
