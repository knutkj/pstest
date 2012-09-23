using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using PsTest;
using Rhino.Mocks;
using System;
using System.Linq;
using System.Management.Automation;

namespace PsTestTests
{
    [TestClass]
    public class NewTestCmdletTests
    {
        public class TestableNewTestCmdlet : NewTestCmdlet
        {
            public void DoProcessRecord() { ProcessRecord(); }
        }

        [TestMethod]
        public void ClassDecorated()
        {
            // Arrange.
            const string verb = VerbsCommon.New;

            // Act.
            var attribute = typeof(NewTestCmdlet)
                .GetAttribute<CmdletAttribute>();

            // Assert.
            Assert.AreEqual<string>(verb, attribute.VerbName);
            Assert.AreEqual<string>("Test", attribute.NounName);
            Assert.AreEqual<string>(
                NewTestCmdlet.DefaultParameterSetName,
                attribute.DefaultParameterSetName
            );
        }

        [TestMethod]
        public void NamePropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(NewTestCmdlet)
                .GetProperty("Name")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.AreEqual(0, attribute.Position);
            Assert.IsTrue(attribute.Mandatory);
            Assert.IsTrue(attribute.ValueFromPipelineByPropertyName);
        }

        [TestMethod]
        public void TestScriptPropDecorated()
        {
            // Arrange.

            // Act.
            var attributes = typeof(NewTestCmdlet)
                .GetProperty("TestScript")
                .GetCustomAttributes(true)
                .Where(a => a is ParameterAttribute)
                .Cast<ParameterAttribute>();

            // Assert.
            var paramSet1 = attributes.First(a => a.Position == 1);
            Assert.AreEqual(
                NewTestCmdlet.DefaultParameterSetName,
                paramSet1.ParameterSetName
            );
            Assert.IsTrue(paramSet1.Mandatory);
            Assert.IsTrue(paramSet1.ValueFromPipelineByPropertyName);

            var paramSet2 = attributes.First(a => a.Position == 2);
            Assert.AreEqual(
                NewTestCmdlet.ExceptionParameterSetName,
                paramSet2.ParameterSetName
            );
            Assert.IsTrue(paramSet2.Mandatory);
            Assert.IsTrue(paramSet2.ValueFromPipelineByPropertyName);
        }

        [TestMethod]
        public void ExpectedExceptionPropDecorated()
        {
            // Arrange.

            // Act.
            var attribute = typeof(NewTestCmdlet)
                .GetProperty("ExpectedException")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.AreEqual(
                NewTestCmdlet.ExceptionParameterSetName,
                attribute.ParameterSetName
            );
            Assert.AreEqual(1, attribute.Position);
            Assert.IsTrue(attribute.Mandatory);
            Assert.IsTrue(attribute.ValueFromPipelineByPropertyName);
        }

        [TestMethod]
        public void ProcessRecord()
        {
            // Arrange.
            var expectedTest = new Test("name", ScriptBlock.Create(""));

            var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            runtime.Expect(r => r.WriteObject(expectedTest));

            var cmdlet = MockRepository
                .GeneratePartialMock<TestableNewTestCmdlet>();
            cmdlet.CommandRuntime = runtime;
            cmdlet.Expect(c => c.CreateTest()).Return(expectedTest);

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            cmdlet.VerifyAllExpectations();
            runtime.VerifyAllExpectations();
        }

        [TestMethod]
        public void CreateTestCreatesTestWithoutExpectedException()
        {
            // Arrange.
            const string expectedName = "name";
            var expectedTestScript = ScriptBlock.Create("");

            var cmdlet = new NewTestCmdlet
            {
                Name = expectedName,
                TestScript = expectedTestScript
            };

            // Act.
            var test = cmdlet.CreateTest();

            // Assert.
            Assert.AreEqual(expectedName, test.Name);
            Assert.AreEqual(expectedTestScript, test.TestScript);
        }

        [TestMethod]
        public void CreateTestCreatesTestWithExpectedException()
        {
            // Arrange.
            const string expectedName = "name";
            var expectedTestScript = ScriptBlock.Create("");
            var expectedException = typeof(Exception);

            var cmdlet = new NewTestCmdlet
            {
                Name = expectedName,
                TestScript = expectedTestScript,
                ExpectedException = expectedException
            };

            // Act.
            var test = cmdlet.CreateTest();

            // Assert.
            Assert.AreEqual(expectedName, test.Name);
            Assert.AreEqual(expectedTestScript, test.TestScript);
            Assert.AreEqual(expectedException, test.ExpectedException);
        }
    }
}
