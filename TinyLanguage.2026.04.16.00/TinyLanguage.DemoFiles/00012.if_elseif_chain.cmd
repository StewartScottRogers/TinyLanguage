@echo off
echo Running 00012.if_elseif_chain.tlg
if "%2"=="" (
    TinyLanguage.exe 00012.if_elseif_chain.tlg output.txt
) else (
    TinyLanguage.exe 00012.if_elseif_chain.tlg %2
)
