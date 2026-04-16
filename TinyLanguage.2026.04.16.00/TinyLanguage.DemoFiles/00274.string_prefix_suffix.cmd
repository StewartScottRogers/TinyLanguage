@echo off
echo Running 00274.string_prefix_suffix.tlg
if "%2"=="" (
    TinyLanguage.exe 00274.string_prefix_suffix.tlg output.txt
) else (
    TinyLanguage.exe 00274.string_prefix_suffix.tlg %2
)
