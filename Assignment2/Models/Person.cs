namespace Assignment2.Models;
using System.ComponentModel.DataAnnotations;

public enum GenderType
{
    Male,
    Female,
    Other
}

public class Person
{
    public int Id { get; set; }
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    [Required(ErrorMessage = "Gender is required.")]
    public GenderType Gender { get; set; } = GenderType.Male;
    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    public string DateOfBirthString => DateOfBirth.ToString("dd/MM/yyyy");
    [RegularExpression(@"0(3|5|7|8|9)[0-9]{8}$", ErrorMessage = "Invalid Vietnamese phone number format.")]
    [Required(ErrorMessage = "Phone number is required.")]
    public string PhoneNumber { get; set; } = string.Empty;
    [MaxLength(100, ErrorMessage = "Birthplace cannot exceed 100 characters.")]
    public string BirthPlace { get; set; } = string.Empty;
    public bool IsGraduated { get; set; } = false;
    public string GraduatedString => IsGraduated ? "Yes" : "No";
}