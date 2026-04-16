@echo off
echo Running 00045.compound_assign_mul.tlg
if "%2"=="" (
    TinyLanguage.exe 00045.compound_assign_mul.tlg output.txt
) else (
    TinyLanguage.exe 00045.compound_assign_mul.tlg %2
)
