using System;

namespace PsTest
{
    /// <summary>
    /// Represents the result of an invoked unit test.
    /// </summary>
    public class TestResult
    {
        private readonly string _testName;
        private readonly bool _success;

        /// <summary>
        /// Initializes a new instance of the TestResult class with the
        /// specified test name and success status.
        /// </summary>
        /// <param name="testName">
        /// The name of the test.
        /// </param>
        /// <param name="success">
        /// True if the test was a success or false.
        /// </param>
        public TestResult(string testName, bool success)
        {
            if (testName == null)
            {
                throw new ArgumentNullException("testName");
            }
            _testName = testName;
            _success = success;
        }

        /// <summary>
        /// Get the name of the test.
        /// </summary>
        public string TestName { get { return _testName; } }

        /// <summary>
        /// Get a value indicating if the test was successful or not.
        /// </summary>
        public bool Success { get { return _success; } }
    }
}
