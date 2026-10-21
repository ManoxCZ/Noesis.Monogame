using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Noesis.MonoGame.TestApp;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        foreach (var item in Directory.GetFiles(Environment.CurrentDirectory, "*.log"))
        {
            File.Delete(item);
        }

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFile("application.log", Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        using (var game = new GameWithNoesis(loggerFactory)
        {
            NoesisLicenceName = string.Empty,
            NoesisLicenceKey = string.Empty,
            NoesisThemeFilename = @"GUI/Resources.xaml",
            NoesisMainViewFilename = @"GUI/MainWindow.xaml",
        })
        {               
            game.Run();
        }
    }
}