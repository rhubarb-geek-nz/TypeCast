// Copyright (c) 2026 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Management.Automation;
using System.Text;

namespace RhubarbGeekNz.TypeCast
{
    [Cmdlet(VerbsCommon.Get, "SystemTextEncoding", DefaultParameterSetName = "All")]
    [OutputType(typeof(Encoding))]
    sealed public class GetSystemTextEncoding : PSCmdlet
    {
        const string NAME = "Name", CODEPAGE = "CodePage", ENCODING = "Encoding";
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Name", Position = 0, ParameterSetName = NAME)]
        public string Name;
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "CodePage", Position = 0, ParameterSetName = CODEPAGE)]
        public int CodePage;
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Encoding", Position = 0, ParameterSetName = ENCODING)]
        public Encoding Encoding;

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
