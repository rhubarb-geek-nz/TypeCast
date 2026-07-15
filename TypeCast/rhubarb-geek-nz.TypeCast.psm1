# Copyright (c) 2026 Roger Brown.
# Licensed under the MIT License.

if ($IsCoreCLR)
{
	$assy = Add-Type -LiteralPath "$PSScriptRoot\netstandard2.1\RhubarbGeekNz.TypeCast.dll" -PassThru
}
else
{
	$assy = Add-Type -LiteralPath "$PSScriptRoot\net461\RhubarbGeekNz.TypeCast.dll" -PassThru
}

Import-Module -Assembly $assy.Assembly
