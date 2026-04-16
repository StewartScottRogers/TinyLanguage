@echo off
echo Running 00085.comment_multi_style.tlg
if "%2"=="" (
    TinyLanguage.exe 00085.comment_multi_style.tlg output.txt
) else (
    TinyLanguage.exe 00085.comment_multi_style.tlg %2
)
