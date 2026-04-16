@echo off
echo Running 00269.template_method.tlg
if "%2"=="" (
    TinyLanguage.exe 00269.template_method.tlg output.txt
) else (
    TinyLanguage.exe 00269.template_method.tlg %2
)
