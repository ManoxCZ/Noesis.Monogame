using System;
using System.Collections.Generic;
using System.IO;

namespace Noesis.MonoGame;

/// <summary>
/// Default texture loading provider for NoesisGUI.
/// Please note this is a very unefficient loader as simply added as a proof of work.
/// You might want to replace it with <see cref="ContentTextureProvider" /> as the much more efficient solution.
/// </summary>
public class FolderTextureProvider : TextureProvider, IDisposable
{
    private readonly Dictionary<string, WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Microsoft.Xna.Framework.Graphics.GraphicsDevice _graphicsDevice;

    public FolderTextureProvider(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public new void Dispose()
    {
        foreach (KeyValuePair<string, WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>> entry in _cache)
        {
            if (entry.Value.TryGetTarget(out Microsoft.Xna.Framework.Graphics.Texture2D? texture))
            {
                texture?.Dispose();
            }
        }

        _cache.Clear();

        base.Dispose();
    }

    public override TextureInfo GetTextureInfo(Uri uri)
    {
        Microsoft.Xna.Framework.Graphics.Texture2D texture = GetTexture(uri.OriginalString);

        return new TextureInfo(texture.Width, texture.Height);
    }

    public override Texture LoadTexture(Uri uri)
    {
        var texture2D = GetTexture(uri.OriginalString);

        return new NoesisTexture(uri.OriginalString, texture2D);
    }

    protected virtual Microsoft.Xna.Framework.Graphics.Texture2D LoadTextureFromStream(FileStream fileStream)
    {
        Microsoft.Xna.Framework.Graphics.Texture2D texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(_graphicsDevice, fileStream);

        if (texture.Format != Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color)
        {
            return texture;
        }

        // unfortunately, MonoGame loads textures as non-premultiplied alpha
        // need to premultiply alpha for correct rendering with NoesisGUI
        var buffer = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
        texture.GetData(buffer);

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
        }

        texture.SetData(buffer);

        return texture;
    }

    private Microsoft.Xna.Framework.Graphics.Texture2D GetTexture(string filename)
    {
        if (_cache.TryGetValue(filename, out var weakReference) &&
            weakReference.TryGetTarget(out var cachedTexture) &&
            !cachedTexture.IsDisposed)
        {
            return cachedTexture;
        }

        string fullPath = System.IO.Path.Combine(Environment.CurrentDirectory, filename);

        FileStream fileStream = File.OpenRead(fullPath);

        Microsoft.Xna.Framework.Graphics.Texture2D texture = LoadTextureFromStream(fileStream);

        _cache[filename] = new WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>(texture);

        return texture;
    }
}