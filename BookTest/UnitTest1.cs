using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ServiceTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test_Create_GET_ReturnsViewResultNullModel()
        {
            // Arrange
            IRegisterRepository context = null;
            var controller = new ServicesController(context);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Test_Create_POST_InvalidModelState()
        {
            // Arrange
            var service = new Service
            {
                Title = "Dog Grooming",
                Description = "Professional grooming services",
                Price = 50.0m
            };
            var mockRepo = new Mock<IRegisterRepository>();
            mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Service>()));
            var controller = new ServicesController(mockRepo.Object);
            controller.ModelState.AddModelError("Title", "Название обязательно");
            controller.ModelState.AddModelError("ImagePath", "Картинка обязательна");

            // Act
            var result = await controller.Create(service);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData.Model);
            mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Service>()), Times.Never);
        }

        [Fact]
        public async Task Test_Create_POST_ValidModelState()
        {
            // Arrange
            var service = new Service
            {
                Title = "Cat Vaccination",
                ImagePath = "cat_vaccination.jpg",
                Description = "Comprehensive vaccination service",
                Price = 25.0m
            };

            var mockRepo = new Mock<IRegisterRepository>();
            mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Service>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var controller = new ServicesController(mockRepo.Object);

            // Act
            var result = await controller.Create(service);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockRepo.Verify();
        }

        [Fact]
        public async Task Test_Edit_POST_ValidModelState()
        {
            // Arrange
            int serviceId = 1;
            var updatedService = new Service
            {
                Id = serviceId,
                Title = "Updated Service",
                ImagePath = "updated_service.jpg",
                Description = "Updated Description",
                Price = 60.0m
            };

            var mockRepo = new Mock<IRegisterRepository>();
            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Service>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            var controller = new ServicesController(mockRepo.Object);

            // Act
            var result = await controller.Edit(serviceId, updatedService);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockRepo.Verify(repo => repo.UpdateAsync(updatedService), Times.Once);
        }

        [Fact]
        public async Task Test_Read_GET_ReturnsViewResult_WithAListOfServices()
        {
            // Arrange
            var mockRepo = new Mock<IRegisterRepository>();
            mockRepo.Setup(repo => repo.ListAsync()).ReturnsAsync(GetTestServices());
            var controller = new ServicesController(mockRepo.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Service>>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Count());
        }

        private static List<Service> GetTestServices()
        {
            var services = new List<Service>
            {
                new Service
                {
                    Title = "Dog Grooming",
                    ImagePath = "dog_grooming.jpg",
                    Description = "Professional grooming services",
                    Price = 50.0m
                },
                new Service
                {
                    Title = "Cat Vaccination",
                    ImagePath = "cat_vaccination.jpg",
                    Description = "Comprehensive vaccination service",
                    Price = 25.0m
                },
                new Service
                {
                    Title = "Bird Checkup",
                    ImagePath = "bird_checkup.jpg",
                    Description = "Regular checkups for your feathered friends",
                    Price = 30.0m
                }
            };

            return services;
        }

        [Fact]
        public async Task Test_Delete_POST_ValidModelState()
        {
            // Arrange
            int serviceId = 1;

            var mockRepo = new Mock<IRegisterRepository>();
            mockRepo.Setup(repo => repo.DeleteAsync(serviceId))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            var controller = new ServicesController(mockRepo.Object);

            // Act
            var result = await controller.DeleteConfirmed(serviceId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockRepo.Verify(repo => repo.DeleteAsync(serviceId), Times.Once);
        }


        [Fact]
        public async Task Test_Details_GET_ReturnsViewResult_WithService()
        {
            // Arrange
            int serviceId = 1;
            var testService = new Service
            {
                Id = serviceId,
                Title = "Dog Grooming",
                ImagePath = "dog_grooming.jpg",
                Description = "Professional grooming services",
                Price = 50.0m
            };
            var mockRepo = new Mock<IRegisterRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(testService);
            var controller = new ServicesController(mockRepo.Object);

            // Act
            var result = await controller.Details(serviceId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Service>(viewResult.ViewData.Model);
            Assert.Equal(serviceId, model.Id);
        }

        [Fact]
        public async Task Test_Edit_POST_InvalidModelState()
        {
            // Arrange
            int serviceId = 1;
            var updatedService = new Service
            {
                Id = serviceId,
                Title = "",
                ImagePath = "updated_service.jpg",
                Description = "Updated Description",
                Price = 60.0m
            };

            var mockRepo = new Mock<IRegisterRepository>();
            var controller = new ServicesController(mockRepo.Object);
            controller.ModelState.AddModelError("Title", "Название обязательно");

            // Act
            var result = await controller.Edit(serviceId, updatedService);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData.Model);
            mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Service>()), Times.Never);
        }

        [Fact]
        public async Task Test_Delete_POST_NonExistentService()
        {
            // Arrange
            int serviceId = 999; // Несуществующий идентификатор

            var mockRepo = new Mock<IRegisterRepository>();
            mockRepo.Setup(repo => repo.DeleteAsync(serviceId))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            var controller = new ServicesController(mockRepo.Object);

            // Act
            var result = await controller.DeleteConfirmed(serviceId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockRepo.Verify(repo => repo.DeleteAsync(serviceId), Times.Once);
        }
    }
}
