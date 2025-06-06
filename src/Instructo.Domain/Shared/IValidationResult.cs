﻿namespace Domain.Shared;

interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "ValidationError",
        "A validation problem occured");

    Error[] Errors { get; }
}
