using System;
using System.Collections.ObjectModel;
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
                    InvokeScriptBlock(test.TestScript);
                    WriteObject(CreateTestResult(test.Name, true));
                }
                catch (Exception e)
                {
                    WriteObject(CreateTestResult(
                        test.Name, 
                        IsExpectedException(test.ExpectedException, e)
                    ));
                }
            }
        }

        /// <summary>
        /// Returns true if the exception is expected.
        /// </summary>
        private static bool IsExpectedException(
            Type expectedExceptionType,
            Exception actualException
        )
        {
            return expectedExceptionType != null &&
                actualException.InnerException != null &&
                actualException
                    .InnerException
                    .GetType()
                    .Equals(expectedExceptionType);
        }

        /// <summary>
        /// Invokes the specified script block.
        /// </summary>
        internal virtual Collection<PSObject> InvokeScriptBlock(
            ScriptBlock scriptBlock
        )
        {
            return scriptBlock.Invoke(null);
        }
    }
}
