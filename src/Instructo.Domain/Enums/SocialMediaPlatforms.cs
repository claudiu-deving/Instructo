using System.Globalization;

using Domain.Shared;

namespace Domain.Enums;

public enum SocialMediaPlatforms
{
    Facebook = 1,
    Instagram = 2,
    Twitter = 3,
    LinkedIn = 4,
    TikTok = 5,
    YouTube = 6,
    Snapchat = 7,
    Pinterest = 8,
    Reddit = 9,
    Tumblr = 10,
    WhatsApp = 11,
    Telegram = 12,
    Discord = 13,
    Twitch = 14
}
public readonly record struct SocialMediatPlatform(
    string IconPath,
    string IconContentType,
    SocialMediaPlatforms Platform,
    string Description);

public class SocialMediaPlatformImageProvider : ISocialMediaPlatformImageProvider
{
    private readonly Dictionary<string, SocialMediatPlatform> socialMediaPlatformImages;

    public SocialMediaPlatformImageProvider()
    {
        // Initialize any static resources if needed
        socialMediaPlatformImages= [];
        foreach(var platform in Enum.GetValues<SocialMediaPlatforms>())
        {
            switch(platform)
            {
                case SocialMediaPlatforms.Facebook:
                    socialMediaPlatformImages.Add("Facebook", new SocialMediatPlatform("image/png", "./assets/images/general/facebook.png", SocialMediaPlatforms.Facebook, GetDescription(SocialMediaPlatforms.Facebook, new CultureInfo("en"))));
                    break;
                case SocialMediaPlatforms.Instagram:
                    socialMediaPlatformImages.Add("Instagram", new SocialMediatPlatform("image/png", "./assets/images/general/instagram.png", SocialMediaPlatforms.Instagram, GetDescription(SocialMediaPlatforms.Instagram, new CultureInfo("en"))));
                    break;
                case SocialMediaPlatforms.Twitter:
                    socialMediaPlatformImages.Add("Twitter", new SocialMediatPlatform("image/png", "./assets/images/general/twitter.png", SocialMediaPlatforms.Twitter, GetDescription(SocialMediaPlatforms.Twitter, new CultureInfo("en"))));
                    break;
                case SocialMediaPlatforms.LinkedIn:
                    socialMediaPlatformImages.Add("LinkedIn", new SocialMediatPlatform("image/png", "./assets/images/general/linkedin.png", SocialMediaPlatforms.LinkedIn, GetDescription(SocialMediaPlatforms.LinkedIn, new CultureInfo("en"))));
                    break;
            }
        }

    }


    private string GetDescription(SocialMediaPlatforms platform, CultureInfo cultureInfo)
    {
        switch(cultureInfo.Name)
        {
            case "en":
                {
                    switch(platform)
                    {
                        case SocialMediaPlatforms.Facebook:
                            return "Social media platform for connecting with friends and family.";
                        case SocialMediaPlatforms.Instagram:
                            return "Photo and video sharing social media platform.";
                        case SocialMediaPlatforms.Twitter:
                            return "Microblogging platform for sharing short messages.";
                        case SocialMediaPlatforms.LinkedIn:
                            return "Professional networking platform.";
                        case SocialMediaPlatforms.TikTok:
                            return "Short-form video sharing platform.";
                        case SocialMediaPlatforms.YouTube:
                            return "Video sharing platform.";
                        case SocialMediaPlatforms.Snapchat:
                            return "Multimedia messaging app.";
                        case SocialMediaPlatforms.Pinterest:
                            return "Image sharing and social media service.";
                        case SocialMediaPlatforms.Reddit:
                            return "Social news aggregation and discussion website.";
                        case SocialMediaPlatforms.Tumblr:
                            return "Microblogging platform.";
                        case SocialMediaPlatforms.WhatsApp:
                            return "Messaging app for sending text, voice, and video messages.";
                        case SocialMediaPlatforms.Telegram:
                            return "Cloud-based instant messaging service.";
                        case SocialMediaPlatforms.Discord:
                            return "Voice, video, and text chat platform for communities.";
                        case SocialMediaPlatforms.Twitch:
                            return "Live streaming platform for gamers.";
                        default:
                            return "Unknown social media platform.";
                    }
                }
            case "ro":
                {
                    switch(platform)
                    {
                        case SocialMediaPlatforms.Facebook:
                            return "Platform de socializare pentru a te conecta cu prietenii și familia.";
                        case SocialMediaPlatforms.Instagram:
                            return "Platformă de social media pentru partajarea fotografiilor și videoclipurilor.";
                        case SocialMediaPlatforms.Twitter:
                            return "Platformă de microblogging pentru partajarea mesajelor scurte.";
                        case SocialMediaPlatforms.LinkedIn:
                            return "Platformă de networking profesional.";
                        case SocialMediaPlatforms.TikTok:
                            return "Platformă de partajare a videoclipurilor de scurtă durată.";
                        case SocialMediaPlatforms.YouTube:
                            return "Platformă de partajare a videoclipurilor.";
                        case SocialMediaPlatforms.Snapchat:
                            return "Aplicație de mesagerie multimedia.";
                        case SocialMediaPlatforms.Pinterest:
                            return "Serviciu de partajare a imaginilor și social media.";
                        case SocialMediaPlatforms.Reddit:
                            return "Site web de agregare și discuții de știri sociale.";
                        case SocialMediaPlatforms.Tumblr:
                            return "Platformă de microblogging.";
                        case SocialMediaPlatforms.WhatsApp:
                            return "Aplicație de mesagerie pentru trimiterea de mesaje text, vocale și video.";
                        case SocialMediaPlatforms.Telegram:
                            return "Serviciu de mesagerie instantanee bazat pe cloud.";
                        case SocialMediaPlatforms.Discord:
                            return "Platformă de chat vocal, video și text pentru comunități.";
                        case SocialMediaPlatforms.Twitch:
                            return "Platformă de streaming live pentru gameri.";
                        default:
                            return "Platformă de social media necunoscută.";
                    }
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(cultureInfo), cultureInfo, null);
        }
    }

    public Result<SocialMediatPlatform> Get(string platform)
    {
        if(!socialMediaPlatformImages.TryGetValue(platform, out var value))
        {
            return Result<SocialMediatPlatform>.Failure(new Error("Invalid-Social_Media_Platform", $"Platform {platform} is not valid, use one of: {string.Join(", ", Enum.GetValues<SocialMediaPlatforms>())}"));
        }
        return value;
    }
}