@echo off
set ruby_path=%merlin_root%\..\External.LCA_RESTRICTED\Languages\Ruby\ruby-1.8.6p368\bin
set ruby=%ruby_path%\ruby.exe
%ruby% %ruby_path%\rake %*
