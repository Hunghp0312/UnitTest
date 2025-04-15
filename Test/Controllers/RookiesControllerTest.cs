using Moq;
using Assignment2.Controllers;
using Assignment2.Services;
using Assignment2.Models;
using Microsoft.AspNetCore.Mvc;
using Assignment2.ViewModels;
using Assignment.CustomException;

namespace Test.Controllers;

[TestFixture]
public class RookiesControllerTests
{
    private Mock<IPersonService> _mockPersonService;
    private RookiesController _controller;

    [SetUp]
    public void Setup()
    {
        _mockPersonService = new Mock<IPersonService>();
        _controller = new RookiesController(_mockPersonService.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose(); // if your controller is IDisposable
    }

    [Test]
    public void Index_ReturnsViewWithCorrectModel()
    {
        // Arrange
        var mockViewModel = new PersonViewModel
        {
            People = new List<Person>(),
            PageSize = 5,
            PageIndex = 1,
            TotalPage = 1
        };
        _mockPersonService.Setup(s => s.GetAll(5, 1)).Returns(mockViewModel);

        // Act
        var result = _controller.Index(5, 1) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("RookiesTable"));
            Assert.That(result.Model, Is.EqualTo(mockViewModel));
        });
    }

    [Test]
    public void MalePerson_ReturnsViewWithFilteredModel()
    {
        // Arrange
        var mockViewModel = new PersonViewModel
        {
            People = new List<Person>(),
            PageSize = 5,
            PageIndex = 1,
            TotalPage = 1
        };
        _mockPersonService.Setup(s => s.GetByFilter(It.Is<Func<Person, bool>>(
            f => f(new Person { Gender = GenderType.Male }))
            , 5, 1)).Returns(mockViewModel);

        // Act
        var result = _controller.MalePerson(5, 1) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("RookiesTable"));
            Assert.That(result.Model, Is.EqualTo(mockViewModel));
        });
    }

    [Test]
    public void GetOldestPerson_ReturnsViewWithOldestPerson()
    {
        // Arrange
        var oldestPerson = new PersonViewModel
        {
            People = new List<Person>(),
            PageSize = 5,
            PageIndex = 1,
            TotalPage = 1
        };
        _mockPersonService.Setup(s => s.GetOldestPerson()).Returns(oldestPerson);

        // Act
        var result = _controller.GetOldestPerson() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("RookiesTable"));
            Assert.That(result.Model, Is.EqualTo(oldestPerson));
        });
    }

    [Test]
    public void Create_ReturnsFormPersonView()
    {
        // Act
        var viewResult = _controller.Create() as ViewResult;

        // Assert
        Assert.That(viewResult, Is.Not.Null);
        Assert.That(viewResult.ViewName, Is.EqualTo("FormPerson"));
    }

    [Test]
    public void Edit_ReturnsFormPersonViewWithPersonModel()
    {
        // Arrange
        var personId = 1;
        var person = new Person { Id = personId, FirstName = "John", LastName = "Doe" };
        _mockPersonService.Setup(s => s.GetById(personId)).Returns(person);
        // Act
        var result = _controller.Update(personId) as ViewResult;
        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ViewName, Is.EqualTo("FormPerson"));
            Assert.That(result.Model, Is.EqualTo(person));
        }
    }

    [Test]
    public void ExportExcel_ReturnsFileResult()
    {
        // Arrange
        var mockStream = new MemoryStream();
        _mockPersonService.Setup(s => s.ExportToExcel()).Returns(mockStream);

        // Act
        var result = _controller.ExportExcel() as FileResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(result.FileDownloadName, Is.EqualTo("Rookies.xlsx"));
        }
    }

    [Test]
    public void Create_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "Hung",
            LastName = "Hoang",
            DateOfBirth = new DateTime(2000, 12, 03),
            BirthPlace = "Cao Bang",
            Gender = GenderType.Male,
            PhoneNumber = "0123456789"
        };

        // Act
        var result = _controller.Create(person) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Index"));
        _mockPersonService.Verify(s => s.Create(person), Times.Once);
    }

    [Test]
    public void Create_InvalidModel_ReturnsFormPersonView()
    {
        // Arrange
        var person = new Person();
        _controller.ModelState.AddModelError("FirstName", "First name is required");

        // Act
        var result = _controller.Create(person) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("FormPerson"));
            Assert.That(result.Model, Is.EqualTo(person));
        });
    }

    [Test]
    public void Update_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "Hung",
            LastName = "Hoang",
            DateOfBirth = new DateTime(2000, 12, 03),
            BirthPlace = "Cao Bang",
            Gender = GenderType.Male,
            PhoneNumber = "0123456789"
        };

        // Act
        var result = _controller.Update(person) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Index"));
        _mockPersonService.Verify(s => s.Update(person), Times.Once);
    }

    [Test]
    public void Update_InvalidModel_ReturnsFormPersonView()
    {
        // Arrange
        var person = new Person();
        _controller.ModelState.AddModelError("FirstName", "First name is required");
        // Act
        var result = _controller.Update(person) as ViewResult;
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("FormPerson"));
            Assert.That(result.Model, Is.EqualTo(person));
        });
    }

    [Test]
    public void Delete_ValidId_ReturnDeleteConfirmationView()
    {
        // Arrange
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        _mockPersonService.Setup(s => s.Delete(1)).Returns(person);
        // Act
        var result = _controller.Delete(1) as ViewResult;
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ViewName, Is.EqualTo("DeleteConfirmation"));
        Assert.That("John Doe", Is.EqualTo(_controller.ViewBag.UserName));
        _mockPersonService.Verify(s => s.Delete(1), Times.Once);
    }
    [Test]
    public void Delete_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var personId = 999;
        _mockPersonService.Setup(s => s.Delete(personId)).Throws(new NotFoundException($"Person with id {personId} not found"));
        // Act
        var result = _controller.Delete(personId) as NotFoundObjectResult;
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
    }
    [Test]
    public void GetFullName_ReturnsOkResult_WithListOfFullNames()
    {
        // Arrange
        var mockFullNames = new List<string> { "John Doe", "Jane Smith" };
        _mockPersonService
            .Setup(s => s.GetPeopleFullName())
            .Returns(mockFullNames);

        // Act
        var result = _controller.GetFullName();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(mockFullNames));
        });
    }
    [Test]
    public void RedirectByYear_ActionIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = _controller.RedirectByYear("");

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
            Assert.That(badRequest.Value, Is.EqualTo("Action parameter is required."));
        });
    }

    [Test]
    public void RedirectByYear_ActionIsLessThan_RedirectsToCorrectAction()
    {
        // Act
        var result = _controller.RedirectByYear("lessthan");

        // Assert
        var redirect = result as RedirectToActionResult;
        Assert.That(redirect, Is.Not.Null);
        Assert.That(redirect.ActionName, Is.EqualTo("PersonBirthYearLess2000"));
    }

    [Test]
    public void RedirectByYear_ActionIsEqual_RedirectsToCorrectAction()
    {
        // Act
        var result = _controller.RedirectByYear("equal");

        // Assert
        var redirect = result as RedirectToActionResult;
        Assert.That(redirect, Is.Not.Null);
        Assert.That(redirect.ActionName, Is.EqualTo("PersonBirthYearEqual2000"));
    }

    [Test]
    public void RedirectByYear_ActionIsGreaterThan_RedirectsToCorrectAction()
    {
        // Act
        var result = _controller.RedirectByYear("greaterthan");

        // Assert
        var redirect = result as RedirectToActionResult;
        Assert.That(redirect, Is.Not.Null);
        Assert.That(redirect.ActionName, Is.EqualTo("PersonBirthYearGreater2000"));
    }

    [Test]
    public void RedirectByYear_InvalidAction_ReturnsBadRequest()
    {
        // Act
        var result = _controller.RedirectByYear("random");

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
            Assert.That(badRequest.Value, Is.EqualTo("Invalid action parameter. Valid values are 'lessthan', 'equal', 'greaterthan'."));
        });
    }

    [Test]
    public void PersonBirthYearLess2000_Valid_ReturnsCorrectViewAndData()
    {
        // Arrange
        var mockPeople = new PersonViewModel
        {
            People = [
                new Person { FirstName = "John", LastName = "Hoang", DateOfBirth = new DateTime(1995, 1, 1) },
                new Person { FirstName = "Jane", LastName = "Hoang", DateOfBirth = new DateTime(1988, 5, 10) }
            ],
            PageSize = 5,
            PageIndex = 1,
            TotalPage = 1
        };
        _mockPersonService
            .Setup(service => service.GetByFilter(It.Is<Func<Person, bool>>(
                f => f(new Person { DateOfBirth = new DateTime(1995, 1, 1) }) 
            && !f(new Person { DateOfBirth = new DateTime(2001, 1, 1) })), 5, 1))
            .Returns(mockPeople);

        // Act
        var result = _controller.PersonBirthYearLess2000() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("RookiesTable"));
            Assert.That(result.Model, Is.EqualTo(mockPeople));
            Assert.That("PersonBirthYearLess2000", Is.EqualTo(_controller.ViewBag.Action));
        });
    }

    [Test]
    public void PersonBirthYearGreater2000_Valid_ReturnsCorrectViewAndData()
    {
        // Arrange
        var mockPeople = new PersonViewModel
        {
            People = [
                new Person { FirstName = "John", DateOfBirth = new DateTime(2005, 1, 1) },
                new Person { FirstName = "Jane", DateOfBirth = new DateTime(2008, 5, 10) }
            ],
            PageSize = 5,
            PageIndex = 1,
            TotalPage = 1
        };
        _mockPersonService
            .Setup(service => service.GetByFilter(It.Is<Func<Person, bool>>(f =>
                f(new Person { DateOfBirth = new DateTime(2005, 1, 1) }) &&  // true: year < 2000
                f(new Person { DateOfBirth = new DateTime(2008, 5, 10) })     // false: year > 2000
            ), 5, 1))
            .Returns(mockPeople);

        // Act
        var result = _controller.PersonBirthYearGreater2000() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("RookiesTable"));
            Assert.That(result.Model, Is.EqualTo(mockPeople));
            Assert.That("PersonBirthYearGreater2000", Is.EqualTo(_controller.ViewBag.Action));
        });
    }

    [Test]
    public void PersonBirthYearEqual2000_Valid_ReturnsCorrectViewAndData()
    {
        // Arrange
        var mockPeople = new PersonViewModel
        {
            People = [
                new Person { FirstName = "John", DateOfBirth = new DateTime(2000, 1, 1) },
                new Person { FirstName = "Jane", DateOfBirth = new DateTime(2000, 5, 10) }
            ],
            PageSize = 5,
            PageIndex = 1,
            TotalPage = 1
        };
        _mockPersonService
            .Setup(service => service.GetByFilter(It.Is<Func<Person, bool>>(f =>
                f(new Person { DateOfBirth = new DateTime(2000, 1, 1) }) &&  
                f(new Person { DateOfBirth = new DateTime(2000, 5, 10) })     
            ), 5, 1))
            .Returns(mockPeople);

        // Act
        var result = _controller.PersonBirthYearEqual2000() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ViewName, Is.EqualTo("RookiesTable"));
            Assert.That(result.Model, Is.EqualTo(mockPeople));
            Assert.That("PersonBirthYearEqual2000", Is.EqualTo(_controller.ViewBag.Action));
        });
    }
}
