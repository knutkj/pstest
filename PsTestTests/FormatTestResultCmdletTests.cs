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
        public void AllPropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(FormatTestResultCmdlet)
                .GetProperty("All")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.IsInstanceOfType(attribute, typeof(ParameterAttribute));
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
        public void ProcessRecordProcessesTwoSuccesfulTestResultsCorrectly()
        {
            // Arrange.
            var test1 = new TestResult("1", true);
            var test2 = new TestResult("2", true);
            var tests = new[] { test1, test2 };

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.All = true;
            cmdlet.TestResult = tests;
            cmdlet.Expect(c => c.NumberOfTestResults = 1);
            cmdlet.Expect(c => c.NumberOfTestResults).Return(5);
            cmdlet.Expect(c => c.NumberOfTestResults = 6);
            cmdlet.Expect(c => c.NumberOfSuccessfulTestResults = 1);
            cmdlet.Expect(c => c.NumberOfSuccessfulTestResults).Return(10);
            cmdlet.Expect(c => c.NumberOfSuccessfulTestResults = 11);
            cmdlet.Expect(c => c.WriteTestResult(test1));
            cmdlet.Expect(c => c.WriteTestResult(test2));

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.VerifyAllExpectations();
        }

        [TestMethod]
        public void ProcessRecordProcessesTwoFailedTestResultsCorrectly()
        {
            // Arrange.
            var test1 = new TestResult("1", false);
            var test2 = new TestResult("2", false);
            var tests = new[] { test1, test2 };

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.All = true;
            cmdlet.TestResult = tests;
            cmdlet.Expect(c => c.NumberOfTestResults = 1);
            cmdlet.Expect(c => c.NumberOfTestResults).Return(4);
            cmdlet.Expect(c => c.NumberOfTestResults = 5);
            cmdlet.Expect(c => c.WriteTestResult(test1));
            cmdlet.Expect(c => c.WriteTestResult(test2));

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.AssertWasNotCalled(
                c => c.NumberOfSuccessfulTestResults = -1,
                c => c.IgnoreArguments()
            );
            cmdlet.VerifyAllExpectations();
        }

        [TestMethod]
        public void ProcessRecordDoesNotWriteSuccessIfNotAllSwitchSpecified()
        {
            // Arrange.
            var tests = new[] { new TestResult("1", true) };

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.TestResult = tests;

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.AssertWasNotCalled(
                c => c.WriteTestResult(null),
                a => a.IgnoreArguments()
            );
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
        public void WriteTestResultCorrectBgForFailedTests()
        {
            // Arrange.
            const string testName = "1";
            var testResult = new TestResult(testName, false);
            const string failureColor = "failure color";

            var expectedLine = string.Format("{0,-80}", testName);

            var colorFormatter = MockRepository
                .GenerateMock<IColorFormatter>();
            colorFormatter.Expect(c => c.SetBackgroundColor(failureColor));

            var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(expectedLine));

            var cmdlet = MockRepository
                .GeneratePartialMock<FormatTestResultCmdlet>();
            cmdlet.Expect(c => c.FailureColor).Return(failureColor);
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
        public void EndProcessingWritesEndResAndRestoresOriginalBgColor()
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
                .Expect(c => c.WriteEndResult());
            cmdlet
                .Expect(c => c.OriginalBackGroundColor)
                .Return(expectedBgColor)
                .WhenCalled(
                    a => cmdlet.AssertWasCalled(c => c.WriteEndResult())
                );

            // Act.
            cmdlet.DoEndProcessing();

            // Assert.
            colorFormatter.VerifyAllExpectations();
            cmdlet.VerifyAllExpectations();
        }

        [TestMethod]
        public void WriteEndResultAllFailedUsesFailureColor()
        {
            // Arrange.
            const int numberOfSuccessfulTestResults = 321;
            const int numberOfTestResults = 123;
            const string failureColor = "failure color";
            var expectedText = string.Format(
                "Passed tests: {0}/{1}.",
                numberOfSuccessfulTestResults,
                numberOfTestResults
            );
            var expectedLine = string.Format("{0,-80}", expectedText);

            var colorFormatter = MockRepository
                .GenerateMock<IColorFormatter>();
            colorFormatter
                .Expect(c => c.SetBackgroundColor(failureColor));

            var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            runtime
                .Expect(r => r.WriteObject(""));
            runtime
                .Expect(r => r.WriteObject(expectedLine))
                .WhenCalled(a =>
                {
                    colorFormatter.AssertWasCalled(
                        c => c.SetBackgroundColor(failureColor)
                    );
                    runtime.AssertWasCalled(r => r.WriteObject(""));
                });

            var cmdlet = MockRepository
                .GeneratePartialMock<FormatTestResultCmdlet>();
            cmdlet.ColorFormatter = colorFormatter;
            cmdlet.CommandRuntime = runtime;
            cmdlet
                .Expect(c => c.Success)
                .Return(false);
            cmdlet
                .Expect(c => c.FailureColor)
                .Return(failureColor)
                .WhenCalled(a => cmdlet.AssertWasCalled(c => c.Success));
            cmdlet
                .Expect(c => c.NumberOfSuccessfulTestResults)
                .Return(numberOfSuccessfulTestResults);
            cmdlet
                .Expect(c => c.NumberOfTestResults)
                .Return(numberOfTestResults);

            // Act.
            cmdlet.WriteEndResult();

            // Assert.
            colorFormatter.VerifyAllExpectations();
            cmdlet.VerifyAllExpectations();
            runtime.VerifyAllExpectations();
        }

        [TestMethod]
        public void WriteEndResultAllSuccessfulUsesSuccessColor()
        {
            // Arrange.
            const string successColor = "success color";

            var colorFormatter = MockRepository
                .GenerateMock<IColorFormatter>();
            colorFormatter
                .Expect(c => c.SetBackgroundColor(successColor));

            var runtime = MockRepository.GenerateStub<ICommandRuntime>();

            var cmdlet = MockRepository
                .GeneratePartialMock<FormatTestResultCmdlet>();
            cmdlet.ColorFormatter = colorFormatter;
            cmdlet.CommandRuntime = runtime;
            cmdlet.Expect(c => c.Success).Return(true);
            cmdlet.Expect(c => c.SuccessColor).Return(successColor);

            // Act.
            cmdlet.WriteEndResult();

            // Assert.
            colorFormatter.VerifyAllExpectations();
            cmdlet.VerifyAllExpectations();
        }

        [TestMethod]
        public void SuccessTrueIfSameNumSuccessAsTotalNumTestResults()
        {
            // Arrange.
            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.Expect(c => c.NumberOfTestResults).Return(1);
            cmdlet.Expect(c => c.NumberOfSuccessfulTestResults).Return(1);

            // Act.
            var res = cmdlet.Success;

            // Assert.
            cmdlet.VerifyAllExpectations();
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void SuccessFalseIfNotSameNumSuccessAsTotalNumTestResults()
        {
            // Arrange.
            var cmdlet = MockRepository
                .GeneratePartialMock<TestableFormatTestResultCmdlet>();
            cmdlet.Expect(c => c.NumberOfTestResults).Return(1);
            cmdlet.Expect(c => c.NumberOfSuccessfulTestResults).Return(0);

            // Act.
            var res = cmdlet.Success;

            // Assert.
            cmdlet.VerifyAllExpectations();
            Assert.IsFalse(res);
        }
    }
}
