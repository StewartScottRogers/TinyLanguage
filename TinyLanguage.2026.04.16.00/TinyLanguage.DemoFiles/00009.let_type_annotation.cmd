@echo off
echo Running 00009.let_type_annotation.tlg
if "%2"=="" (
    TinyLanguage.exe 00009.let_type_annotation.tlg output.txt
) else (
    TinyLanguage.exe 00009.let_type_annotation.tlg %2
)
