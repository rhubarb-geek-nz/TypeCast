// Copyright (c) 2026 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Management.Automation;
using System.Text;
#if NETSTANDARD
using System.Reflection;
using System.Collections.Generic;
#else
using Microsoft.PowerShell.Commands;
using System.Runtime.InteropServices;
#endif

namespace RhubarbGeekNz.TypeCast
{
    [Cmdlet(VerbsCommon.Get, "SystemTextEncoding", DefaultParameterSetName = "All")]
    [OutputType(typeof(Encoding))]
    sealed public class GetSystemTextEncoding : PSCmdlet
    {
        const string NAME = "Name", CODEPAGE = "CodePage", ENCODING = "Encoding", FILESYS= "FileSystemCmdletProviderEncoding";
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Name", Position = 0, ParameterSetName = NAME)]
        public string Name;
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "CodePage", Position = 0, ParameterSetName = CODEPAGE)]
        public int CodePage;
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Encoding", Position = 0, ParameterSetName = ENCODING)]
        public Encoding Encoding;
#if NETSTANDARD
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "FileSystemCmdletProviderEncoding", ParameterSetName = FILESYS)]
        public string FileSystemCmdletProviderEncoding;
        readonly static IDictionary<string, Encoding> encodingMap; 
        static GetSystemTextEncoding()
        {
            var typeClass = typeof(PSCmdlet).Assembly.GetType("System.Management.Automation.EncodingConversion");
            if (typeClass != null)
            {
                var fieldInfo = typeClass.GetField("encodingMap", BindingFlags.NonPublic | BindingFlags.Static);

                if (fieldInfo != null)
                {
                    encodingMap = (IDictionary<string, Encoding>)fieldInfo.GetValue(null);
                }
            }
        }
#else
        [DllImport("kernel32.dll")]
        extern static UInt16 GetOEMCP();
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "FileSystemCmdletProviderEncoding", Position = 0, ParameterSetName = FILESYS)]
        public FileSystemCmdletProviderEncoding FileSystemCmdletProviderEncoding;
        private readonly static Encoding[] encodingList = new Encoding[] {
                    Console.OutputEncoding,
                    Encoding.Unicode,
                    Encoding.Unicode,
                    Encoding.GetEncoding("iso-8859-1"),
                    new UnicodeEncoding(true,true),
                    Encoding.UTF8,
                    Encoding.UTF7,
                    Encoding.UTF32,
                    Encoding.ASCII,
                    Encoding.Default,
                    Encoding.GetEncoding(GetOEMCP()),
                    new UTF32Encoding(true,true)
                };
#endif

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case NAME:
                    WriteObject(Encoding.GetEncoding(Name));
                    break;
                case CODEPAGE:
                    WriteObject(Encoding.GetEncoding(CodePage));
                    break;
                case ENCODING:
                    WriteObject(Encoding);
                    break;
                case FILESYS:
#if NETSTANDARD
                    WriteObject(encodingMap[FileSystemCmdletProviderEncoding]);
#else
                    WriteObject(encodingList[(int)FileSystemCmdletProviderEncoding]);
#endif
                    break;
                default:
                    foreach (var e in Encoding.GetEncodings())
                    {
                        WriteObject(e.GetEncoding());
                    }
                    break;
            }
        }
    }
}
