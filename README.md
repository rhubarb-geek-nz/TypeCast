# rhubarb-geek-nz.TypeCast
SystemTextEncoding for PowerShell

```
Get-SystemTextEncoding [<CommonParameters>]

Get-SystemTextEncoding [-Name] <string> [<CommonParameters>]

Get-SystemTextEncoding [-CodePage] <int> [<CommonParameters>]

Get-SystemTextEncoding [-Encoding] <Encoding> [<CommonParameters>]
```

Uses [System.Text.Encoding.GetEncoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.getencoding) for lookup by name or code page.

Example

```
PS> Get-SystemTextEncoding 850

IsSingleByte      : True
BodyName          : ibm850
EncodingName      : Western European (DOS)
HeaderName        : ibm850
WebName           : ibm850
WindowsCodePage   : 1252
IsBrowserDisplay  : False
IsBrowserSave     : False
IsMailNewsDisplay : False
IsMailNewsSave    : False
EncoderFallback   : System.Text.InternalEncoderBestFitFallback
DecoderFallback   : System.Text.InternalDecoderBestFitFallback
IsReadOnly        : True
CodePage          : 850
```
