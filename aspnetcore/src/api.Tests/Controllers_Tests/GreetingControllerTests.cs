using Xunit;
using Microsoft.AspNetCore.Mvc;
using api.Controllers;

namespace api.Tests.Controllers_Tests
{
    /*
     * Tests for GreetingController experimental MVC functionality
     */
    public class GreetingControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new GreetingController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Greet_WithValidName_ReturnsResultView()
        {
            // Arrange
            var controller = new GreetingController();
            var name = "John Doe";

            // Act
            var result = controller.Greet(name);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Result", viewResult.ViewName);
            Assert.Equal(name, controller.ViewBag.Name);
        }

        [Fact]
        public void Greet_WithEmptyName_ReturnsIndexViewWithError()
        {
            // Arrange
            var controller = new GreetingController();

            // Act
            var result = controller.Greet("");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.NotNull(controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public void Greet_WithWhitespaceName_ReturnsIndexViewWithError()
        {
            // Arrange
            var controller = new GreetingController();

            // Act
            var result = controller.Greet("   ");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.NotNull(controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public void Greet_WithNullName_ReturnsIndexViewWithError()
        {
            // Arrange
            var controller = new GreetingController();

            // Act
            var result = controller.Greet(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.NotNull(controller.ViewBag.ErrorMessage);
        }
    }
}