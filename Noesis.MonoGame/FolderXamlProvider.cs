using System;
using System.IO;

namespace Noesis.MonoGame;

public class FolderXamlProvider : XamlProvider
{
    public override Stream? LoadXaml(Uri uri)
    {
        if (uri.IsAbsoluteUri && File.OpenRead(uri.AbsolutePath) is Stream stream)
        {
            return stream;
        }

        if (File.OpenRead(uri.OriginalString) is Stream originalStream)
        {
            return originalStream;
        }

        throw new FileNotFoundException("File not found", uri.OriginalString);
    }
}