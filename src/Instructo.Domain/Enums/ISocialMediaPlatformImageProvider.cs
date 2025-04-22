using Domain.Shared;

namespace Domain.Enums;

public interface ISocialMediaPlatformImageProvider
{
    Result<SocialMediatPlatform> Get(string platform);
}