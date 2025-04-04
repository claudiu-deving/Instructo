using Instructo.Application.Abstractions.Messaging;
using Instructo.Application.Users.Commands.RegisterUser;
using Instructo.Domain.Dtos;
using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

using MediatR;

namespace Instructo.Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandHandler(
    ICommandRepository<School, SchoolId> repository,
    ICommandRepository<Image, ImageId> imageRepository,
    IIdentityService identityService,
    ISender sender)
    : ICommandHandler<CreateSchoolCommand, Result<SchoolReadDto>>
{
    public async Task<Result<SchoolReadDto>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        var registerUserCommand = new RegisterUserCommand(
            request.OwnerFirstName,
            request.OwnerLastName,
            request.OwnerEmail,
            request.OwnerPassword,
            request.PhoneNumber,
            "Owner");
        var userRegistrationRequest = await sender.Send(
            registerUserCommand, cancellationToken);
        if(userRegistrationRequest.IsError)
        {
            return Result<SchoolReadDto>.Failure(userRegistrationRequest.Errors);
        }
        var user = await identityService.GetUserByEmailAsync(request.OwnerEmail);
        if(user is null)
        {
            return Result<SchoolReadDto>.Failure([new Error(
                "Create-School", 
                "User retrieval failed after registration")]);
        }
        var image = Image.Create(
            $"{request.CompanyName}-Icon", 
            request.ImageContentType, 
            request.ImagePath,
            $"Company logo");

        var school = new School(
            user,
            new SchoolName(request.Name),
            new CompanyName(request.CompanyName), image);
        school.AddLink(new WebsiteLink(
            "https://www.instructo.com",
            "Instructo",
            "Instructo website",
            image));
        var repositoryRequest = await repository.AddAsync(school);
        return repositoryRequest.Match(
             ok =>
             {
                 if(ok is null)
                 {
                     return Result<SchoolReadDto>.Failure(
                         [new Error("Create-School", "School creation failed")]);
                 }
                 return Result<SchoolReadDto>.Success(new SchoolReadDto
                 {
                     Id=ok.Id.Id,
                     Name=ok.Name,
                     CompanyName=ok.CompanyName,
                     Email=ok.Email??"",
                     PhoneNumber=ok.PhoneNumber??"",
                     Links= [.. ok.WebsiteLinks.Select(x => x.ToDto())],
                     IconData=ok.Icon?.ToDto()??new ImageReadDto(),
                 });
             },
             Result<SchoolReadDto>.Failure);
    }
}