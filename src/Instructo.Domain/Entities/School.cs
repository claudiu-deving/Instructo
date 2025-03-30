using System.Collections.Generic;
using System.Numerics;

using Instructo.Domain.Enums;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;
using Instructo.Shared;

namespace Instructo.Domain.Entities;

public sealed class School
{

    public School(SchoolId id)
    {
    }
}

public class User
{
    public User()
    {

    }
    public User(UserId id, string firstName,string lastName, string email, string role)
    {
        Id=id;
        FirstName=firstName;
        LastName=lastName;
        Email=email;
        Role=role;
    }

    public UserId? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get;  set; }
    public string? Email { get; set; }
    public string? Role { get; set; }

}