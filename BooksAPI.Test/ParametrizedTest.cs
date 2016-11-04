using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BooksAPI.DTOs;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooksAPI.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ParametrizedTest
    {
        DbHelper db = new DbHelper();
        HttpHelper http = new HttpHelper();
        public ParametrizedTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        //[DeploymentItem(@"|DataDirectory|\DATA\Data.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", @"|DataDirectory|\DATA\Data.xml", "Row", DataAccessMethod.Sequential)]
        [TestMethod]
        public async Task GetBook_ShouldReturnBooks_IfProvidedValidDate()
        {
            DateTime date = new DateTime(2011, 10, 20, 12, 55, 53);
            db.AddNewBookWithAllFields(date, title: "test1");
            db.AddNewBookWithAllFields(date, title: "test2");

            string dateFormat = TestContext.DataRow[0].ToString();
            string message = TestContext.DataRow[1].ToString();

            using (var client = http.NewHttpClient())
            {
                var resp = await client.GetAsync("books/date/" + date.ToString(dateFormat));
                resp.EnsureSuccessStatusCode();
                var books = await resp.Content.ReadAsAsync<List<BookDto>>();
                var expectedBooks = db.GetBooksByDate(date);

                Assert.IsTrue(books.Count > 1);
                CollectionAssert.AreEqual(expectedBooks, books,message);
            }
        }
    }
}
