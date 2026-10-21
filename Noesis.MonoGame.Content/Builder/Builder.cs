/// <summary>
/// Entry point for the Content Builder project, 
/// which when executed will build content according to the "Content Collection Strategy" defined in the Builder class.
/// </summary>
/// <remarks>
/// Make sure to validate the directory paths in the "ContentBuilderParams" for your specific project.
/// For more details regarding the Content Builder, see the MonoGame documentation: <tbc.>
/// </remarks>

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

var contentCollectionArgs = new ContentBuilderParams()
{
    Mode = ContentBuilderMode.Builder,
    WorkingDirectory = $"{AppContext.BaseDirectory}..\\..\\..\\", // path to where your content folder can be located
   SourceDirectory = "Assets", // Not actually needed as this is the default, but added for reference
    GraphicsProfile = GraphicsProfile.HiDef,
    LogLevel = LogLevel.Info,    
    Platform = TargetPlatform.Windows
};
var builder = new Builder();

if (args is not null && args.Length > 0)
{
    builder.Run(args);
}
else
{
    builder.Run(contentCollectionArgs);
}

Console.WriteLine("Ahoj");

return builder.FailedToBuild > 0 ? -1 : 0;

public class Builder : ContentBuilder
{
    public override IContentCollection GetContentCollection()
    {
        ContentCollection contentCollection = new();

        ImportShader(contentCollection, @"Noesis/Effects/Blur.fx", "HAS_COLOR;HAS_UV1;EFFECT_BLUR;PAINT_SOLID");
        ImportShader(contentCollection, @"Noesis/Effects/RGBA.fx", "EFFECT_RGBA");
        ImportShader(contentCollection, @"Noesis/Effects/Mask.fx", "EFFECT_MASK");
        ImportShader(contentCollection, @"Noesis/Effects/Clear.fx", "EFFECT_CLEAR");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Solid.fx", "HAS_COLOR;EFFECT_PATH;PAINT_SOLID");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Linear.fx", "HAS_UV0;EFFECT_PATH;PAINT_LINEAR");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Radial.fx", "HAS_UV0;EFFECT_PATH;PAINT_RADIAL");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Pattern.fx", "HAS_UV0;EFFECT_PATH;PAINT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Pattern_Clamp.fx", "HAS_UV0;HAS_RECT;EFFECT_PATH;PAINT_PATTERN;CLAMP_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Pattern_Repeat.fx", "HAS_UV0;HAS_RECT;HAS_TILE;EFFECT_PATH;PAINT_PATTERN;REPEAT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Pattern_MirrorU.fx", "HAS_UV0;HAS_RECT;HAS_TILE;EFFECT_PATH;PAINT_PATTERN;MIRRORU_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Pattern_MirrorV.fx", "HAS_UV0;HAS_RECT;HAS_TILE;EFFECT_PATH;PAINT_PATTERN;MIRRORV_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_Pattern_Mirror.fx", "HAS_UV0;HAS_RECT;HAS_TILE;EFFECT_PATH;PAINT_PATTERN;MIRROR_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Solid.fx", "HAS_COLOR;HAS_COVERAGE;EFFECT_PATH_AA;PAINT_SOLID");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Linear.fx", "HAS_UV0;HAS_COVERAGE;EFFECT_PATH_AA;PAINT_LINEAR");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Radial.fx", "HAS_UV0;HAS_COVERAGE;EFFECT_PATH_AA;PAINT_RADIAL");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Pattern.fx", "HAS_UV0;HAS_COVERAGE;EFFECT_PATH_AA;PAINT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Pattern_Clamp.fx", "HAS_UV0;HAS_COVERAGE;HAS_RECT;EFFECT_PATH_AA;PAINT_PATTERN;CLAMP_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Pattern_Repeat.fx", "HAS_UV0;HAS_COVERAGE;HAS_RECT;HAS_TILE;EFFECT_PATH_AA;PAINT_PATTERN;REPEAT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Pattern_MirrorU.fx", "HAS_UV0;HAS_COVERAGE;HAS_RECT;HAS_TILE;EFFECT_PATH_AA;PAINT_PATTERN;MIRRORU_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Pattern_MirrorV.fx", "HAS_UV0;HAS_COVERAGE;HAS_RECT;HAS_TILE;EFFECT_PATH_AA;PAINT_PATTERN;MIRRORV_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Path_AA_Pattern_Mirror.fx", "HAS_UV0;HAS_COVERAGE;HAS_RECT;HAS_TILE;EFFECT_PATH_AA;PAINT_PATTERN;MIRROR_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Solid.fx", "HAS_COLOR;HAS_UV1;SDF;EFFECT_SDF;PAINT_SOLID");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Linear.fx", "HAS_UV0;HAS_UV1;SDF;EFFECT_SDF;PAINT_LINEAR");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Radial.fx", "HAS_UV0;HAS_UV1;SDF;EFFECT_SDF;PAINT_RADIAL");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Pattern.fx", "HAS_UV0;HAS_UV1;SDF;EFFECT_SDF;PAINT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Pattern_Clamp.fx", "HAS_UV0;HAS_UV1;HAS_RECT;SDF;EFFECT_SDF;PAINT_PATTERN;CLAMP_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Pattern_Repeat.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF;PAINT_PATTERN;REPEAT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Pattern_MirrorU.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF;PAINT_PATTERN;MIRRORU_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Pattern_MirrorV.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF;PAINT_PATTERN;MIRRORV_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_Pattern_Mirror.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF;PAINT_PATTERN;MIRROR_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Solid.fx", "HAS_COLOR;HAS_UV1;SDF;EFFECT_SDF_LCD;PAINT_SOLID");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Linear.fx", "HAS_UV0;HAS_UV1;SDF;EFFECT_SDF_LCD;PAINT_LINEAR");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Radial.fx", "HAS_UV0;HAS_UV1;SDF;EFFECT_SDF_LCD;PAINT_RADIAL");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Pattern.fx", "HAS_UV0;HAS_UV1;SDF;EFFECT_SDF_LCD;PAINT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Pattern_Clamp.fx", "HAS_UV0;HAS_UV1;HAS_RECT;SDF;EFFECT_SDF_LCD;PAINT_PATTERN;CLAMP_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Pattern_Repeat.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF_LCD;PAINT_PATTERN;REPEAT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Pattern_MirrorU.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF_LCD;PAINT_PATTERN;MIRRORU_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Pattern_MirrorV.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF_LCD;PAINT_PATTERN;MIRRORV_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/SDF_LCD_Pattern_Mirror.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF;EFFECT_SDF_LCD;PAINT_PATTERN;MIRROR_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Solid.fx", "HAS_COLOR;HAS_UV1;EFFECT_OPACITY;PAINT_SOLID");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Linear.fx", "HAS_UV0;HAS_UV1;EFFECT_OPACITY;PAINT_LINEAR");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Radial.fx", "HAS_UV0;HAS_UV1;EFFECT_OPACITY;PAINT_RADIAL");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Pattern.fx", "HAS_UV0;HAS_UV1;EFFECT_OPACITY;PAINT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Pattern_Clamp.fx", "HAS_UV0;HAS_UV1;HAS_RECT;EFFECT_OPACITY;PAINT_PATTERN;CLAMP_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Pattern_Repeat.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;EFFECT_OPACITY;PAINT_PATTERN;REPEAT_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Pattern_MirrorU.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;EFFECT_OPACITY;PAINT_PATTERN;MIRRORU_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Pattern_MirrorV.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;EFFECT_OPACITY;PAINT_PATTERN;MIRRORV_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Opacity_Pattern_Mirror.fx", "HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;EFFECT_OPACITY;PAINT_PATTERN;MIRROR_PATTERN");
        ImportShader(contentCollection, @"Noesis/Effects/Upsample.fx", "HAS_COLOR;HAS_UV0;HAS_UV1;EFFECT_UPSAMPLE");
        ImportShader(contentCollection, @"Noesis/Effects/Downsample.fx", "HAS_UV0;HAS_UV1;DOWNSAMPLE;EFFECT_DOWNSAMPLE");
        ImportShader(contentCollection, @"Noesis/Effects/Shadow.fx", "HAS_COLOR;HAS_UV1;HAS_RECT;EFFECT_SHADOW;PAINT_SOLID");
        ImportShader(contentCollection, @"Noesis/Effects/Blur.fx", "HAS_COLOR;HAS_UV1;EFFECT_BLUR;PAINT_SOLID");

        return contentCollection;
    }

    private static void ImportShader(ContentCollection contentCollection, string effectPath, string defines)
    {
        EffectImporter effectImporter = new();
        EffectProcessor effectProcessor = new()
        {
            DebugMode = EffectProcessorDebugMode.Auto,
            Defines = defines
        };

        contentCollection.Include(effectPath, effectImporter, effectProcessor);
    }
}