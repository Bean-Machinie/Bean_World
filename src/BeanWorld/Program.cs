using BeanWorld;

// Write any unhandled exception to crash.log before the process dies
AppDomain.CurrentDomain.UnhandledException += (_, args) =>
{
    var text = args.ExceptionObject?.ToString() ?? "Unknown error";
    File.WriteAllText("crash.log", text);
    Console.Error.WriteLine(text);
};

try
{
    using var game = new BeanWorldGame();
    game.Run();
}
catch (Exception ex)
{
    File.WriteAllText("crash.log", ex.ToString());
    Console.Error.WriteLine(ex);
}
