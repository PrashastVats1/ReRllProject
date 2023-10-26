using System;
using NUnit.Framework;
using JobsPortal.Controllers;
using System.Web.Mvc;
using DatabaseAccessLayer;
using Moq;

namespace UnitTestProject
{
    [TestFixture]
    public class JobNatureTablesControllerTest
    {
        private JobNatureTablesController controller; // Controller instance for testing
        private Mock<ControllerContext> controllerContext; // Mock ControllerContext for setting up HttpContext

        [SetUp]
        public void Initialize()
        {
            controller = new JobNatureTablesController();

            // Create a mock ControllerContext and set it in the controller
            controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["UserTypeID"]).Returns("1"); // Set the session value
            controller.ControllerContext = controllerContext.Object;
        }

        [Test]
        public void TestIndexAction()
        {
            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.IsNotNull(result);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "The action did not return a ViewResult.");

            // Since the ViewName is not set explicitly in your action, the ViewName property will be empty. 
            // We should consider this case as valid for your Index action.
            if (string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "Index")
            {
                Assert.Pass("The view name matches the expected result.");
            }
            else
            {
                Assert.Fail($"Expected 'Index', but was '{viewResult.ViewName}'.");
            }
        }

        [Test]
        public void TestCreateAction()
        {
            // Act
            ActionResult result = controller.Create();

            // Assert
            Assert.IsNotNull(result);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "The action did not return a ViewResult.");

            // Since the ViewName is not set explicitly in your action, the ViewName property will be empty. 
            // We should consider this case as valid for your Create action.
            if (string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "Create")
            {
                Assert.Pass("The view name matches the expected result.");
            }
            else
            {
                Assert.Fail($"Expected 'Create', but was '{viewResult.ViewName}'.");
            }
        }

        [TearDown]
        public void Cleanup()
        {
            // Dispose of resources or perform cleanup if needed
        }
    }
}