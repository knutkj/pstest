using Microsoft.VisualStudio.TestTools.UnitTesting;
using PsTest;
using System;
using System.Management.Automation.Runspaces;

namespace PsTestTests
{
    [TestClass]
    public class RunspaceWrapperTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CtorThrows()
        {
            // Arrange.
            Runspace runspace = null;

            // Act.

            // Assert.
            new RunspaceWrapper(runspace);
        }

        [TestMethod]
        public void CtorSavesRunspaceReference()
        {
            // Arrange.
            var expectedRunspace = RunspaceFactory.CreateRunspace();

            // Act.
            var wrapper = new RunspaceWrapper(expectedRunspace);

            // Assert.
            Assert.AreEqual(expectedRunspace, wrapper.Runspace);
        }

        [TestMethod]
        public void CreateNestedPipelineWrappedPipeline()
        {
            // Arrange.
            var expectedRunspace = RunspaceFactory.CreateRunspace();
            var wrapper = new RunspaceWrapper(expectedRunspace);

            // Act.
            var pipeline = wrapper.CreateNestedPipeline("", false);

            // Assert.
            Assert.IsInstanceOfType(pipeline, typeof(PipelineWrapper));
        }
    }
}
