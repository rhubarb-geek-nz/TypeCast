// Copyright (c) 2026 Roger Brown.
// Licensed under the MIT License.

#if NETCOREAPP
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;

namespace RhubarbGeekNz.TypeCast
{
    [TestClass]
    public class UnitTests
    {
        readonly InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        public UnitTests()
        {
            foreach (Type t in new Type[] {
                typeof(GetSystemTextEncoding)
            })
            {
                CmdletAttribute ca = t.GetCustomAttribute<CmdletAttribute>();

                if (ca == null) throw new NullReferenceException();

                initialSessionState.Commands.Add(new SessionStateCmdletEntry($"{ca.VerbName}-{ca.NounName}", t, ca.HelpUri));
            }

            initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Stop action"));
        }

        [TestMethod]
        public void TestASCII()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Get-SystemTextEncoding ([System.Text.Encoding]::ASCII)");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual("us-ascii", ((Encoding)outputPipeline[0].BaseObject).WebName);
            }
        }

        [TestMethod]
        public void TestUTF8()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Get-SystemTextEncoding 'utf-8'");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual("utf-8", ((Encoding)outputPipeline[0].BaseObject).WebName);
            }
        }

        [TestMethod]
        public void Test850()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Get-SystemTextEncoding 850");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual("ibm850", ((Encoding)outputPipeline[0].BaseObject).WebName);
            }
        }

        [TestMethod]
        public void TestEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                int count = Encoding.GetEncodings().Length;
                powerShell.AddScript("Get-SystemTextEncoding");
                var outputPipeline = powerShell.Invoke();
                Assert.AreEqual(count, outputPipeline.Count);

                foreach (var o in outputPipeline)
                {
                    Encoding e = o.BaseObject as Encoding;
                    Assert.IsNotNull(e);
                }
            }
        }

        [TestMethod]
        public void TestFileSysString()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Get-SystemTextEncoding -FileSystemCmdletProviderEncoding 'ascii'");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual("us-ascii", ((Encoding)outputPipeline[0].BaseObject).WebName);
            }
        }

#if NETCOREAPP
#else
        [TestMethod]
        public void TestFileSysName()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript(
                    "$value = [Microsoft.PowerShell.Commands.FileSystemCmdletProviderEncoding]::ASCII\r\n"+
                    "Get-SystemTextEncoding $value");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual("us-ascii", ((Encoding)outputPipeline[0].BaseObject).WebName);
            }
        }
        [TestMethod]
        public void TestFileSysOrdinal()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript(
                    "$value = [Microsoft.PowerShell.Commands.FileSystemCmdletProviderEncoding]4\r\n" +
                    "Get-SystemTextEncoding $value");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual("utf-16BE", ((Encoding)outputPipeline[0].BaseObject).WebName);
            }
        }
#endif
    }
}
