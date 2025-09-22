using Xunit;
using Microsoft.AspNetCore.Mvc;
using api.Controllers;

namespace api.Tests.Controllers_Tests
{
    /*
     * Tests for AiPocController experimental MVC functionality
     */
    public class AiPocControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new AiPocController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Greet_WithValidInput_ReturnsResultView()
        {
            // Arrange
            var controller = new AiPocController();
            var orcid = "abc123";

            // Act
            var result = controller.Greet(orcid);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Result", viewResult.ViewName);
            Assert.Equal(orcid, controller.ViewBag.Orcid);
        }

        [Fact]
        public void Greet_WithEmptyOrcid_ReturnsIndexViewWithError()
        {
            // Arrange
            var controller = new AiPocController();

            // Act
            var result = controller.Greet("");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.NotNull(controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public void Greet_WithWhitespaceOrcid_ReturnsIndexViewWithError()
        {
            // Arrange
            var controller = new AiPocController();

            // Act
            var result = controller.Greet("   ");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.NotNull(controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public void Greet_WithNullOrcid_ReturnsIndexViewWithError()
        {
            // Arrange
            var controller = new AiPocController();

            // Act
            var result = controller.Greet(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.NotNull(controller.ViewBag.ErrorMessage);
        }
    }
}