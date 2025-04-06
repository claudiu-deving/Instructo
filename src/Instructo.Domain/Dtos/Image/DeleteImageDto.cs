using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Dtos.Image;

public readonly record struct DeleteImageDto(int ImageId);
public readonly record struct CreateImageDto(string FileName, string ContentType, string Url, string? Description = null);
public readonly record struct UpdateImageDto(int ImageId, string FileName, string ContentType, string Url, string? Description = null);
