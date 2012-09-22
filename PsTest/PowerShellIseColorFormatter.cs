using System;

namespace PsTest
{
    /// <summary>
    /// Represents a color formatter which formats colors
    /// for a PowerShell ISE host.
    /// </summary>
    public class PowerShellIseColorFormatter : IColorFormatter
    {
        private readonly IRunspace _runspace;

        /// <summary>
        /// Initializes a new instance of the PowerShell ISE color formatter
        /// with the specified runtime.
        /// </summary>
        /// <param name="runspace">
        /// The runtime to use for formatting instructions.
        /// </param>
        public PowerShellIseColorFormatter(IRunspace runspace)
        {
            if (runspace == null)
            {
                throw new ArgumentNullException("runspace");
            }
            _runspace = runspace;
        }

        /// <summary>
        /// Get the runspace to use for formatting instructions.
        /// </summary>
        internal IRunspace Runspace { get { return _runspace; } }

        /// <summary>
        /// Sets the background color to the specified color.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        public void SetBackgroundColor(string color)
        {
            var command = string.Format(
                "$psISE.Options.OutputPaneTextBackgroundColor = '{0}'",
                color
            );
            Runspace.CreateNestedPipeline(command, false).Invoke();
        }

        /// <summary>
        /// Get the current background color.
        /// </summary>
        /// <returns>
        /// The background color.
        /// </returns>
        public string GetBackgroundColor()
        {
            var command = 
                "[string]$psISE.Options.OutputPaneTextBackgroundColor";
            return (string)Runspace
                .CreateNestedPipeline(command, false).Invoke()[0]
                .BaseObject;
        }
    }
}
