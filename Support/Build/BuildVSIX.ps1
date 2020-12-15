Param (
    [string]$XpandFolder=(Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild.exe",
    [string]$DXVersion="0.0.0.0",
    [string]$Source="$(Get-PackageFeed -Nuget);$env:DxFeed",
    [bool]$Release=$true
)
$ErrorActionPreference = "Stop"
$psource=Get-PackageFeed -Nuget
if (!$Release){
    $psource=Get-PackageFeed -Xpand
}

Import-Module XpandPwsh -Force -Prefix X

if ($DXVersion -eq "0.0.0.0"){
    $DXVersion=Get-AssemblyInfoVersion "$PSScriptRoot\..\..\Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"
}
Set-Location $XpandFolder\Xpand.Plugins
$result=Get-NugetPackage Xpand.XAF.ModelEditor -ResultType DownloadResults -Source $psource
Copy-Item "$((Get-Item $result.PackageReader.GetNuspecFile()).DirectoryName)\build\Xpand.XAF.ModelEditor.exe" "$XpandFolder\Xpand.dll" -Force



#build VSIX
$fileName="$XpandFolder\Xpand.Plugins\Xpand.VSIX\Xpand.VSIX.csproj"
& "$(Get-XNugetPath)" Restore $fileName -PackagesDirectory "$XpandFolder\Support\_third_party_assemblies\Packages" -source $Source 
& "$msbuild" "$fileName" "/p:Configuration=Release;DeployExtension=false;OutputPath=$XpandFolder\Xpand.Dll\Plugins" /v:m 




