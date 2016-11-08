using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BooksAPI.Test
{
    /*
     * This class is made only with purpose of cleaning data after tests execution.
     */
    [TestClass]
    public class BookController_BaseTest
    {
        protected static DbHelper db = new DbHelper();
        private static int firstExternalIdForTests;

        [AssemblyInitialize]
        public static void SetUp(TestContext context)
        {
            firstExternalIdForTests = db.GetMaxBookExternalId()+1;
        }

        [AssemblyCleanup]
        public static void ClassCleanup()
        {
            db.DeleteBooksFromId(firstExternalIdForTests);
        }
    }
}
