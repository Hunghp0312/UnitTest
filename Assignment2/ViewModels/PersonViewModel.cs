using Assignment2.Models;

namespace Assignment2.ViewModels;

public class PersonViewModel
{
    public List<Person> People { get; set; } = new();
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public int TotalPage { get; set; }
}