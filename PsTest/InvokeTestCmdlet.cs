using System;
using System.Management.Automation;

namespace PsTest
{
    /// <summary>
    /// Invokes a specified PsTest test.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "Test")]
    public class InvokeTestCmdlet : Cmdlet
    {
        /// <summary>
        /// Get or set the test to invoke.
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true
        )]
        public Test[] Test { get; set; }

        /// <summary>
        /// Creates a new test result.
        /// </summary>
        internal virtual TestResult CreateTestResult(
            string testName,
            bool success
        )
        {
            return new TestResult(testName, success);
        }

        /// <summary>
        /// Invokes a PsTest.
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var test in Test)
            {
                try
                {
                    test.TestScript.Invoke(null);
                    WriteObject(CreateTestResult(test.Name, true));
                }
                catch
                {
                    WriteObject(CreateTestResult(test.Name, false));
                }
            }
        }
    }
}
