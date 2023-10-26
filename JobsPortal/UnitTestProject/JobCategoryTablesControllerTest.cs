using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobsPortal.Controllers;
using System.Web.Mvc;
using DatabaseAccessLayer;
using Moq;

namespace UnitTestProject
{
    [TestClass]
    public class JobCategoryTablesControllerTest
    {


        private JobCategoryTablesController controller; // Controller instance for testing
        private Mock<ControllerContext> controllerContext; // Mock ControllerContext for setting up HttpContext

        [TestInitialize]
        public void Initialize()
        {
            controller = new JobCategoryTablesController();

            // Create a mock ControllerContext and set it in the controller
            controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["UserTypeID"]).Returns("1"); // Set the session value
            controller.ControllerContext = controllerContext.Object;
        }

        [TestMethod]
        public void TestIndexAction()
        {
            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Index");
        }


        [TestMethod]
        public void TestCreateAction()
        {
            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Create");
        }




        [TestMethod]
        public void TestEditAction()
        {
            // Arrange

            // Act
            int? id = 1; // Provide a valid ID for an existing JobCategoryTable
            ViewResult result = controller.Edit(id) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Edit");
        }

        [TestMethod]
        public void TestEditActionWithInvalidId()
        {
            // Arrange

            // Act
            int? id = null; // Provide an invalid ID
            HttpStatusCodeResult result = controller.Edit(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode); // Assuming you return a 400 status code for a bad request
        }

        [TestMethod]
        public void TestEditActionWithNonExistentId()
        {
            // Arrange

            // Act
            int? id = 999; // Provide a non-existent ID
            var result = controller.Edit(id);

            // Assert
            Assert.IsNotNull(result);

            // Assuming you return HttpNotFoundResult for non-existent IDs
            if (result is HttpNotFoundResult)
            {
                Assert.AreEqual(404, ((HttpStatusCodeResult)result).StatusCode);
            }
            else
            {
                // Handle the case when the action returns a different result, if necessary
                Assert.Fail("Expected HttpNotFoundResult, but received a different result.");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose of resources or perform cleanup if needed
        }
    }
}