using Microsoft.VisualStudio.TestTools.UnitTesting;
using PsTest;
using System;
using System.Management.Automation.Runspaces;

namespace PsTestTests
{
    [TestClass]
    public class PipelineWrapperTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoPipelineArgumentNullException()
        {
            // Arrange.
            Pipeline pipeline = null;

            // Act.

            // Assert.
            new PipelineWrapper(pipeline);
        }

        [TestMethod]
        public void CtorSavesPipelineReference()
        {
            // Arrange.
            var expectedPipeline = RunspaceFactory
                .CreateRunspace()
                .CreatePipeline();

            // Act.
            var wrapper = new PipelineWrapper(expectedPipeline);

            // Assert.
            Assert.AreEqual(expectedPipeline, wrapper.Pipeline);
        }

        [TestMethod]
        public void InvokeDelegatesToWrappedPipeline()
        {
            // Arrange.
            var runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            var expectedPipeline = runspace.CreatePipeline("1+1");
            var wrapper = new PipelineWrapper(expectedPipeline);

            // Act.
            var res = wrapper.Invoke();

            // Assert.
            Assert.AreEqual(2, (int)res[0].BaseObject);
        }
    }
}
