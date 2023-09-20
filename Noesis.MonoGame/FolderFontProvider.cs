using System;
using System.IO;

namespace Noesis.MonoGame;

public class FolderFontProvider : FontProvider
{
    public override Stream OpenFont(Uri uri, string id)
    {
        foreach (var extension in new[] { ".ttf", ".otf", ".ttc" })
        {
            string fontPath = id + extension;

            if (File.Exists(fontPath))
            {
                return File.OpenRead(fontPath);
            }
        }

        throw new FileNotFoundException("Font file not found", id);
    }

    public override void ScanFolder(Uri uri)
    {        
        if (!Directory.Exists(uri.OriginalString))
        {
            return;
        }        

        string[] fontFilePaths = Directory.GetFiles(uri.OriginalString, "*.*", searchOption: SearchOption.AllDirectories);

        foreach (string fontPath in fontFilePaths)
        {
            string fontFilename = fontPath.Replace("\\", "/");

            if (fontPath.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                fontPath.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
                fontPath.EndsWith(".ttc", StringComparison.OrdinalIgnoreCase))
            {
                RegisterFont(uri, System.IO.Path.ChangeExtension(fontFilename, null));
            }
        }
    }
}