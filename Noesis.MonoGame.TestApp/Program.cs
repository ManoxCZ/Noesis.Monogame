using Microsoft.Extensions.Logging;
using System;

namespace Noesis.MonoGame.TestApp;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFile("application.log", Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        using (var game = new GameWithNoesis(loggerFactory)
        {
            NoesisLicenceName = "your_licence_name",
            NoesisLicenceKey = "your_licence_key",
            NoesisThemeFilename = @"GUI/Resources.xaml",
            NoesisMainViewFilename = @"GUI/MainWindow.xaml",
        })
        {               
            game.Run();
        }
    }
}