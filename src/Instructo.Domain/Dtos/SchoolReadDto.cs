using Instructo.Domain.Entities;

namespace Instructo.Domain.Dtos;

public readonly record struct SchoolReadDto(Guid Id, string Name, string CompanyName, string Email, string PhoneNumber, ImageReadDto IconData, WebsiteLinkReadDto[] Links);
