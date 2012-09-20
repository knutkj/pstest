using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using PsTest;
using System.Management.Automation;
using System.Linq;
using Rhino.Mocks;
using System.Collections.Generic;

namespace PsTestTests
{
    [TestClass]
    public class NewTestCmdletTests
    {
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
            Assert.IsTrue(attribute.Mandatory);
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
            Assert.IsTrue(attribute.Mandatory);
        }

        [TestMethod]
        public void ProcessRecord()
        {
            // Arrange.
            const string expectedName = "Unit test name";
            var expectedTestScript = ScriptBlock.Create("");

            //var runtime = MockRepository.GenerateMock<ICommandRuntime>();
            //runtime
            //    .Expect(r => r.WriteObject(null))
            //    .IgnoreArguments();

            var cmdlet = new NewTestCmdlet
            {
                Name = expectedName,
                TestScript = expectedTestScript//,
                //CommandRuntime = runtime
            };

            // Act.
            var res = ToTestableList(cmdlet.Invoke<Test>());

            // Assert.
            Assert.AreEqual(1, res.Count());
            var test = res.First();
            Assert.AreEqual(expectedName, test.Name);
            Assert.AreEqual(expectedTestScript, test.TestScript);
        }

        #region Private helper methods...

        private static List<Test> ToTestableList(IEnumerable<Test> tmp)
        {
            var tests = new List<Test>();
            tests.AddRange(tmp);
            return tests;
        }

        #endregion
    }
}
