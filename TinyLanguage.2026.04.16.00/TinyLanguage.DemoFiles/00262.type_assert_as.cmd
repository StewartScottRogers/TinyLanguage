@echo off
echo Running 00262.type_assert_as.tlg
if "%2"=="" (
    TinyLanguage.exe 00262.type_assert_as.tlg output.txt
) else (
    TinyLanguage.exe 00262.type_assert_as.tlg %2
)
