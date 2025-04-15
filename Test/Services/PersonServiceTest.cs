using Assignment2.Services;
using Assignment2.Models;
using Assignment.CustomException;
using System.Collections;
using ClosedXML.Excel;

namespace Test.Services
{
    [TestFixture]
    public class PersonServiceTests
    {
        private PersonService _personService;

        [SetUp]
        public void Setup()
        {
            _personService = new PersonService();
        }

        [TestCase(5, 1, 5, 4)]
        [TestCase(10, 2, 10, 2)]
        [TestCase(7, 3, 6, 3)]
        public void GetAll_Valid_ReturnsCorrectPage(int pageSize, int pageIndex, int count, int totalPage)
        {
            // Act
            var result = _personService.GetAll(pageSize, pageIndex);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.People, Has.Count.EqualTo(count));
                Assert.That(result.TotalPage, Is.EqualTo(totalPage)); // Total people = 20, page size = 5
                Assert.That(result.PageIndex, Is.EqualTo(pageIndex));
            });
        }

        [Test]
        [TestCaseSource(nameof(FilterTestCase))]
        public void GetByFilter_Valid_ReturnsFilteredResults(Func<Person, bool> filter)
        {
            // Act
            var result = _personService.GetByFilter(filter, 5, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.People.All(filter), Is.True);
                Assert.That(result.PageIndex, Is.EqualTo(1));
            });
        }

        [Test]
        public void GetByFilter_NoMatch_ReturnsEmpty()
        {
            // Act
            var result = _personService.GetByFilter(p => p.FirstName == "NonExistent", 5, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.People, Is.Empty);
        }

        [Test]
        public void GetOldestPerson_Valid_ReturnsOldestPerson()
        {
            // Act
            var result = _personService.GetOldestPerson();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.People, Has.Count.EqualTo(1));
            Assert.That(result.People[0].FirstName, Is.EqualTo("Frank"));
        }

        [Test]
        public void GetPeopleFullName_Valid_ReturnsAllFullNames()
        {
            // Act
            var result = _personService.GetPeopleFullName();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(20)); // Total people = 20
                Assert.That(result, Does.Contain("John Doe"));
                Assert.That(result, Does.Contain("Jane Smith"));
            });
        }

        [Test]
        public void ExportToExcel_ReturnsNonEmptyStream()
        {
            // Act
            var result = _personService.ExportToExcel();
            using var workbook = new XLWorkbook(result);
            var worksheet = workbook.Worksheet("Rookies");
            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(worksheet, Is.Not.Null);
                Assert.That(result, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(worksheet.Cell(1, 1).Value.ToString(), Is.EqualTo("ID"));
                Assert.That(worksheet.Cell(1, 2).Value.ToString(), Is.EqualTo("First Name"));
                Assert.That(worksheet.Cell(2, 2).Value.ToString(), Is.EqualTo("John"));
                Assert.That(result, Is.InstanceOf<Stream>());
                Assert.That(result.Length, Is.GreaterThan(0));
            });
        }

        [Test]
        public void GetById_ReturnsCorrectPerson()
        {
            // Act
            var result = _personService.GetById(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(1));
                Assert.That(result.FirstName, Is.EqualTo("John"));
            });
        }

        [Test]
        public void GetById_ThrowsException_WhenPersonNotFound()
        {
            // Act & Assert
            Assert.Throws<NotFoundException>(() => _personService.GetById(999));
        }

        [Test]
        public void Create_AddsNewPerson()
        {
            // Arrange
            var newPerson = new Person
            {
                FirstName = "Hung",
                LastName = "Hoang",
                Gender = GenderType.Male,
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "0123456789",
                BirthPlace = "Cao Bang",
                IsGraduated = false
            };

            // Act
            _personService.Create(newPerson);

            // Assert
            Assert.That(newPerson, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(newPerson.FirstName, Is.EqualTo("Hung"));
                Assert.That(newPerson.LastName, Is.EqualTo("Hoang"));
                Assert.That(newPerson.Id, Is.EqualTo(21)); // Assuming ID is auto-incremented
            });
        }

        [Test]
        public void Update_UpdatesExistingPerson()
        {
            // Arrange
            var personToUpdate = new Person
            {
                Id = 2,
                FirstName = "Updated",
            };

            // Act
            var result = _personService.Update(personToUpdate);

            // Assert
            Assert.That(result, Is.Not.EqualTo(-1)); // Index of the updated person
            Assert.That(personToUpdate.FirstName, Is.EqualTo("Updated"));
        }

        [Test]
        public void Update_ThrowException_WhenPersonNotFound()
        {
            // Arrange
            var nonExistentPerson = new Person
            {
                Id = 999,
                FirstName = "NonExistent"
            };

            // Act
            var ex = Assert.Throws<NotFoundException>(() => _personService.Update(nonExistentPerson));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Person with id 999 not found"));
        }

        [Test]
        public void Delete_RemovesPerson()
        {
            // Act
            var deletedPerson = _personService.Delete(1);

            // Assert
            Assert.That(deletedPerson, Is.Not.Null);
            Assert.That(deletedPerson.Id, Is.EqualTo(1));
            Assert.Throws<NotFoundException>(() => _personService.GetById(1));
        }

        [Test]
        public void Delete_ThrowsNotFoundException_WhenPersonDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<NotFoundException>(() => _personService.Delete(999));
        }
        private static IEnumerable FilterTestCase()
        {
            yield return new TestCaseData
            (
                new Func<Person, bool>(p => p.Gender == GenderType.Male)
            );
            yield return new TestCaseData
            (
                new Func<Person, bool>(p => p.Gender == GenderType.Female)
            );
        }
    }
}