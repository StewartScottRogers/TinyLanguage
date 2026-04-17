@echo off
echo Running 00249.caesar_cipher.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000249.caesar_cipher.tlg output.txt
) else (
    TinyLanguage.exe %~dp000249.caesar_cipher.tlg %2
)
