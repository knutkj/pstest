using System;
using System.Management.Automation.Runspaces;

namespace PsTest
{
    /// <summary>
    /// Represents the runspace that is the operating environment for command
    /// pipelines. This class provides methods for opening the runspace,
    /// creating single and nested pipelines for the runspace, and closing the
    /// runspace.
    /// </summary>
    public class RunspaceWrapper : IRunspace
    {
        private readonly Runspace _runspace;

        /// <summary>
        /// Initializes a new instance of a runspace wrapper wrapping the
        /// specified runspace.
        /// </summary>
        /// <param name="runspace">
        /// The runspace to wrap.
        /// </param>
        public RunspaceWrapper(Runspace runspace)
        {
            if (runspace == null)
            {
                throw new ArgumentNullException("runspace");
            }
            _runspace = runspace;
        }

        /// <summary>
        /// Get the wrapped runspace.
        /// </summary>
        internal Runspace Runspace { get { return _runspace; } }

        /// <summary>
        /// Creates a pipeline for the runspace while an existing pipeline is
        /// executing. This method also specifies the commands (such as cmdlets
        /// and scripts) that can be executed by the pipeline and specifies a
        /// Boolean value that indicates that pipeline execution is added to
        /// the history of the runspace. 
        /// </summary>
        /// <param name="command">
        /// Cmdlets, scripts, native applications, executables, or files
        /// available to the runspace through the pipeline.
        /// </param>
        /// <param name="addToHistory">
        /// <c>true</c> indicates that pipeline execution is added to the
        /// history of the runspace; otherwise false is returned.
        /// </param>
        /// <returns>
        /// A Pipeline object that defines and manages a pipeline within the
        /// runspace. Note that the methods provided by the pipeline are used
        /// by the runspace to execute commands. 
        /// </returns>
        public IPipeline CreateNestedPipeline(
            string command, 
            bool addToHistory
        )
        {
            return new PipelineWrapper(Runspace.CreateNestedPipeline(
                command, 
                addToHistory
            ));
        }
    }
}
