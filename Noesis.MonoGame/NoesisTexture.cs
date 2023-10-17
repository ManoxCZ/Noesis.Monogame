using System;

namespace Noesis.MonoGame;

public sealed class NoesisTexture : Texture
{
    public string Label { get; }
    public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; }


    public NoesisTexture(string label, Microsoft.Xna.Framework.Graphics.Texture2D texture)
    {
        Label = label;
        Texture = texture;
    }

    public override uint Width => (uint)Texture.Width;

    public override uint Height => (uint)Texture.Height;

    public override bool HasMipMaps => Texture.LevelCount > 1;

    public override bool IsInverted => false;

    public override bool HasAlpha => Texture.Format == Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color;


    public static Microsoft.Xna.Framework.Graphics.SurfaceFormat GetSurfaceFormat(Noesis.TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.RGBA8:
                return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color;

            case TextureFormat.R8:
                return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Alpha8;

            default:
                throw new NotSupportedException($"Format ${format} is not supported");
        }
    }

    public static int GetPixelSizeForSurfaceFormat(Microsoft.Xna.Framework.Graphics.SurfaceFormat format)
    {
        switch (format)
        {
            case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color:
                return 4;

            case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Alpha8:
                return 1;

            default:
                throw new NotSupportedException($"Format ${format} is not supported");
        }
    }
}
