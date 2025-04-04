using Instructo.Domain.Dtos;
using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Entities;

public class Image : BaseAuditableEntity<ImageId>
{
    private Image() { }
    public FileName FileName { get; private set; }
    public ContentType ContentType { get; private set; }
    public Url Url { get; private set; }
    public string? Description { get; private set; }
    public Image(string fileName, string contentType, string url, string? description)
    {
        Id=ImageId.CreateNew();
        FileName=fileName;
        ContentType=contentType;
        Url=url;
        Description=description;
    }


    public static Image Create(string fileName, string contentType, string url, string? description)
    {
        return new Image(fileName, contentType, url, description);
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
