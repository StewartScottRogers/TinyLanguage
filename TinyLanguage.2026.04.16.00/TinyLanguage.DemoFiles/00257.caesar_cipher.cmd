@echo off
echo Running 00257.caesar_cipher.tlg
if "%2"=="" (
    TinyLanguage.exe 00257.caesar_cipher.tlg output.txt
) else (
    TinyLanguage.exe 00257.caesar_cipher.tlg %2
)
