namespace Prefixr;

public static class Interpreter
{
    private static bool _hadError;
    
    private static void Main(string[] args)
    {
        var platform = Environment.OSVersion.Platform;
        
        switch (args.Length)
        {
            case > 1:
                Console.Error.WriteLine(platform is PlatformID.MacOSX or PlatformID.Unix
                    ? "usage: Prefixr [path]"
                    : "usage: Prefixr.exe [path]");
                
                Environment.Exit(64);
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                RunPrompt();
                break;
        }
    }
    
    private static void RunFile(string path)
    {
        var qualifiedPath = Path.Combine(Environment.CurrentDirectory, path);
        var bytes = File.ReadAllBytes(qualifiedPath);
        var program = System.Text.Encoding.UTF8.GetString(bytes).ToCharArray();

        Run(new string(program));

        if (_hadError) Environment.Exit(64);
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();

            if (line is null) break;
            Run(line);
        }
    }

    private static void Run(string program)
    {
        var scanner = new Scanner(program);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    internal static void Error(int line, string message, string location = "")
    {
        Console.Error.WriteLine(location == string.Empty
            ? $"at line {line}, error: {message}"
            : $"at line {line}, {location} error: {message}");
        
        _hadError = true;
    }
}