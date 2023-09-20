//#define EXPORT_TEXTURES
//#define DEBUG_LOG

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Noesis.MonoGame.Generated;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Noesis.MonoGame;

public sealed class NoesisRenderDevice : RenderDevice
{
#if WINDOWSDX
    private const string PATTERN_TEXTURE_SLOT = "pattern";
    private const string RAMPS_TEXTURE_SLOT = "ramps";
    private const string IMAGE_TEXTURE_SLOT = "image";
    private const string GLYPHS_TEXTURE_SLOT = "glyphs";
    private const string SHADOW_TEXTURE_SLOT = "shadow";
#else
    private const string PATTERN_TEXTURE_SLOT = "patternSampler+pattern";
    private const string RAMPS_TEXTURE_SLOT = "ramps";
    private const string IMAGE_TEXTURE_SLOT = "imageSampler+image";
    private const string GLYPHS_TEXTURE_SLOT = "glyphsSampler+glyphs";
    private const string SHADOW_TEXTURE_SLOT = "shadowSampler+shadow";
#endif
    private const string PROJECTION_MATRIX_PARAMETER = "projectionMtx";
    private const string TEXTURE_SIZE_PARAMETER = "textureSize";
    private const string OPACITY_PARAMETER = "opacity";
    private const string RGBA_PARAMETER = "rgba";

    private readonly ILogger _logger;
    private readonly Microsoft.Xna.Framework.Graphics.GraphicsDevice _device;
    private readonly Microsoft.Xna.Framework.Graphics.Effect[] _effects;
    private readonly Microsoft.Xna.Framework.Graphics.BlendState[] _blendStates;
    private readonly Microsoft.Xna.Framework.Graphics.DepthStencilState[] _depthStencilStates;
    private readonly Microsoft.Xna.Framework.Graphics.RasterizerState _rasterizerState;
    private readonly Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;

    private byte[] _vertices = Array.Empty<byte>();
    private Memory<byte> _verticesMemory = new();
    private MemoryHandle? _verticesMemoryHandle;

    private short[] _indices = Array.Empty<short>();
    private Memory<short> _indicesMemory = new();
    private MemoryHandle? _indicesMemoryHandle;


    public override DeviceCaps Caps => new()
    {
        CenterPixelOffset = 0,
        LinearRendering = false,
        SubpixelRendering = true,
    };

    public NoesisRenderDevice(
        Microsoft.Xna.Framework.Graphics.GraphicsDevice device,
        Microsoft.Xna.Framework.Content.ContentManager contentManager, 
        ILoggerFactory? loggerFactory = null)
    {
        _logger = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger<NoesisRenderDevice>();

        _device = device;

        _effects = LoadShaders(contentManager);
        _blendStates = CreateBlendStates();
        _depthStencilStates = CreateDepthStencilStates();
        _rasterizerState = new Microsoft.Xna.Framework.Graphics.RasterizerState()
        {
            CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None,
            DepthBias = 0,
            FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid,
            SlopeScaleDepthBias = 0,
            DepthClipEnable = true,
            ScissorTestEnable = true
        };
        _spriteBatch = new (_device);

        _logger.LogDebug("All resources loaded");

        GlyphCacheHeight = 2048;
    }

    private static Microsoft.Xna.Framework.Graphics.Effect[] LoadShaders(Microsoft.Xna.Framework.Content.ContentManager contentManager)
    {
        Microsoft.Xna.Framework.Graphics.Effect[] effects = new Microsoft.Xna.Framework.Graphics.Effect[(int)Shader.Enum.Count - 1];

        for (int i = 0; i < effects.Length; i++)
        {
            string shaderName = ((Shader.Enum)i).ToString();

            effects[i] = contentManager.Load<Microsoft.Xna.Framework.Graphics.Effect>($"Content\\Noesis\\Effects\\{shaderName}");
        }

        return effects;
    }

    private static Microsoft.Xna.Framework.Graphics.BlendState[] CreateBlendStates()
    {
        return new Microsoft.Xna.Framework.Graphics.BlendState[]
        {
            //Src,
            Microsoft.Xna.Framework.Graphics.BlendState.Opaque,
            //SrcOver,            
            new()
            {
                Name ="SrcOver",
                IndependentBlendEnable = false,
                AlphaBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
                AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                ColorBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
            },
            //SrcOver_Multiply,
            new()
            {
                Name = "SrcOver_Multiply",
                AlphaBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
                AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.DestinationColor,
                ColorBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
            },
            //SrcOver_Screen,
            new()
            {
                Name = "SrcOver_Screen",
                AlphaBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
                AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
            },
            //SrcOver_Additive,
            new()
            {
                Name = "SrcOver_Additive",
                AlphaBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
                AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
            },
            //SrcOver_Dual            
            new()
            {
                Name = "SrcOver_Dual",
                IndependentBlendEnable = false,
                AlphaBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
                AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
                ColorBlendFunction = Microsoft.Xna.Framework.Graphics.BlendFunction.Add,
                ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceColor,
                ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
            }
        };
    }


    private static Microsoft.Xna.Framework.Graphics.DepthStencilState[] CreateDepthStencilStates()
    {
        return new Microsoft.Xna.Framework.Graphics.DepthStencilState[]
        {
            //Disabled,
            new Microsoft.Xna.Framework.Graphics.DepthStencilState()
            {
                Name = "Disabled",
                StencilMask = 0xff,
                StencilWriteMask = 0xff,
                TwoSidedStencilMode = true,
                StencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                StencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                DepthBufferEnable = false,
                DepthBufferFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Never,
                StencilEnable = false,
                StencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                StencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                CounterClockwiseStencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
            },
            //Equal_Keep,
            new Microsoft.Xna.Framework.Graphics.DepthStencilState()
            {
                Name = "Equal_Keep",
                StencilMask = 0xff,
                StencilWriteMask = 0xff,
                TwoSidedStencilMode = true,
                StencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                StencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                DepthBufferEnable = false,
                DepthBufferFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Never,
                StencilEnable = true,
                StencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                StencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                CounterClockwiseStencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
            },
            //Equal_Incr,
            new Microsoft.Xna.Framework.Graphics.DepthStencilState()
            {
                Name = "Equal_Incr",
                StencilMask = 0xff,
                StencilWriteMask = 0xff,
                TwoSidedStencilMode = true,
                StencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                StencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                DepthBufferEnable = false,
                DepthBufferFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Never,
                StencilEnable = true,
                StencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                StencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Increment,
                CounterClockwiseStencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                CounterClockwiseStencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Increment,
            },
            //Equal_Decr,
            new Microsoft.Xna.Framework.Graphics.DepthStencilState()
            {
                Name = "Equal_Decr",
                StencilMask = 0xff,
                StencilWriteMask = 0xff,
                TwoSidedStencilMode = true,
                StencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                StencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                DepthBufferEnable = false,
                DepthBufferFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Never,
                StencilEnable = true,
                StencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                StencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Decrement,
                CounterClockwiseStencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Equal,
                CounterClockwiseStencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Decrement,
            },
            //Clear
            new Microsoft.Xna.Framework.Graphics.DepthStencilState()
            {
                Name = "Clear",
                StencilMask = 0xff,
                StencilWriteMask = 0xff,
                TwoSidedStencilMode = true,
                StencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                StencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilDepthBufferFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                CounterClockwiseStencilFail = Microsoft.Xna.Framework.Graphics.StencilOperation.Keep,
                DepthBufferEnable = false,
                DepthBufferFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Never,
                StencilEnable = true,
                StencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Always,
                StencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Zero,
                CounterClockwiseStencilFunction = Microsoft.Xna.Framework.Graphics.CompareFunction.Always,
                CounterClockwiseStencilPass = Microsoft.Xna.Framework.Graphics.StencilOperation.Zero,
            },
        };
    }

    public override void BeginOffscreenRender()
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(BeginOffscreenRender));        
#endif
    }

    public override void BeginOnscreenRender()
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(BeginOnscreenRender));

        
#endif
    }

    public override void BeginTile(RenderTarget surface, Tile tile)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(BeginTile));
#endif

        //_device.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int)tile.X, (int)tile.Y, (int)tile.Width, (int)tile.Height);
    }

    public override RenderTarget CloneRenderTarget(string label, RenderTarget surface)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(CloneRenderTarget));
#endif

        if (surface is NoesisRenderTarget renderTarget)
        {
            return new NoesisRenderTarget(
                label,
                new Microsoft.Xna.Framework.Graphics.RenderTarget2D(_device,
                    renderTarget.RenderTarget2D.Width,
                    renderTarget.RenderTarget2D.Height,
                    renderTarget.RenderTarget2D.LevelCount != 1,
                    renderTarget.RenderTarget2D.Format,
                    renderTarget.RenderTarget2D.DepthStencilFormat,
                    renderTarget.RenderTarget2D.MultiSampleCount,
                    renderTarget.RenderTarget2D.RenderTargetUsage));
        }

        Exception exception = new ArgumentException($"Surface is not a correct type, should be {nameof(NoesisRenderTarget)} but it is {surface.GetType().FullName}");

        _logger.LogError(exception, nameof(CloneRenderTarget));

        throw exception;
    }

    public override RenderTarget CreateRenderTarget(string label, uint width, uint height, uint sampleCount, bool needsStencil)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(CreateRenderTarget));
#endif

        return new NoesisRenderTarget(label,
            new Microsoft.Xna.Framework.Graphics.RenderTarget2D(
                _device,
                (int)width,
                (int)height,
                false,
                Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color,
                needsStencil ? Microsoft.Xna.Framework.Graphics.DepthFormat.Depth24Stencil8 : Microsoft.Xna.Framework.Graphics.DepthFormat.Depth24,
                0, 
                Microsoft.Xna.Framework.Graphics.RenderTargetUsage.PreserveContents,
                false));
    }

    public override Texture CreateTexture(string label, uint width, uint height, uint numLevels, TextureFormat format, nint data)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(CreateTexture));
#endif

        NoesisTexture noesisTexture = new(
            label,
            new Microsoft.Xna.Framework.Graphics.Texture2D(_device, 
                (int)width, (int)height, numLevels > 1, NoesisTexture.GetSurfaceFormat(format), (int)numLevels));

        if (data != 0)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(CreateTexture) + " - setting data");
#endif

            nint[] dataArray = new nint[numLevels];

            for (int i = 0; i < numLevels; i++)
            {
                dataArray[i] = Marshal.ReadIntPtr(data, i * Marshal.SizeOf<nint>());
            }                       

            for (int i = 0; i < numLevels; i++)
            {
                if (dataArray[i] != 0)
                {
                    UpdateTexture(noesisTexture, (uint)i, 0, 0, width, height, dataArray[i]);
                }
                width >>= 1;
                height >>= 1;

                width = Math.Max(width, 1);
                height = Math.Max(height, 1);
            }

#if DEBUG_LOG
            _logger.LogDebug(nameof(CreateTexture) + " - data set");
#endif
        }

        return noesisTexture;
    }

    public override void DrawBatch(ref Batch batch)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(DrawBatch));
#endif

        Microsoft.Xna.Framework.Graphics.Effect effect = _effects[batch.Shader.Index];

        SetTextures(batch, effect);

        SetEffectParameters(batch, effect);

        effect.Techniques[0].Passes[0].Apply();
        
        SetRenderDeviceState(batch);

        NoesisVertexUtils.DrawUserIndexedPrimitives(_device, _vertices, _indices, batch);
    }

    private void SetTextures(in Batch batch, Microsoft.Xna.Framework.Graphics.Effect effect)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(SetTextures));
#endif        
        if (batch.Pattern is NoesisTexture patternTexture)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetTextures) + " - Pattern: " + patternTexture.Label);
#endif
            if (effect.Parameters[PATTERN_TEXTURE_SLOT] is Microsoft.Xna.Framework.Graphics.EffectParameter parameter)
            {
                if (parameter.GetValueTexture2D() != patternTexture.Texture)
                {
                    parameter.SetValue(patternTexture.Texture);
                }
            }
        }

        if (batch.Ramps is NoesisTexture rampsTexture)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetTextures) + " - Ramps: " + rampsTexture.Label);
#endif
            if (effect.Parameters[RAMPS_TEXTURE_SLOT] is Microsoft.Xna.Framework.Graphics.EffectParameter parameter)
            {
                if (parameter.GetValueTexture2D() != rampsTexture.Texture)
                {
                    parameter.SetValue(rampsTexture.Texture);
                }
            }
        }

        if (batch.Image is NoesisTexture imageTexture)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetTextures) + " - Image: " + imageTexture.Label);
#endif
            if (effect.Parameters[IMAGE_TEXTURE_SLOT] is Microsoft.Xna.Framework.Graphics.EffectParameter parameter)
            {
                if (parameter.GetValueTexture2D() != imageTexture.Texture)
                {
                    parameter.SetValue(imageTexture.Texture);
                }
            }
        }

        if (batch.Glyphs is NoesisTexture glyphsTexture)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetTextures) + " - Glyphs: " + glyphsTexture.Label);
#endif
            if (effect.Parameters[GLYPHS_TEXTURE_SLOT] is Microsoft.Xna.Framework.Graphics.EffectParameter parameter)
            {
                if (parameter.GetValueTexture2D() != glyphsTexture.Texture)
                {
                    parameter.SetValue(glyphsTexture.Texture);
                }
            }
        }

        if (batch.Shadow is NoesisTexture shadowTexture)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetTextures) + " - Shadow: " + shadowTexture.Label);
#endif
            if (effect.Parameters[SHADOW_TEXTURE_SLOT] is Microsoft.Xna.Framework.Graphics.EffectParameter parameter)
            {
                if (parameter.GetValueTexture2D() != shadowTexture.Texture)
                {
                    parameter.SetValue(shadowTexture.Texture);
                }
            }
        }
    }

    private void SetEffectParameters(in Batch batch, Microsoft.Xna.Framework.Graphics.Effect effect)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(SetEffectParameters));
#endif

        if (batch.VertexUniform0.NumWords == 16 &&
            Marshal.PtrToStructure<Matrix4>(batch.VertexUniform0.Values) is Matrix4 matrix)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetEffectParameters) + " - " + PROJECTION_MATRIX_PARAMETER);
#endif

            effect.Parameters[PROJECTION_MATRIX_PARAMETER].SetValue(FromNoesisToMonogame(matrix));
        }
        else if (batch.VertexUniform0.NumWords != 0)
        {
            throw new NotImplementedException();
        }

        if (batch.VertexUniform1.NumWords == 2 &&
            Marshal.PtrToStructure<Microsoft.Xna.Framework.Vector2>(batch.VertexUniform1.Values) is Microsoft.Xna.Framework.Vector2 texSize)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetEffectParameters) + " - " + TEXTURE_SIZE_PARAMETER + ": " + texSize.ToString());
#endif

            effect.Parameters[TEXTURE_SIZE_PARAMETER].SetValue(texSize);
        }
        else if (batch.VertexUniform1.NumWords != 0)
        {
            throw new NotImplementedException();
        }

        if (batch.PixelUniform0.NumWords == 1 &&
            Marshal.PtrToStructure<float>(batch.PixelUniform0.Values) is float opacity)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetEffectParameters) + " - " + OPACITY_PARAMETER + ": " + opacity.ToString());
#endif

            effect.Parameters[OPACITY_PARAMETER].SetValue(opacity);
        }
        else if (batch.PixelUniform0.NumWords == 4 &&
            Marshal.PtrToStructure< Microsoft.Xna.Framework.Vector4> (batch.PixelUniform0.Values) is Microsoft.Xna.Framework.Vector4 rgba)
        {
#if DEBUG_LOG
            _logger.LogDebug(nameof(SetEffectParameters) + " - " + RGBA_PARAMETER + ": " + rgba.ToString());
#endif

            effect.Parameters[RGBA_PARAMETER].SetValue(rgba);
        }
        else if (batch.PixelUniform0.NumWords != 0)
        {
            throw new NotImplementedException();
        }

        if (batch.PixelUniform1.NumWords != 0)
        {
            throw new NotImplementedException();
        }
    }

    private void SetRenderDeviceState(in Batch batch)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(SetRenderDeviceState));
#endif

        _device.BlendState = _blendStates[(int)batch.RenderState.BlendMode];
        _device.DepthStencilState = _depthStencilStates[(int)batch.RenderState.StencilMode];
        _device.RasterizerState = _rasterizerState;
    }

    public override void EndOffscreenRender()
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(EndOffscreenRender));
#endif
        _device.SetRenderTarget(null);
    }

    public override void EndOnscreenRender()
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(EndOnscreenRender));
#endif      
    }

    public override void EndTile(RenderTarget surface)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(EndTile));
#endif

        //_device.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, _device.Viewport.Width, _device.Viewport.Height);
        _device.RasterizerState = Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone;
    }

    public unsafe override nint MapIndices(uint bytes)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(MapIndices));
#endif

        int indicesCount = (int)(bytes / 2);

        if (_indices.Length < indicesCount)
        {
            _indices = new short[indicesCount];

            _indicesMemory = new Memory<short>(_indices);
        }

        _indicesMemoryHandle = _indicesMemory.Pin();

        return new nint(_indicesMemoryHandle.Value.Pointer);
    }

    public unsafe override nint MapVertices(uint bytes)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(MapVertices));
#endif

        if (_vertices.Length < bytes)
        {
            _vertices = new byte[bytes];

            _verticesMemory = new Memory<byte>(_vertices);
        }

        _verticesMemoryHandle = _verticesMemory.Pin();

        return new nint(_verticesMemoryHandle.Value.Pointer);
    }

    public override void ResolveRenderTarget(RenderTarget surface, Tile[] tiles)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(ResolveRenderTarget));
#endif
        //_device.SetRenderTarget(null);
    }

    public override void SetRenderTarget(RenderTarget surface)
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(SetRenderTarget));
#endif
        if (surface is NoesisRenderTarget renderTarget)
        {
            _device.SetRenderTarget(renderTarget.RenderTarget2D);
        }
        else
        {
            throw new ArgumentException($"Surface is not a correct type, should be {nameof(NoesisRenderTarget)} but it is {surface.GetType().FullName}");
        }
    }

    public override void UnmapIndices()
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(UnmapIndices));
#endif

        _indicesMemoryHandle?.Dispose();
        _indicesMemoryHandle = null;
    }

    public override void UnmapVertices()
    {
#if DEBUG_LOG
        _logger.LogDebug(nameof(UnmapVertices));
#endif

        _verticesMemoryHandle?.Dispose();
        _verticesMemoryHandle = null;
    }

    public unsafe override void UpdateTexture(Texture texture, uint level, uint x, uint y, uint width, uint height, nint data)
    {        
        if (texture is NoesisTexture noesisTexture)
        {           
#if DEBUG_LOG
            _logger.LogDebug($"{nameof(UpdateTexture)} {noesisTexture.Label}: {level}, {x}, {y}, {width}, {height}");
#endif
            
            int pixelSize = NoesisTexture.GetPixelSizeForSurfaceFormat(noesisTexture.Texture.Format);
            int size = (int)(width * height * pixelSize);

            Span<byte> dataSpan = new(data.ToPointer(), size);

            noesisTexture.Texture.SetData((int)level, new((int)x, (int)y, (int)width, (int)height), dataSpan.ToArray(), 0, size);

#if DEBUG_LOG
            _logger.LogDebug($"{nameof(UpdateTexture)} data set");
#endif

#if EXPORT_TEXTURES
            using System.IO.StreamWriter writer = new (@$"D:\Projects\textures\{noesisTexture.Label}.png");

            noesisTexture.Texture.SaveAsPng(writer.BaseStream, noesisTexture.Texture.Width, noesisTexture.Texture.Height);
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Microsoft.Xna.Framework.Matrix FromNoesisToMonogame(Matrix4 matrix)
    {
        Microsoft.Xna.Framework.Matrix result = new(
            matrix[0][0], matrix[1][0], matrix[2][0], matrix[3][0],
            matrix[0][1], matrix[1][1], matrix[2][1], matrix[3][1],
            matrix[0][2], matrix[1][2], matrix[2][2], matrix[3][2],
            matrix[0][3], matrix[1][3], -matrix[2][3], matrix[3][3]);

        return result;
    }    
}
