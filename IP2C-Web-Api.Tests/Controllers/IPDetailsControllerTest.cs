using FakeItEasy;
using FluentAssertions;
using IP2C_Web_API.Controllers;
using IP2C_Web_API.Interface;
using IP2C_Web_API.Interfaces;
using IP2C_Web_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace IP2C_Web_Api.Tests.Controllers;

public class IPDetailsControllerTest
{
    [Theory]
    [InlineData("1.1.1.1")]
    [InlineData("5.243.22.1")]
    [InlineData("255.1.1.255")]
    [InlineData("255.255.255.255")]
    public async Task IPDetailsController_GetIPDetails_ReturnsOkResult(string ip)
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var messageProducer = A.Fake<IMessageProducer>();
        var serviceResponse = new ServiceResponse<IPDetailsDTO>
        {
            Data = new IPDetailsDTO
            {
                CountryName = "Test",
                TwoLetterCode = "TS",
                ThreeLetterCode = "TST"
            },
            Success = true
        };

        A.CallTo(() => unitOfWork.IPDetails.GetIPDetails(ip)).Returns(serviceResponse);

        // Act
        var response = await new IPDetailsController(unitOfWork, messageProducer).GetIPDetails(ip);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeEquivalentTo(serviceResponse);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1,2,3,4")]
    [InlineData("1 2 3 4")]
    [InlineData("1.1.1.256")]
    [InlineData("Hello")]
    [InlineData(" 5.43.23.54")]
    [InlineData("100.0.100.100")]
    [InlineData("50.50.50.50.")]
    [InlineData("50.50.50.50.50")]
    [InlineData("5.5.5.o")]
    public async Task IPDetailsController_GetIPDetails_ReturnsNotFoundResult(string ip)
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var messageProducer = A.Fake<IMessageProducer>();

        A.CallTo(() => unitOfWork.IPDetails.GetIPDetails(ip)).Returns(new ServiceResponse<IPDetailsDTO>
        {
            Success = false,
            Message = "Invalid IP value."
        }
        );

        // Act
        var response = await new IPDetailsController(unitOfWork,messageProducer).GetIPDetails(ip);

        // Assert
        response.Result.Should().BeOfType<NotFoundObjectResult>();
    }

}
