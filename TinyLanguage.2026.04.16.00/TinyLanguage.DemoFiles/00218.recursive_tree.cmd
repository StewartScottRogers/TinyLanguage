@echo off
echo Running 00218.recursive_tree.tlg
if "%2"=="" (
    TinyLanguage.exe 00218.recursive_tree.tlg output.txt
) else (
    TinyLanguage.exe 00218.recursive_tree.tlg %2
)
