using Microsoft.VisualStudio.TestTools.UnitTesting;
using PsTest;
using System;

namespace PsTestTests
{
    [TestClass]
    public class TestResultTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoTestNameArgumentNullException()
        {
            // Arrange.

            // Act and assert.
            new TestResult(null, false);
        }

        [TestMethod]
        public void CtorSavesArgs()
        {
            // Arrange.
            const string testName = "test name";
            const bool success = true;

            // Act.
            var res = new TestResult(testName, success);

            // Assert.
            Assert.AreEqual(testName, res.TestName);
            Assert.AreEqual(success, res.Success);
        }
    }
}
