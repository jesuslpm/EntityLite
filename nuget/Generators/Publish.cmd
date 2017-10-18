%~dp0..\NuGet.exe Update -self
%~dp0..\NuGet.exe Push EntityLite.2.0.0-beta2.nupkg -source https://www.nuget.org
pause