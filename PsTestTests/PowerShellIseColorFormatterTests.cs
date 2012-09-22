using Microsoft.VisualStudio.TestTools.UnitTesting;
using PsTest;
using Rhino.Mocks;
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PsTestTests
{
    [TestClass]
    public class PowerShellIseColorFormatterTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoRunspaceThrowsArgumentNullException()
        {
            // Arrange.

            // Act.

            // Assert.
            new PowerShellIseColorFormatter(null);
        }

        [TestMethod]
        public void CtorSavesRunspaceReference()
        {
            // Arrange.
            var expectedRunspace = MockRepository.GenerateStub<IRunspace>();

            // Act.
            var formatter = new PowerShellIseColorFormatter(expectedRunspace);

            // Assert.
            Assert.AreEqual(expectedRunspace, formatter.Runspace);
        }

        [TestMethod]
        public void SetBackgroundColor()
        {
            // Arrange.
            const string color = "color";
            var command = string.Format(
                "$psISE.Options.OutputPaneTextBackgroundColor = '{0}'",
                color
            );

            var pipeline = MockRepository.GenerateMock<IPipeline>();
            pipeline.Expect(p => p.Invoke()).Return(null);

            var runspace = MockRepository.GenerateMock<IRunspace>();
            runspace
                .Expect(r => r.CreateNestedPipeline(command, false))
                .Return(pipeline);

            var formatter = new PowerShellIseColorFormatter(runspace);

            // Act.
            formatter.SetBackgroundColor(color);

            // Assert.
            runspace.VerifyAllExpectations();
            pipeline.VerifyAllExpectations();
        }

        [TestMethod]
        public void GetBackgroundColor()
        {
            // Arrange.
            const string expectedColor = "color";
            var psobjects = new Collection<PSObject>
            { 
                new PSObject(expectedColor)
            };
            const string command =
                "[string]$psISE.Options.OutputPaneTextBackgroundColor";

            var pipeline = MockRepository.GenerateMock<IPipeline>();
            pipeline.Expect(p => p.Invoke()).Return(psobjects);

            var runspace = MockRepository.GenerateMock<IRunspace>();
            runspace
                .Expect(r => r.CreateNestedPipeline(command, false))
                .Return(pipeline);

            var formatter = new PowerShellIseColorFormatter(runspace);

            // Act.
            var actualColor = formatter.GetBackgroundColor();

            // Assert.
            runspace.VerifyAllExpectations();
            pipeline.VerifyAllExpectations();
            Assert.AreEqual(expectedColor, actualColor);
        }
    }
}
