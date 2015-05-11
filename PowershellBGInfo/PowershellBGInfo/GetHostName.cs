using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PowershellBGInfo
{
    [Cmdlet(VerbsCommon.Get, "HostName")]
    public class GetHostName : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Environment.MachineName);
        }
    }

    [Cmdlet(VerbsCommon.Get, "ComputerName")]
    public class GetComputerName : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Environment.MachineName);
        }
    }
}
