@echo off
echo Running 00210.do_block.tlg
if "%2"=="" (
    TinyLanguage.exe 00210.do_block.tlg output.txt
) else (
    TinyLanguage.exe 00210.do_block.tlg %2
)
