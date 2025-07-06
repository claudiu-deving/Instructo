namespace Api;

public class StartupHelpers
{
    public static string GetIronManId(string[] args)
    {
#if DEBUG
        return "d9d42c1e-024a-489f-61f7-08dd73a823b2";
#endif
        var s = Environment.GetEnvironmentVariable("IRONMAN_ID");
        if (s is not null) return s;
        if (args.Length > 0 && args[0] is { } id)
            s = id;
        else
            throw new ArgumentException("{IRONMAN_ID} is missing,cannot proceed");

        return s;
    }
}