@echo off
echo Running 00080.as_type_assert.tlg
if "%2"=="" (
    TinyLanguage.exe 00080.as_type_assert.tlg output.txt
) else (
    TinyLanguage.exe 00080.as_type_assert.tlg %2
)
