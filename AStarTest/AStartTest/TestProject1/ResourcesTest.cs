using AStartTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Resources;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ResourcesTest and is intended
    ///to contain all ResourcesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResourcesTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Resources Constructor
        ///</summary>
        [TestMethod()]
        public void ResourcesConstructorTest()
        {
            Resources target = new Resources();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Culture
        ///</summary>
        [TestMethod()]
        public void CultureTest()
        {
            CultureInfo expected = null; // TODO: Initialize to an appropriate value
            CultureInfo actual;
            Resources.Culture = expected;
            actual = Resources.Culture;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ResourceManager
        ///</summary>
        [TestMethod()]
        public void ResourceManagerTest()
        {
            ResourceManager actual;
            actual = Resources.ResourceManager;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
