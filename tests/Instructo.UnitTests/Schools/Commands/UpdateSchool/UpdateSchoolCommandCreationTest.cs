﻿using Application.Schools.Commands.UpdateSchool;
using Domain.Dtos.School;
using Domain.Shared;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit.Sdk;

namespace Instructo.UnitTests.Schools.Commands.UpdateSchool;

[TestSubject(typeof(UpdateSchoolCommand))]
public class UpdateSchoolCommandCreationTest
{

    [Fact]
    public void CreateUpdateSchoolCommandFromValidDto_ReturnsUpdateCommand()
    {
        var validUpdateSchoolDto = new UpdateSchoolCommandDto(){
           OwnerEmail = "Owner@email.com",
            Address = "Address"
            };
        var updateSchoolCommand = UpdateSchoolCommand.Create(validUpdateSchoolDto,Guid.NewGuid());
        if (updateSchoolCommand.IsError)
        {
            updateSchoolCommand.IsError.Should()
                .BeFalse($"{string.Join(Environment.NewLine, updateSchoolCommand.Errors.ToList())}");
        }
        updateSchoolCommand.Value.Should().NotBeNull();
        updateSchoolCommand.Value!.Address?.Value.Should().Be("Address");
        updateSchoolCommand.Value!.Name.Should().BeNull();
    }
}