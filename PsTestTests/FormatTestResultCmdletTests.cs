using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using PsTest;
using Rhino.Mocks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PsTestTests
{
    [TestClass]
    public class FormatTestResultCmdletTests
    {
        public class TestableFormatTestResultCmdlet : FormatTestResultCmdlet
        {
            public void DoBeginProcessing() { BeginProcessing(); }
            public void DoProcessRecord() { ProcessRecord(); }
            public void DoEndProcessing() { EndProcessing(); }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var expectedRunspace = RunspaceFactory.CreateRunspace();
            Runspace.DefaultRunspace = expectedRunspace;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Runspace.DefaultRunspace = null;
        }

        [TestMethod]
        public void ClassDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(FormatTestResultCmdlet)
                .GetAttribute<CmdletAttribute>();

            // Assert.
            Assert.AreEqual(VerbsCommon.Format, attribute.VerbName);
            Assert.AreEqual("TestResult", attribute.NounName);
        }

        [TestMethod]
        public void TestResultPropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(FormatTestResultCmdlet)
                .GetProperty("TestResult")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.AreEqual(0, attribute.Position);
            Assert.IsTrue(attribute.Mandatory);
            Assert.IsTrue(attribute.ValueFromPipeline);
        }

        [TestMethod]
        public void SuccessColorPropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(FormatTestResultCmdlet)
                .GetProperty("SuccessColor")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.AreEqual(1, attribute.Position);
            Assert.IsFalse(attribute.Mandatory);
        }

        [TestMethod]
        public void FailureColorPropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(FormatTestResultCmdlet)
                .GetProperty("FailureColor")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.AreEqual(2, attribute.Position);
            Assert.IsFalse(attribute.Mandatory);
        }

        [TestMethod]
        public void ColorFormatterPropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(FormatTestResultCmdlet)
                .GetProperty("ColorFormatter")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.IsFalse(attribute.Mandatory);
        }

        [TestMethod]
        public void CtorSetsDefaultValuesForColorFormatterAndColors()
        {
            // Arrange.
            var expectedRunspace = Runspace.DefaultRunspace;

            // Act.
            var cmdlet = new FormatTestResultCmdlet();

            // Assert.
            Assert.AreEqual(
                FormatTestResultCmdlet.DefaultSuccessColor,
                cmdlet.SuccessColor
            );
            Assert.AreEqual(
                FormatTestResultCmdlet.DefaultFailureColor,
                cmdlet.FailureColor
            );
            Assert.IsInstanceOfType(
                cmdlet.ColorFormatter,
                typeof(PowerShellIseColorFormatter)
            );
            var formatter = (PowerShellIseColorFormatter)cmdlet.ColorFormatter;
            var actualRunspace =
                ((RunspaceWrapper)formatter.Runspace).Runspace;
            Assert.AreEqual(expectedRunspace, actualRunspace);
        }

        [TestMethod]
        public void ProcessRecordProcessesTwoTestResults()
        {
            // Arrange.
            var test1 = new TestResult("1", true);
            var test2 = new TestResult("2", true);
            var tests = new[] { test1, test2 };

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.TestResult = tests;
            cmdlet.Expect(c => c.WriteTestResult(test1));
            cmdlet.Expect(c => c.WriteTestResult(test2));

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.VerifyAllExpectations();
        }

        [TestMethod]
        public void WriteTestResultSetsBackgroundColorAndWritesTestName()
        {
            // Arrange.
            const string testName = "1";
            var testResult = new TestResult(testName, true);
            const string successColor = "success color";

            var expectedLine = string.Format("{0,-80}", testName);

            var colorFormatter = MockRepository
                .GenerateMock<IColorFormatter>();
            colorFormatter.Expect(c => c.SetBackgroundColor(successColor));

            var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(expectedLine));

            var cmdlet = MockRepository
                .GeneratePartialMock<FormatTestResultCmdlet>();
            cmdlet.Expect(c => c.SuccessColor).Return(successColor);
            cmdlet.ColorFormatter = colorFormatter;
            cmdlet.CommandRuntime = runtime;

            // Act.
            cmdlet.WriteTestResult(testResult);

            // Assert.
            cmdlet.VerifyAllExpectations();
            colorFormatter.VerifyAllExpectations();
            runtime.VerifyAllExpectations();
        }

        [TestMethod]
        public void BeginProcessingSavesOriginalBgColor()
        {
            // Arrange.
            const string expectedBgColor = "bg color";

            var colorFormatter = MockRepository
                .GenerateMock<IColorFormatter>();
            colorFormatter
                .Expect(c => c.GetBackgroundColor())
                .Return(expectedBgColor);

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.ColorFormatter = colorFormatter;
            cmdlet.Expect(c => c.OriginalBackGroundColor = expectedBgColor);

            // Act.
            cmdlet.DoBeginProcessing();

            // Assert.
            colorFormatter.VerifyAllExpectations();
            cmdlet.VerifyAllExpectations();
        }

        [TestMethod]
        public void EndProcessingRestoresOriginalBgColor()
        {
            // Arrange.
            const string expectedBgColor = "bg color";

            var colorFormatter = MockRepository
                .GenerateMock<IColorFormatter>();
            colorFormatter
                .Expect(c => c.SetBackgroundColor(expectedBgColor));

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.ColorFormatter = colorFormatter;
            cmdlet
                .Expect(c => c.OriginalBackGroundColor)
                .Return(expectedBgColor);

            // Act.
            cmdlet.DoEndProcessing();

            // Assert.
            colorFormatter.VerifyAllExpectations();
            cmdlet.VerifyAllExpectations();
        }
    }
}
