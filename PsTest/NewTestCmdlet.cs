using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace PsTest
{
    /// <summary>
    /// Creates a new unit test.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Test")]
    public class NewTestCmdlet : Cmdlet
    {
        /// <summary>
        /// Get or set the name of the unit test.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        /// <summary>
        /// Get or set the unit test's script block.
        /// </summary>
        [Parameter(Mandatory = true)]
        public ScriptBlock TestScript { get; set; }

        /// <summary>
        /// Processes the parameters.
        /// </summary>
        protected override void ProcessRecord()
        {
            WriteObject(new Test(Name, TestScript));
        }
    }
}
