using System;
using System.Management.Automation;

namespace PsTest
{
    /// <summary>
    /// Represents a PsTest test.
    /// </summary>
    public class Test
    {
        private readonly string _name;
        private readonly ScriptBlock _testScript;

        /// <summary>
        /// Initializes a new test instance with the specified name
        /// and test script.
        /// </summary>
        /// <param name="name">
        /// The name of the test.
        /// </param>
        /// <param name="testScript">
        /// The test script.
        /// </param>
        public Test(string name, ScriptBlock testScript)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (testScript == null)
            {
                throw new ArgumentNullException("testScript");
            }
            _name = name;
            _testScript = testScript;
        }

        /// <summary>
        /// Get the name of the test.
        /// </summary>
        public string Name { get { return _name; } }

        public Exception ExpectedException { get; set; }

        /// <summary>
        /// Get the test script.
        /// </summary>
        public ScriptBlock TestScript { get { return _testScript; } }
    }
}
