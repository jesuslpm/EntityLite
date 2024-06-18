%~dp0..\NuGet.exe Update -self
%~dp0..\NuGet.exe Push EntityLite.MassTransit.1.0.0-beta1.nupkg -source https://www.nuget.org
pause