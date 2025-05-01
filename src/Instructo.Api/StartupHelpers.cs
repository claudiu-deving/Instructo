namespace Api;

public class StartupHelpers
{
    public static string GetIronManId(string[] args)
    {
        var s = Environment.GetEnvironmentVariable("IRONMAN_ID");
        if (s is not null) return s;
        if(args.Length >0 && args[0] is { } id)
        {
            s = id; 
        }
        else
        {
            throw new ArgumentException("{IRONMAN_ID} is missing,cannot proceed");
        }

        return s;
    }
}