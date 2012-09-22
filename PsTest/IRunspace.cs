namespace PsTest
{
    /// <summary>
    /// Represents the runspace that is the operating environment for command
    /// pipelines. This class provides methods for opening the runspace,
    /// creating single and nested pipelines for the runspace, and closing the
    /// runspace.
    /// </summary>
    public interface IRunspace
    {
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
        IPipeline CreateNestedPipeline(string command, bool addToHistory);
    }
}
