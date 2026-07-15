#!/usr/bin/env pwsh
# Copyright (c) 2026 Roger Brown.
# Licensed under the MIT License.

param($ProjectName='TypeCast', $IntermediateOutputPath='.', $OutDir='.\', $PublishDir = 'publish\')

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

trap
{
	throw $PSItem
}

if (Test-Path -LiteralPath $PublishDir)
{
	Remove-Item -LiteralPath $PublishDir -Recurse
}

dotnet clean --configuration Release

if ($LastExitCode)
{
	throw "clean $LastExitCode"
}

dotnet build --configuration Release

if ($LastExitCode)
{
	throw "build $LastExitCode"
}

$null = New-Item -Path . -Name publish -ItemType Directory

Copy-Item -LiteralPath ..\README.md -Destination $PublishDir

function Get-SingleNodeValue([System.Xml.XmlDocument]$doc,[string]$path)
{
	return $doc.SelectSingleNode($path).FirstChild.Value
}

$xmlDoc = [System.Xml.XmlDocument](Get-Content "$ProjectName.csproj")

$ModuleId = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/PackageId'
$Version = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/Version'
$ProjectUri = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/PackageProjectUrl'
$Description = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/Description'
$Author = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/Authors'
$Copyright = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/Copyright'
$AssemblyName = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/AssemblyName'
$CompanyName = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/Company'
$TargetFrameworks = Get-SingleNodeValue $xmlDoc '/Project/PropertyGroup/TargetFrameworks'

foreach ($framework in $TargetFrameworks.Split(';'))
{
	$dir = New-Item -Path $PublishDir -Name $framework -ItemType Directory
	Copy-Item -Path "bin\Release\$framework\RhubarbGeekNz.TypeCast.dll" -Destination $dir
}

$moduleSettings = @{
	Path = "$OutDir$ModuleId.psd1"
	RootModule = "$ModuleId.psm1"
	ModuleVersion = $Version
	Guid = '4bf7f4ff-a579-4ccc-9df4-eb385c95cbd9'
	Author = $Author
	CompanyName = $CompanyName
	Copyright = $Copyright
	Description = $Description
	FunctionsToExport = @()
	CmdletsToExport = @('Get-SystemTextEncoding')
	VariablesToExport = '*'
	AliasesToExport = @()
	ProjectUri = $ProjectUri
}

New-ModuleManifest @moduleSettings

Import-PowerShellDataFile -LiteralPath "$OutDir$ModuleId.psd1" | Export-PowerShellDataFile | Set-Content -LiteralPath "$PublishDir$ModuleId.psd1" -Encoding utf8BOM

Remove-Item "$OutDir$ModuleId.psd1"

Copy-Item "$ModuleId.psm1" -Destination $PublishDir
