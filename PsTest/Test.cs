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
        private readonly Type _expectedException;

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
        /// Initializes a new instance with the specified name, test script
        /// and expected exception.
        /// </summary>
        /// <param name="name">
        /// The name of the test.
        /// </param>
        /// <param name="testScript">
        /// The test script.
        /// </param>
        /// <param name="expectedException">
        /// The expected exception.
        /// </param>
        public Test(
            string name,
            ScriptBlock testScript,
            Type expectedException
        )
            : this(name, testScript)
        {
            if (expectedException == null)
            {
                throw new ArgumentNullException();
            }
            _expectedException = expectedException;
        }

        /// <summary>
        /// Get the name of the test.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Get the expected exception type.
        /// </summary>
        public Type ExpectedException
        {
            get { return _expectedException; }
        }

        /// <summary>
        /// Get the test script.
        /// </summary>
        public ScriptBlock TestScript { get { return _testScript; } }
    }
}
