using System.Data;
using Assignment2.Models;
using ClosedXML.Excel;
using Assignment2.ViewModels;
using Assignment.CustomException;
namespace Assignment2.Services;

public class PersonService : IPersonService
{
    public List<Person> _people =
    [
        new Person { Id = 1, FirstName = "John", LastName = "Doe", Gender = GenderType.Male, DateOfBirth = new DateTime(1995, 5, 23), PhoneNumber = "0587960813", BirthPlace = "New York", IsGraduated = true },
        new Person { Id = 2, FirstName = "Jane", LastName = "Smith", Gender = GenderType.Female, DateOfBirth = new DateTime(1998, 8, 14), PhoneNumber = "0709747227", BirthPlace = "Los Angeles", IsGraduated = false },
        new Person { Id = 3, FirstName = "Alice", LastName = "Johnson", Gender = GenderType.Other, DateOfBirth = new DateTime(2000, 1, 10), PhoneNumber = "0326881162", BirthPlace = "Chicago", IsGraduated = true },
        new Person { Id = 4, FirstName = "Bob", LastName = "Brown", Gender = GenderType.Male, DateOfBirth = new DateTime(1992, 12, 30), PhoneNumber = "0825493289", BirthPlace = "Houston", IsGraduated = false },
        new Person { Id = 5, FirstName = "Charlie", LastName = "Davis", Gender = GenderType.Male, DateOfBirth = new DateTime(1990, 3, 15), PhoneNumber = "0376781592", BirthPlace = "San Francisco", IsGraduated = true },
        new Person { Id = 6, FirstName = "Emily", LastName = "White", Gender = GenderType.Female, DateOfBirth = new DateTime(1997, 7, 21), PhoneNumber = "0949972803", BirthPlace = "Miami", IsGraduated = false },
        new Person { Id = 7, FirstName = "Frank", LastName = "Green", Gender = GenderType.Male, DateOfBirth = new DateTime(1985, 11, 2), PhoneNumber = "0852080128", BirthPlace = "Seattle", IsGraduated = true },
        new Person { Id = 8, FirstName = "Grace", LastName = "Hall", Gender = GenderType.Female, DateOfBirth = new DateTime(1993, 6, 18), PhoneNumber = "0706578826", BirthPlace = "Boston", IsGraduated = false },
        new Person { Id = 9, FirstName = "Henry", LastName = "Moore", Gender = GenderType.Other, DateOfBirth = new DateTime(1988, 9, 5), PhoneNumber = "0378651840", BirthPlace = "Denver", IsGraduated = true },
        new Person { Id = 10, FirstName = "Isabella", LastName = "Clark", Gender = GenderType.Female, DateOfBirth = new DateTime(2001, 4, 12), PhoneNumber = "0823582921", BirthPlace = "Austin", IsGraduated = false },
        new Person { Id = 11, FirstName = "John", LastName = "Doe", Gender = GenderType.Male, DateOfBirth = new DateTime(1995, 5, 23), PhoneNumber = "0587960813", BirthPlace = "New York", IsGraduated = true },
        new Person { Id = 12, FirstName = "Jane", LastName = "Smith", Gender = GenderType.Female, DateOfBirth = new DateTime(1998, 8, 14), PhoneNumber = "0709747227", BirthPlace = "Los Angeles", IsGraduated = false },
        new Person { Id = 13, FirstName = "Alice", LastName = "Johnson", Gender = GenderType.Other, DateOfBirth = new DateTime(2000, 1, 10), PhoneNumber = "0326881162", BirthPlace = "Chicago", IsGraduated = true },
        new Person { Id = 14, FirstName = "Bob", LastName = "Brown", Gender = GenderType.Male, DateOfBirth = new DateTime(1992, 12, 30), PhoneNumber = "0825493289", BirthPlace = "Houston", IsGraduated = false },
        new Person { Id = 15, FirstName = "Charlie", LastName = "Davis", Gender = GenderType.Male, DateOfBirth = new DateTime(1990, 3, 15), PhoneNumber = "0376781592", BirthPlace = "San Francisco", IsGraduated = true },
        new Person { Id = 16, FirstName = "Emily", LastName = "White", Gender = GenderType.Female, DateOfBirth = new DateTime(1997, 7, 21), PhoneNumber = "0949972803", BirthPlace = "Miami", IsGraduated = false },
        new Person { Id = 17, FirstName = "Frank", LastName = "Green", Gender = GenderType.Male, DateOfBirth = new DateTime(1985, 11, 2), PhoneNumber = "0852080128", BirthPlace = "Seattle", IsGraduated = true },
        new Person { Id = 18, FirstName = "Grace", LastName = "Hall", Gender = GenderType.Female, DateOfBirth = new DateTime(1993, 6, 18), PhoneNumber = "0706578826", BirthPlace = "Boston", IsGraduated = false },
        new Person { Id = 19, FirstName = "Henry", LastName = "Moore", Gender = GenderType.Other, DateOfBirth = new DateTime(1988, 9, 5), PhoneNumber = "0378651840", BirthPlace = "Denver", IsGraduated = true },
        new Person { Id = 20, FirstName = "Isabella", LastName = "Clark", Gender = GenderType.Female, DateOfBirth = new DateTime(2001, 4, 12), PhoneNumber = "0823582921", BirthPlace = "Austin", IsGraduated = false }
    ];

    public PersonViewModel GetAll(int pageSize, int pageIndex)
    {
        var personViewModel = new PersonViewModel
        {
            People = _people.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPage = (int)Math.Ceiling((double)_people.Count / pageSize)
        };

        return personViewModel;
    }

    public PersonViewModel GetByFilter(Func<Person, bool> filter, int pageSize, int pageIndex)
    {
        var personViewModel = new PersonViewModel
        {
            People = _people.Where(filter).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPage = (int)Math.Ceiling((double)_people.Count / pageSize)
        };

        return personViewModel;
    }

    public PersonViewModel GetOldestPerson()
    {
        var personViewModel = new PersonViewModel
        {
            People = [_people.OrderBy(p => p.DateOfBirth).First()],
            PageSize = 1,
            PageIndex = 1,
            TotalPage = 1
        };

        return personViewModel;
    }

    public List<string> GetPeopleFullName()
    {
        return _people.Select(p => p.FullName).ToList();
    }

    public Stream ExportToExcel()
    {
        DataTable dt = new("Rookies");
        dt.Columns.AddRange(
        [
            new DataColumn("ID", typeof(int)),
            new DataColumn("First Name", typeof(string)),
            new DataColumn("Last Name", typeof(string)),
            new DataColumn("Gender", typeof(string)),
            new DataColumn("DateOfBirth", typeof(DateTime)),
            new DataColumn("PhoneNumber", typeof(string)),
            new DataColumn("BirthPlace", typeof(string)),
            new DataColumn("IsGraduated", typeof(bool)),
        ]);
        foreach (var person in _people)
        {
            dt.Rows.Add(person.Id, person.FirstName, person.LastName, 
                person.Gender, person.DateOfBirth, person.PhoneNumber,
                person.BirthPlace, person.IsGraduated);
        }
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(dt, "Rookies");

        // Adjust column widths
        worksheet.Columns().AdjustToContents();

        // Save to MemoryStream
        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }

    public Person GetById(int id)
    {
        return _people.FirstOrDefault(p => p.Id == id) ?? throw new NotFoundException($"Person with id {id} not found");
    }

    public void Create(Person person)
    {
        person.Id = _people.Max(p => p.Id) + 1;
        _people.Insert(0, person);
    }

    public int Update(Person person)
    {
        var index = _people.FindIndex(p => p.Id == person.Id);
        if (index == -1)
        {
            throw new NotFoundException($"Person with id {person.Id} not found");
        }
        _people[index] = person;

        return index;
    }

    public Person Delete(int id)
    {
        var person = _people.FirstOrDefault(p => p.Id == id) ?? throw new NotFoundException($"Person with id {id} not found");
        _people.Remove(person);

        return person;
    }
}