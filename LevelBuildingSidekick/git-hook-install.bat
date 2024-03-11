@echo off
echo. > .git\hooks\post-receive
echo #!/bin/bash >> .git\hooks\post-receive
echo GIT_WORK_TREE="%CD%" git -C "$GIT_WORK_TREE" pull origin master >> .git\hooks\post-receive