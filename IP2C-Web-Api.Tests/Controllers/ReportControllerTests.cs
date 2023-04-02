using FakeItEasy;
using FluentAssertions;
using IP2C_Web_API.Controllers;
using IP2C_Web_API.Interface;
using IP2C_Web_API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IP2C_Web_Api.Tests.Controllers
{
    public class ReportControllerTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("GR")]
        [InlineData("GR,CN")]
        [InlineData("GR,TEST")]
        [InlineData("TEST,DE,CY")]
        public async Task ReportController_GetReports_ReturnsOkResult(string? codes)
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var serviceResponse = new ServiceResponse<List<ReportDTO>>
            {
                Data = new List<ReportDTO>
                {
                    new ReportDTO
                    {
                        CountryName = "Test",
                        AddressesCount = 1,
                        LastAddressUpdated = DateTime.Now
                    }
                },
                Success = true
            };

            A.CallTo(() => unitOfWork.Report.GetReport(codes)).Returns(serviceResponse);

            var controller = new ReportController(unitOfWork);

            // Act
            var response = await controller.GetReports(codes);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(serviceResponse);
        }

        [Theory]
        [InlineData("T")]
        [InlineData("InvalidCountryName")]
        [InlineData("GRR")]
        [InlineData("GEE")]
        [InlineData("0")]
        [InlineData("123")]
        public async Task ReportController_GetReports_ReturnsNotFoundResult(string? codes)
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var serviceResponse = new ServiceResponse<List<ReportDTO>>
            {
                Success = false,
                Message = "No data found for the provided country codes."
            };

            A.CallTo(() => unitOfWork.Report.GetReport(codes)).Returns(serviceResponse);

            var controller = new ReportController(unitOfWork);

            // Act
            var response = await controller.GetReports(codes);

            // Assert
            response.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
