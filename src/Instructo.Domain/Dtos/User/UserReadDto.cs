namespace Domain.Dtos.User;
public class UserReadDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
public class UserReadSuperDto : UserReadDto
{
    public DateTime CreatedUtc { get; set; }
    public DateTime LastLoginUtc { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsPhoneConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}
