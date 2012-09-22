using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using PsTest;
using Rhino.Mocks;
using System.Management.Automation;

namespace PsTestTests
{
    [TestClass]
    public class InvokeTestCmdletTests
    {
        public class TestableInvokeTestCmdlet : InvokeTestCmdlet
        {
            public void DoProcessRecord() { ProcessRecord(); }
        }

        [TestMethod]
        public void ClassDecorated()
        {
            // Arrange.
            const string expectedNounName = "Test";

            // Act.
            var attribute = typeof(InvokeTestCmdlet)
                .GetAttribute<CmdletAttribute>();

            // Assert.
            Assert.AreEqual(VerbsLifecycle.Invoke, attribute.VerbName);
            Assert.AreEqual(expectedNounName, attribute.NounName);
        }

        [TestMethod]
        public void TestPropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(InvokeTestCmdlet)
                .GetProperty("Test")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.AreEqual(0, attribute.Position);
            Assert.IsTrue(attribute.Mandatory);
            Assert.IsTrue(attribute.ValueFromPipeline);
        }

        [TestMethod]
        public void ProcessRecordTwoTestsProcessed()
        {
            // Arrange.
            var runspace = new RunspaceInvoke();

            const string testName1 = "test1";
            var testScript1 = (ScriptBlock)
                runspace.Invoke("{$global:testScript1=$true}")[0].BaseObject;
            var test1 = new Test(testName1, testScript1);
            var testResult1 = new TestResult(testName1, true);

            var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(testResult1));

            const string testName2 = "test2";
            var testScript2 = (ScriptBlock)
                runspace.Invoke("{$global:testScript2=$true}")[0].BaseObject;
            var test2 = new Test(testName2, testScript2);
            var testResult2 = new TestResult(testName2, true);

            runtime.Expect(r => r.WriteObject(testResult2));

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableInvokeTestCmdlet>();
            cmdlet.Test = new[] { test1, test2 };
            cmdlet.CommandRuntime = runtime;
            cmdlet
                .Expect(c => c.CreateTestResult(test1.Name, true))
                .Return(testResult1);
            cmdlet
                .Expect(c => c.CreateTestResult(test2.Name, true))
                .Return(testResult2);

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.VerifyAllExpectations();
            runtime.VerifyAllExpectations();
            Assert.IsTrue((bool)runspace.Invoke("$testScript1")[0].BaseObject);
            Assert.IsTrue((bool)runspace.Invoke("$testScript2")[0].BaseObject);
        }

        [TestMethod]
        public void ProcessRecordThrowsNoSuccess()
        {
            // Arrange.
            const string testName = "test";
            var testScript1 = (ScriptBlock)
                new RunspaceInvoke().Invoke("{throw 1}")[0].BaseObject;
            var test = new Test(testName, testScript1);
            var testResult = new TestResult(testName, true);

            var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(testResult));

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableInvokeTestCmdlet>();
            cmdlet.Test = new[] { test };
            cmdlet.CommandRuntime = runtime;
            cmdlet
                .Expect(c => c.CreateTestResult(test.Name, false))
                .Return(testResult);

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.VerifyAllExpectations();
            runtime.VerifyAllExpectations();
        }

        [TestMethod]
        public void CreateTestResult()
        {
            // Arrange.
            const string expectedTestName = "test name";
            var cmdlet = new InvokeTestCmdlet();

            // Act.
            var res = cmdlet.CreateTestResult(expectedTestName, true);

            // Assert.
            Assert.AreEqual(expectedTestName, res.TestName);
            Assert.IsTrue(res.Success);
        }
    }
}
