using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using PsTest;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Management.Automation;

namespace PsTestTests
{
    [TestClass]
    public class NewTestCmdletTests
    {
        private class TestableNewTestCmdlet : NewTestCmdlet
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
            var attribute = typeof(NewTestCmdlet)
                .GetProperty("TestScript")
                .GetAttribute<ParameterAttribute>();

            // Assert.
            Assert.AreEqual(1, attribute.Position);
            Assert.IsTrue(attribute.Mandatory);
            Assert.IsTrue(attribute.ValueFromPipelineByPropertyName);
        }

        [TestMethod]
        public void ProcessRecord()
        {
            // Arrange.
            const string expectedName = "Unit test name";
            var expectedTestScript = ScriptBlock.Create("");
            Test expectedTest = null;

            var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            runtime
                .Expect(r => r.WriteObject(Arg<Test>.Is.TypeOf))
                .WhenCalled(a => expectedTest = (Test)a.Arguments[0]);

            var cmdlet = new TestableNewTestCmdlet
            {
                Name = expectedName,
                TestScript = expectedTestScript,
                CommandRuntime = runtime
            };

            // Act.
            cmdlet.DoProcessRecord();

            // Assert.
            runtime.VerifyAllExpectations();
            Assert.AreEqual(expectedName, expectedTest.Name);
            Assert.AreEqual(expectedTestScript, expectedTest.TestScript);
        }
    }
}
