namespace Domain.Dtos.School;

public readonly record struct UpdateApprovalStatusCommandDto(Guid Id, bool IsApproved);