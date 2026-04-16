@echo off
echo Running 00279.string_builder.tlg
if "%2"=="" (
    TinyLanguage.exe 00279.string_builder.tlg output.txt
) else (
    TinyLanguage.exe 00279.string_builder.tlg %2
)
