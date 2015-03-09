SET TOOLS=..\tools
SET ILMERGE=%TOOLS%\ILMerge.exe
SET RELEASE=..\src\Transformer\bin\Release
SET INPUT=%RELEASE%\Transformer.exe
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Text.dll
SET INPUT=%INPUT% %RELEASE%\Transformer.Core.dll
SET INPUT=%INPUT% %RELEASE%\NLog.dll
SET INPUT=%INPUT% %RELEASE%\CommandLine.dll

%ILMERGE% /target:exe /targetplatform:v4,"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:Transformer.exe /ndebug %INPUT%
