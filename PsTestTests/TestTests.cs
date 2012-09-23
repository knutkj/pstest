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

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoExpectedExceptionThrowsArgumentNullException()
        {
            // Arrange.

            // Act and assert.
            new Test("test name", ScriptBlock.Create(""), null);
        }

        [TestMethod]
        public void CtorWithTwoParamsSavesArgs()
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

        [TestMethod]
        public void CtorWithThreeParamsSavesArgs()
        {
            // Arrange.
            const string expectedName = "test name";
            var expectedTestScript = ScriptBlock.Create("");
            var expectedException = typeof(Exception);

            // Act.
            var res = new Test(
                expectedName,
                expectedTestScript,
                expectedException
            );

            // Assert.
            Assert.AreEqual(expectedName, res.Name);
            Assert.AreEqual(expectedTestScript, res.TestScript);
            Assert.AreEqual(expectedException, res.ExpectedException);
        }
    }
}
