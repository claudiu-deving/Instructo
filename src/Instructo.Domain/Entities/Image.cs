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


    public Result<Image> Update(string? fileName, string? contentType, string? url, string? description)
    {
        var errors = new List<Error>();
        if (fileName is not null)
        {
            var fileNameResult = FileName.Create(fileName)
                .OnSuccess(name => this.FileName=name)
                .OnError(errors.AddRange);
        }
        if (contentType is not null)
        {
            var contentTypeResult = ContentType.Create(contentType)
                .OnSuccess(type => this.ContentType=type)
                .OnError(errors.AddRange);
        }

        if (url is not null)
        {
            var urlResult = Url.Create(url)
                .OnSuccess(link => this.Url=link)
                .OnError(errors.AddRange);
        }

        if (description is not null)
        {
            this.Description = description;
        }
        return errors.Count>0 ? Result<Image>.Failure([.. errors]) : this;
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
        return errors.Count>0 ? Result<Image>.Failure([.. errors]) : image;
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
