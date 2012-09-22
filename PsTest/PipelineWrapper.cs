using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PsTest
{
    /// <summary>
    /// Represents the base functionality of a pipeline that can be used to
    /// invoke commands.
    /// </summary>
    /// <remarks>
    /// Pipelines are created within the context of a runspace. Pipelines can
    /// be created using the following methods:
    /// 
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <see cref="IRunspace.CreatePipeline"/>: Overloaded method that can be
    /// used to create a pipeline or to create a pipeline with a valid command
    /// string.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="IRunspace.CreateNestedPipeline"/>: Overloaded method that
    /// can be used to create a nested pipeline or to create a nested pipeline
    /// with a valid command string.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class PipelineWrapper : IPipeline
    {
        private readonly Pipeline _pipeline;

        /// <summary>
        /// Initializes a new instance of pipeline wrapper wrapping the
        /// specified pipeline.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to wrap.
        /// </param>
        public PipelineWrapper(Pipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }
            _pipeline = pipeline;
        }

        /// <summary>
        /// Get the wrapped pipeline.
        /// </summary>
        internal Pipeline Pipeline { get { return _pipeline; } }

        /// <summary>
        /// Invokes the pipeline synchronously. 
        /// </summary>
        /// <remarks>
        /// When this method is used to invoke the pipeline, the Windows
        /// PowerShell runtime closes the Input pipe. 
        /// 
        /// This method cannot be called when another pipeline is running.
        ///  
        /// This method cannot be called multiple times on a given pipeline.
        /// The state of the pipeline must be NotStarted when Invoke is called.
        /// When this method is called, it changes the state of the pipeline to
        /// Running. When Invoke is completed, it changes the state of the
        /// pipeline to one of following:
        /// 
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// Completed: The pipeline state is Completed if the pipeline
        /// invocation completed successfully.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Failed: The pipeline state is Failed if the pipeline invocation
        /// failed or one of the commands in the pipeline threw a terminating
        /// error.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Stopped: The pipeline state is Stopped if the pipeline was stopped
        /// by calling Stop or StopAsync.
        /// </description>
        /// </item>
        /// </list>
        /// 
        /// Applications can get notified about pipeline state changes by
        /// registering for the StateChanged event. This event is raised each
        /// time the state of the pipeline changes.
        /// </remarks>
        public Collection<PSObject> Invoke()
        {
            return Pipeline.Invoke();
        }
    }
}
