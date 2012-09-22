using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PsTest
{
    /// <summary>
    /// Formats a test result.
    /// </summary>
    [Cmdlet(VerbsCommon.Format, "TestResult")]
    public class FormatTestResultCmdlet : PSCmdlet
    {
        /// <summary>
        /// The default color to use for successful tests.
        /// </summary>
        public const string DefaultSuccessColor = "LawnGreen";

        /// <summary>
        /// The default color to use for failed tests.
        /// </summary>
        public const string DefaultFailureColor = "Salmon";

        /// <summary>
        /// Initializes a new cmdlet with a default color formatter.
        /// </summary>
        public FormatTestResultCmdlet()
        {
            ColorFormatter = new PowerShellIseColorFormatter(
                new RunspaceWrapper(Runspace.DefaultRunspace)
            );
            SuccessColor = DefaultSuccessColor;
            FailureColor = DefaultFailureColor;
        }

        /// <summary>
        /// Get or set the test results to format.
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true
        )]
        public TestResult[] TestResult { get; set; }

        /// <summary>
        /// Get or set the success color.
        /// </summary>
        [Parameter(Position = 1)]
        public virtual string SuccessColor { get; set; }

        /// <summary>
        /// Get or set the failure color.
        /// </summary>
        [Parameter(Position = 2)]
        public virtual string FailureColor { get; set; }

        /// <summary>
        /// Get or set the color formatter.
        /// </summary>
        [Parameter()]
        public IColorFormatter ColorFormatter { get; set; }

        /// <summary>
        /// Stores the original background color.
        /// </summary>
        protected override void BeginProcessing()
        {
            OriginalBackGroundColor = ColorFormatter.GetBackgroundColor();
        }

        /// <summary>
        /// Get or set the original background color of the PowerShell host.
        /// </summary>
        internal virtual string OriginalBackGroundColor { get; set; }

        /// <summary>
        /// Formats each test result.
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var testResult in TestResult)
            {
                WriteTestResult(testResult);
            }
        }

        /// <summary>
        /// Sets the background color and writes the test name.
        /// </summary>
        internal virtual void WriteTestResult(TestResult testResult)
        {
            ColorFormatter.SetBackgroundColor(SuccessColor);
            WriteObject(string.Format("{0,-80}", testResult.TestName));
        }

        /// <summary>
        /// Sets the background color back to the original and writes results.
        /// </summary>
        protected override void EndProcessing()
        {
            ColorFormatter.SetBackgroundColor(OriginalBackGroundColor);
        }
    }
}
