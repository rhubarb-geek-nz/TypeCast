#!/usr/bin/env pwsh
# Copyright (c) 2026 Roger Brown.
# Licensed under the MIT License.

param([string]$PublishDir = 'publish')

$ErrorActionPreference = 'Stop'

trap
{
	throw $PSItem
}

$DSC = [System.IO.Path]::DirectorySeparatorChar

$moduleDir = $env:PSModulePath.Split([System.IO.Path]::PathSeparator)[0]

Write-Output $moduleDir
Write-Output $PublishDir

$PSD = (Get-ChildItem -LiteralPath $PublishDir -Filter '*.psd1')

Write-Output $PSD.FullName

$ModuleId = $PSD.BaseName

Write-Output $ModuleId

$manifest = Import-PowerShellDataFile -LiteralPath $PSD.FullName

$ModuleVersion = $manifest.ModuleVersion

Write-Output $ModuleVersion

$moduleBaseDir =  "$moduleDir${DSC}$ModuleId"

if ( Test-Path $moduleBaseDir -Type Container )
{
	Write-Output "remove $moduleBaseDir"

	Remove-Item -LiteralPath $moduleBaseDir -Force -Recurse
}

$null = New-Item -Path $moduleDir -Name $ModuleId -Type Directory

$destination = "$moduleBaseDir${DSC}$ModuleVersion"

Copy-Item -LiteralPath $PublishDir -Destination $destination -Recurse

$PSD = (Get-ChildItem -LiteralPath $destination -Filter '*.psd1')

Write-Output $PSD.FullName
