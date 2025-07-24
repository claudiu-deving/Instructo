using System.ComponentModel.DataAnnotations.Schema;

using Domain.ValueObjects;

namespace Domain.Entities.SchoolEntities;

public class InstructorProfile : BaseAuditableEntity<Guid>
{
    private readonly List<VehicleCategory> _vehicleCategories = [];

    private InstructorProfile(
        string firstName,
        string lastName,
        int birthYear,
        int yearsExperience,
        string specialization,
        string description,
        string phone,
        string email,
        string gender,
        Image? profileImage,
        List<VehicleCategory> vehicleCategories)
    {
        Id=Guid.NewGuid();
        FirstName=firstName;
        LastName=lastName;
        BirthYear=birthYear;
        YearsExperience=yearsExperience;
        Specialization=specialization;
        Description=description;
        Phone=phone;
        Email=email;
        ProfileImage=profileImage;
        _vehicleCategories.AddRange(vehicleCategories);
        Gender=gender;
    }

    private InstructorProfile() { }

    [ForeignKey("Team")]
    public Guid TeamId { get; private set; }

    public Team Team { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public int BirthYear { get; private set; }
    public int YearsExperience { get; private set; }
    public string Specialization { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Gender { get; private set; } = string.Empty;
    public virtual Image? ProfileImage { get; private set; }

    [ForeignKey("Images")]
    public ImageId ProfileImageId { get; private set; }

    public virtual IReadOnlyCollection<VehicleCategory> VehicleCategories => _vehicleCategories.AsReadOnly();

    public void AddVehicleCategory(VehicleCategory vehicleCategory)
    {
        if(_vehicleCategories.Contains(vehicleCategory))
            return;
        _vehicleCategories.Add(vehicleCategory);
    }

    public bool RemoveVehicleCategory(VehicleCategory vehicleCategory)
    {
        if(!_vehicleCategories.Contains(vehicleCategory))
            return false;
        _vehicleCategories.Remove(vehicleCategory);
        return true;
    }

    public void UpdateProfile(
        string? firstName,
        string? lastName,
        int? birthYear,
        int? yearsExperience,
        string? specialization,
        string? description,
        string? phone,
        string? email,
        Image? profileImage)
    {
        if(firstName is not null)
            FirstName=firstName;
        if(lastName is not null)
            LastName=lastName;
        if(birthYear is not null)
            BirthYear=birthYear.Value;
        if(yearsExperience is not null)
            YearsExperience=yearsExperience.Value;
        if(specialization is not null)
            Specialization=specialization;
        if(description is not null)
            Description=description;
        if(phone is not null)
            Phone=phone;
        if(email is not null)
            Email=email;
        if(profileImage is not null)
            ProfileImage=profileImage;
    }

    public static InstructorProfile Create(
        string firstName,
        string lastName,
        int birthYear,
        int yearsExperience,
        string specialization,
        string? description,
        string? phone,
        string? email,
        string gender,
        Image? profileImage,
        List<VehicleCategory> vehicleCategories)
    {
        return new InstructorProfile(
            firstName,
            lastName,
            birthYear,
            yearsExperience,
            specialization,
            description,
            phone,
            email,
            gender,
            profileImage,
            vehicleCategories);
    }
}