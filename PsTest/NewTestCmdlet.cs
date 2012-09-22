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
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Name { get; set; }

        /// <summary>
        /// Get or set the unit test's script block.
        /// </summary>
        [Parameter(
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
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
