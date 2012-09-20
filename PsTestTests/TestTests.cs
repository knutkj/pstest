using Microsoft.VisualStudio.TestTools.UnitTesting;
using PsTest;
using System;
using System.Management.Automation;

namespace PsTestTests
{
    [TestClass]
    public class TestTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoNameException()
        {
            // Arrange.

            // Act and assert.
            new Test(null, ScriptBlock.Create(""));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoTestScriptException()
        {
            // Arrange.

            // Act and assert.
            new Test("test name", null);
        }

        [TestMethod]
        public void CtorSavesArgs()
        {
            // Arrange.
            const string name = "test name";
            var testScript = ScriptBlock.Create("");

            // Act.
            var res = new Test(name, testScript);

            // Assert.
            Assert.AreEqual(name, res.Name);
            Assert.AreEqual(testScript, res.TestScript);
        }
    }
}
