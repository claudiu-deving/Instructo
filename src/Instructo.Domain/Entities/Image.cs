using Domain.Dtos.Image;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Image : BaseAuditableEntity<ImageId>
{
    public FileName FileName { get; private set; }
    public ContentType ContentType { get; private set; }
    public Url Url { get; private set; }
    public string? Description { get; private set; }
    private Image()
    {
    }


    public static Result<Image> Create(string fileName, string contentType, string url, string? description)
    {
        var image = new Image();
        var errors = new List<Error>();
        var fileNameResult = FileName.Create(fileName)
            .OnSuccess(fileName => image.FileName=fileName)
            .OnError(errors.AddRange);
        var contentTypeResult = ContentType.Create(contentType)
            .OnSuccess(contentType => image.ContentType=contentType)
            .OnError(errors.AddRange);
        var urlResult = Url.Create(url)
            .OnSuccess(url => image.Url=url)
            .OnError(errors.AddRange);
        image.Description=description;
        image.Id=ImageId.CreateNew();
        if(errors.Count>0)
            return Result<Image>.Failure([.. errors]);
        return image;
    }

    public ImageReadDto ToDto()
    {
        return new ImageReadDto
        {
            FileName=FileName,
            Url=Url,
            ContentType=ContentType,
            Description=Description??""
        };
    }
}
