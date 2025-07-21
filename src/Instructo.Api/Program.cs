using Api.Builder;
using Api.Config;


(await AppBuilder.BuildApp(args).Compose()).Run();

public partial class Program
{
}