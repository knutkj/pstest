using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using PsTest;
using Rhino.Mocks;
using System;
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
            var test1 = new Test("t1", ScriptBlock.Create(string.Empty));
            var testResult1 = new TestResult("tr2", true);

            var test2 = new Test("t2", ScriptBlock.Create(string.Empty));
            var testResult2 = new TestResult("tr2", true);

            var runtime = MockRepository.GenerateStrictMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(testResult1));
            runtime.Expect(r => r.WriteObject(testResult2));

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableInvokeTestCmdlet>();
            cmdlet.Test = new[] { test1, test2 };
            cmdlet.CommandRuntime = runtime;
            cmdlet
                .Expect(c => c.InvokeScriptBlock(test1.TestScript))
                .Return(null);
            cmdlet
                .Expect(c => c.CreateTestResult(test1.Name, true))
                .Return(testResult1);
            cmdlet
                .Expect(c => c.InvokeScriptBlock(test2.TestScript))
                .Return(null);
            cmdlet
                .Expect(c => c.CreateTestResult(test2.Name, true))
                .Return(testResult2);

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.VerifyAllExpectations();
            runtime.VerifyAllExpectations();
        }

        [TestMethod]
        public void ProcessRecordThrowsNoSuccess()
        {
            // Arrange.
            var test = new Test("t", ScriptBlock.Create(string.Empty));
            var testResult = new TestResult("tr", true);

            var runtime = MockRepository.GenerateStrictMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(testResult));

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableInvokeTestCmdlet>();
            cmdlet.Test = new[] { test };
            cmdlet.CommandRuntime = runtime;
            cmdlet
                .Expect(c => c.InvokeScriptBlock(test.TestScript))
                .Throw(new Exception());
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
        public void ProcessRecordThrowsExpectedExceptionSuccess()
        {
            // Arrange.
            var test = new Test(
                "t", 
                ScriptBlock.Create(string.Empty),
                typeof(ArgumentNullException)
            );
            var testResult = new TestResult("tr", true);

            var runtime = MockRepository.GenerateStrictMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(testResult));

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableInvokeTestCmdlet>();
            cmdlet.Test = new[] { test };
            cmdlet.CommandRuntime = runtime;
            cmdlet
                .Expect(c => c.InvokeScriptBlock(test.TestScript))
                .Throw(new Exception("", new ArgumentNullException()));
            cmdlet
                .Expect(c => c.CreateTestResult(test.Name, true))
                .Return(testResult);

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.VerifyAllExpectations();
            runtime.VerifyAllExpectations();
        }

        [TestMethod]
        public void InvokeScriptBlock()
        {
            // Arrange.
            var runspace = new RunspaceInvoke();
            var scriptBlock = (ScriptBlock)
                runspace.Invoke("{$global:sb1=$true;123}")[0].BaseObject;
            var cmdlet = new InvokeTestCmdlet();

            // Act.
            var res = cmdlet.InvokeScriptBlock(scriptBlock);

            // Assert.
            Assert.AreEqual(123, (int)res[0].BaseObject);
            Assert.IsTrue((bool)runspace.Invoke("$sb1")[0].BaseObject);
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
