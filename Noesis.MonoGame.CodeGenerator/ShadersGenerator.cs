using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Noesis.MonoGame.CodeGenerator;

internal class ShadersGenerator
{
    private readonly List<(string, string, string)> _vertexShaders = new(new (string, string, string)[]
    {
        ("Pos","ShaderVS.hlsl",""),
        ("PosColor","ShaderVS.hlsl","HAS_COLOR"),
        ("PosTex0","ShaderVS.hlsl","HAS_UV0"),
        ("PosTex0Rect","ShaderVS.hlsl","HAS_UV0;HAS_RECT"),
        ("PosTex0RectTile","ShaderVS.hlsl","HAS_UV0;HAS_RECT;HAS_TILE"),
        ("PosColorCoverage","ShaderVS.hlsl","HAS_COLOR;HAS_COVERAGE"),
        ("PosTex0Coverage","ShaderVS.hlsl","HAS_UV0;HAS_COVERAGE"),
        ("PosTex0CoverageRect","ShaderVS.hlsl","HAS_UV0;HAS_COVERAGE;HAS_RECT"),
        ("PosTex0CoverageRectTile","ShaderVS.hlsl","HAS_UV0;HAS_COVERAGE;HAS_RECT;HAS_TILE"),
        ("PosColorTex1_SDF","ShaderVS.hlsl","HAS_COLOR;HAS_UV1;SDF"),
        ("PosTex0Tex1_SDF","ShaderVS.hlsl","HAS_UV0;HAS_UV1;SDF"),
        ("PosTex0Tex1Rect_SDF","ShaderVS.hlsl","HAS_UV0;HAS_UV1;HAS_RECT;SDF"),
        ("PosTex0Tex1RectTile_SDF","ShaderVS.hlsl","HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE;SDF"),
        ("PosColorTex1","ShaderVS.hlsl","HAS_COLOR;HAS_UV1"),
        ("PosTex0Tex1","ShaderVS.hlsl","HAS_UV0;HAS_UV1"),
        ("PosTex0Tex1Rect","ShaderVS.hlsl","HAS_UV0;HAS_UV1;HAS_RECT"),
        ("PosTex0Tex1RectTile","ShaderVS.hlsl","HAS_UV0;HAS_UV1;HAS_RECT;HAS_TILE"),
        ("PosColorTex0Tex1","ShaderVS.hlsl","HAS_COLOR;HAS_UV0;HAS_UV1"),
        ("PosTex0Tex1_Downsample","ShaderVS.hlsl","HAS_UV0;HAS_UV1;DOWNSAMPLE"),
        ("PosColorTex1Rect","ShaderVS.hlsl", "HAS_COLOR;HAS_UV1;HAS_RECT"),
        ("PosColorTex0RectImagePos","ShaderVS.hlsl","HAS_COLOR;HAS_UV0;HAS_RECT;HAS_IMAGE_POSITION"),
        ("PosColor_SRGB","ShaderVS.hlsl","HAS_COLOR;LINEAR_COLOR_SPACE"),
        ("PosColorCoverage_SRGB","ShaderVS.hlsl","HAS_COLOR;HAS_COVERAGE;LINEAR_COLOR_SPACE"),
        ("PosColorTex1_SDF_SRGB","ShaderVS.hlsl","HAS_COLOR;HAS_UV1;SDF;LINEAR_COLOR_SPACE"),
        ("PosColorTex1_SRGB","ShaderVS.hlsl","HAS_COLOR;HAS_UV1;LINEAR_COLOR_SPACE"),
        ("PosColorTex0Tex1_SRGB","ShaderVS.hlsl","HAS_COLOR;HAS_UV0;HAS_UV1;LINEAR_COLOR_SPACE"),
        ("PosColorTex1Rect_SRGB","ShaderVS.hlsl","HAS_COLOR;HAS_UV1;HAS_RECT;LINEAR_COLOR_SPACE"),
        ("PosColorTex0RectImagePos_SRGB","ShaderVS.hlsl","HAS_COLOR;HAS_UV0;HAS_RECT;HAS_IMAGE_POSITION;LINEAR_COLOR_SPACE")
    });

    private readonly List<(string, string, string)> _pixelShaders = new(new (string, string, string)[]
    {
        ("RGBA","ShaderPS.hlsl","EFFECT_RGBA"),
        ("Mask","ShaderPS.hlsl","EFFECT_MASK"),
        ("Clear","ShaderPS.hlsl","EFFECT_CLEAR"),
        ("Path_Solid","ShaderPS.hlsl","EFFECT_PATH;PAINT_SOLID"),
        ("Path_Linear","ShaderPS.hlsl","EFFECT_PATH;PAINT_LINEAR"),
        ("Path_Radial","ShaderPS.hlsl","EFFECT_PATH;PAINT_RADIAL"),
        ("Path_Pattern","ShaderPS.hlsl","EFFECT_PATH;PAINT_PATTERN"),
        ("Path_Pattern_Clamp","ShaderPS.hlsl","EFFECT_PATH;PAINT_PATTERN;CLAMP_PATTERN"),
        ("Path_Pattern_Repeat","ShaderPS.hlsl","EFFECT_PATH;PAINT_PATTERN;REPEAT_PATTERN"),
        ("Path_Pattern_MirrorU","ShaderPS.hlsl","EFFECT_PATH;PAINT_PATTERN;MIRRORU_PATTERN"),
        ("Path_Pattern_MirrorV","ShaderPS.hlsl","EFFECT_PATH;PAINT_PATTERN;MIRRORV_PATTERN"),
        ("Path_Pattern_Mirror","ShaderPS.hlsl","EFFECT_PATH;PAINT_PATTERN;MIRROR_PATTERN"),
        ("Path_AA_Solid","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_SOLID"),
        ("Path_AA_Linear","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_LINEAR"),
        ("Path_AA_Radial","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_RADIAL"),
        ("Path_AA_Pattern","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_PATTERN"),
        ("Path_AA_Pattern_Clamp","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_PATTERN;CLAMP_PATTERN"),
        ("Path_AA_Pattern_Repeat","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_PATTERN;REPEAT_PATTERN"),
        ("Path_AA_Pattern_MirrorU","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_PATTERN;MIRRORU_PATTERN"),
        ("Path_AA_Pattern_MirrorV","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_PATTERN;MIRRORV_PATTERN"),
        ("Path_AA_Pattern_Mirror","ShaderPS.hlsl","EFFECT_PATH_AA;PAINT_PATTERN;MIRROR_PATTERN"),
        ("SDF_Solid","ShaderPS.hlsl","EFFECT_SDF;PAINT_SOLID"),
        ("SDF_Linear","ShaderPS.hlsl","EFFECT_SDF;PAINT_LINEAR"),
        ("SDF_Radial","ShaderPS.hlsl","EFFECT_SDF;PAINT_RADIAL"),
        ("SDF_Pattern","ShaderPS.hlsl","EFFECT_SDF;PAINT_PATTERN"),
        ("SDF_Pattern_Clamp","ShaderPS.hlsl","EFFECT_SDF;PAINT_PATTERN;CLAMP_PATTERN"),
        ("SDF_Pattern_Repeat","ShaderPS.hlsl","EFFECT_SDF;PAINT_PATTERN;REPEAT_PATTERN"),
        ("SDF_Pattern_MirrorU","ShaderPS.hlsl","EFFECT_SDF;PAINT_PATTERN;MIRRORU_PATTERN"),
        ("SDF_Pattern_MirrorV","ShaderPS.hlsl","EFFECT_SDF;PAINT_PATTERN;MIRRORV_PATTERN"),
        ("SDF_Pattern_Mirror","ShaderPS.hlsl","EFFECT_SDF;PAINT_PATTERN;MIRROR_PATTERN"),
        ("SDF_LCD_Solid","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_SOLID"),
        ("SDF_LCD_Linear","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_LINEAR"),
        ("SDF_LCD_Radial","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_RADIAL"),
        ("SDF_LCD_Pattern","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_PATTERN"),
        ("SDF_LCD_Pattern_Clamp","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_PATTERN;CLAMP_PATTERN"),
        ("SDF_LCD_Pattern_Repeat","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_PATTERN;REPEAT_PATTERN"),
        ("SDF_LCD_Pattern_MirrorU","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_PATTERN;MIRRORU_PATTERN"),
        ("SDF_LCD_Pattern_MirrorV","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_PATTERN;MIRRORV_PATTERN"),
        ("SDF_LCD_Pattern_Mirror","ShaderPS.hlsl","EFFECT_SDF_LCD;PAINT_PATTERN;MIRROR_PATTERN"),
        ("Opacity_Solid","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_SOLID"),
        ("Opacity_Linear","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_LINEAR"),
        ("Opacity_Radial","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_RADIAL"),
        ("Opacity_Pattern","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_PATTERN"),
        ("Opacity_Pattern_Clamp","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_PATTERN;CLAMP_PATTERN"),
        ("Opacity_Pattern_Repeat","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_PATTERN;REPEAT_PATTERN"),
        ("Opacity_Pattern_MirrorU","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_PATTERN;MIRRORU_PATTERN"),
        ("Opacity_Pattern_MirrorV","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_PATTERN;MIRRORV_PATTERN"),
        ("Opacity_Pattern_Mirror","ShaderPS.hlsl","EFFECT_OPACITY;PAINT_PATTERN;MIRROR_PATTERN"),
        ("Upsample","ShaderPS.hlsl","EFFECT_UPSAMPLE"),
        ("Downsample","ShaderPS.hlsl","EFFECT_DOWNSAMPLE"),
        ("Shadow","ShaderPS.hlsl","EFFECT_SHADOW;PAINT_SOLID"),
        ("Blur","ShaderPS.hlsl","EFFECT_BLUR;PAINT_SOLID"),
        ("Resolve2","ResolvePS.hlsl","NUM_SAMPLES=2"),
        ("Resolve4","ResolvePS.hlsl","NUM_SAMPLES=4"),
        ("Resolve8","ResolvePS.hlsl","NUM_SAMPLES=8"),
        ("Resolve16","ResolvePS.hlsl","NUM_SAMPLES=16"),
    });


    public void Execute(string outputPath)
    {
        foreach (string path in Directory.EnumerateFiles("Shaders", "*.hlsl"))
        {
            File.Copy(path, System.IO.Path.Combine(outputPath, "Noesis", "Effects", System.IO.Path.GetFileName(path)), true);
        }

        using StreamWriter contentWriter = new(System.IO.Path.Combine(outputPath, "Content.mgcb"));

        contentWriter.WriteLine("#----------------------------- Global Properties ----------------------------#");
        contentWriter.WriteLine();
        contentWriter.WriteLine("/outputDir:bin /$(Platform)");
        contentWriter.WriteLine("/intermediateDir:obj /$(Platform)");
        contentWriter.WriteLine("/platform:Windows");
        contentWriter.WriteLine("/config:");
        contentWriter.WriteLine("/profile:HiDef");
        contentWriter.WriteLine("/compress:False");
        contentWriter.WriteLine();
        contentWriter.WriteLine("#-------------------------------- References --------------------------------#");
        contentWriter.WriteLine();
        contentWriter.WriteLine();
        contentWriter.WriteLine("#---------------------------------- Content ---------------------------------#");
        contentWriter.WriteLine();

        for (int i = 0; i < (int)Shader.Enum.Count - 1; i++)
        {
            string shaderName = ((Shader.Enum)i).ToString();

            Shader.Vertex.Enum vertexForShader = Shader.VertexForShader((Shader.Enum)i);

            (string, string, string) vertexShader = _vertexShaders.First(item => item.Item1 == vertexForShader.ToString());

            string vertexForShaderDefines = vertexShader.Item3;

            if (vertexForShaderDefines.Length > 0)
            {
                vertexForShaderDefines += ";";
            }

            contentWriter.WriteLine($"# begin Noesis/Effects/{shaderName}.fx");
            contentWriter.WriteLine("/importer:EffectImporter");
            contentWriter.WriteLine("/processor:EffectProcessor");
            contentWriter.WriteLine("/processorParam:DebugMode = Auto");
            contentWriter.WriteLine($"/processorParam:Defines={vertexForShaderDefines}{_pixelShaders[i].Item3}");
            contentWriter.WriteLine($"/build:Noesis/Effects/{shaderName}.fx");
            contentWriter.WriteLine();

            using StreamWriter effectWriter = new(System.IO.Path.Combine(outputPath, "Noesis", "Effects", $"{shaderName}.fx"));

            effectWriter.WriteLine("#if OPENGL");
            effectWriter.WriteLine("#define SV_POSITION POSITION");
            effectWriter.WriteLine("#define VS_SHADERMODEL vs_3_0");
            effectWriter.WriteLine("#define PS_SHADERMODEL ps_3_0");
            effectWriter.WriteLine();
            effectWriter.WriteLine($"#include \"{vertexShader.Item2.Replace(".hlsl", "-DesktopGL.hlsl")}\"");
            effectWriter.WriteLine($"#include \"{_pixelShaders[i].Item2.Replace(".hlsl", "-DesktopGL.hlsl")}\"");
            effectWriter.WriteLine("#else");
            effectWriter.WriteLine("#define VS_SHADERMODEL vs_5_0");
            effectWriter.WriteLine("#define PS_SHADERMODEL ps_5_0");
            effectWriter.WriteLine();
            effectWriter.WriteLine($"#include \"{vertexShader.Item2.Replace(".hlsl", "-WindowsDX.hlsl")}\"");
            effectWriter.WriteLine($"#include \"{_pixelShaders[i].Item2.Replace(".hlsl", "-WindowsDX.hlsl")}\"");
            effectWriter.WriteLine("#endif");            
            effectWriter.WriteLine();
            effectWriter.WriteLine("technique");
            effectWriter.WriteLine("{");
            effectWriter.WriteLine("   pass");
            effectWriter.WriteLine("   {");
            effectWriter.WriteLine("       VertexShader = compile VS_SHADERMODEL MainVS();");
            effectWriter.WriteLine("       PixelShader = compile PS_SHADERMODEL MainPS();");
            effectWriter.WriteLine("   }");
            effectWriter.WriteLine("};");
        }
    }
}
