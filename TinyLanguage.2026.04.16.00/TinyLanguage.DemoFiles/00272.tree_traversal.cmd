@echo off
echo Running 00272.tree_traversal.tlg
if "%2"=="" (
    TinyLanguage.exe 00272.tree_traversal.tlg output.txt
) else (
    TinyLanguage.exe 00272.tree_traversal.tlg %2
)
