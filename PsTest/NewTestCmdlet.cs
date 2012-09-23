using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace PsTest
{
    /// <summary>
    /// Creates a new unit test.
    /// </summary>
    [Cmdlet(
        VerbsCommon.New, 
        "Test", 
        DefaultParameterSetName = DefaultParameterSetName
    )]
    public class NewTestCmdlet : Cmdlet
    {
        internal const string DefaultParameterSetName = "Default";
        internal const string ExceptionParameterSetName = "Exception";

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
        /// Get or set the expected exception.
        /// </summary>
        [Parameter(
            ParameterSetName = ExceptionParameterSetName,
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public Type ExpectedException { get; set; }

        /// <summary>
        /// Get or set the unit test's script block.
        /// </summary>
        [Parameter(
            ParameterSetName = DefaultParameterSetName,
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        [Parameter(
            ParameterSetName = ExceptionParameterSetName,
            Position = 2,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public ScriptBlock TestScript { get; set; }

        /// <summary>
        /// Processes the parameters.
        /// </summary>
        protected override void ProcessRecord()
        {
            WriteObject(CreateTest());
        }

        /// <summary>
        /// Creates a new test based on the specified parameters.
        /// </summary>
        internal virtual Test CreateTest()
        {
            if (ExpectedException != null)
            {
                return new Test(Name, TestScript, ExpectedException);
            }
            return new Test(Name, TestScript);
        }
    }
}
