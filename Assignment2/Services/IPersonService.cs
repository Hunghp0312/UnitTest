using Assignment2.Models;
using Assignment2.ViewModels;
namespace Assignment2.Services;

public interface IPersonService
{
    PersonViewModel GetAll(int pageSize, int pageIndex);
    PersonViewModel GetByFilter(Func<Person, bool> filter, int pageSize, int pageIndex);
    PersonViewModel GetOldestPerson();
    List<string> GetPeopleFullName();
    Stream ExportToExcel();
    Person? GetById(int id);
    void Create(Person person);
    int Update(Person person);
    Person Delete(int id);
}
