struct VsIn
{
    float2 position: POSITION;

#if HAS_COLOR
    half4 color: COLOR0;
#endif

#if HAS_UV0
    float2 uv0: TEXCOORD0;
#endif

#if HAS_UV1
    float2 uv1: TEXCOORD1;
#endif

#if HAS_COVERAGE
    half coverage: NORMAL;
#endif

#if HAS_RECT
    float4 rect: BINORMAL;
#endif

#if HAS_TILE
    float4 tile: TANGENT;
#endif

#if HAS_IMAGE_POSITION
    float4 imagePos: NORMAL1;
#endif

#if STEREO_RENDERING
    uint eyeIndex : SV_InstanceID;
#endif
};

struct VsOut
{
    float4 position: SV_POSITION;

#if HAS_COLOR
    nointerpolation half4 color: COLOR;
#endif

#if HAS_UV0
    float2 uv0: TEXCOORD0;
#endif

#if HAS_UV1
    float2 uv1: TEXCOORD1;
#endif

#if DOWNSAMPLE
    float2 uv2: TEXCOORD2;
    float2 uv3: TEXCOORD3;
#endif

#if SDF
    float4 st1: TEXCOORD2;
#endif

#if HAS_COVERAGE
    half coverage: NORMAL0;
#endif

#if HAS_RECT
    nointerpolation float4 rect: BINORMAL;
#endif

#if HAS_TILE
    nointerpolation float4 tile: TANGENT;
#endif

#if HAS_IMAGE_POSITION
    float4 imagePos: NORMAL1;
#endif

#if STEREO_RENDERING
    uint renderTargetIndex: SV_RenderTargetArrayIndex;
#endif
};

#define BEGIN_CONSTANTS cbuffer Buffer0: register(b0) {
#if STEREO_RENDERING
    float4x4 projectionMtx[2];
 #else
    float4x4 projectionMtx;
#endif
#define END_CONSTANTS }

float2 textureSize;

// warning X3571: pow(f, e) will not work for negative f
#pragma warning (disable : 3571)

float SRGBToLinear(float v)
{
    if (v <= 0.04045)
    {
      return v * (1.0 / 12.92);
    }
    else
    {
      return pow( v * (1.0 / 1.055) + 0.0521327, 2.4);
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
VsOut MainVS(in VsIn i)
{
    VsOut o;

#if STEREO_RENDERING
    o.position = mul(float4(i.position, 0, 1), projectionMtx[i.eyeIndex]);
    o.renderTargetIndex = i.eyeIndex;
#else
    o.position = mul(float4(i.position, 0, 1), projectionMtx);
#endif

#if HAS_COLOR
  #if LINEAR_COLOR_SPACE
    o.color.r = SRGBToLinear(i.color.r);
    o.color.g = SRGBToLinear(i.color.g);
    o.color.b = SRGBToLinear(i.color.b);
    o.color.a = i.color.a;
  #else
    o.color = i.color;
  #endif
#endif

#if DOWNSAMPLE
    o.uv0 = i.uv0 + float2(i.uv1.x, i.uv1.y);
    o.uv1 = i.uv0 + float2(i.uv1.x, -i.uv1.y);
    o.uv2 = i.uv0 + float2(-i.uv1.x, i.uv1.y);
    o.uv3 = i.uv0 + float2(-i.uv1.x, -i.uv1.y);
#else
    #if HAS_UV0
      o.uv0 = i.uv0;
    #endif
    #if HAS_UV1
      o.uv1 = i.uv1;
    #endif
#endif

#if SDF
    o.st1 = float4(i.uv1 * textureSize.xy, 1.0 / (3.0 * textureSize.xy));
#endif

#if HAS_COVERAGE
    o.coverage = i.coverage;
#endif

#if HAS_RECT
    o.rect = i.rect;
#endif

#if HAS_TILE
    o.tile = i.tile;
#endif

#if HAS_IMAGE_POSITION
    o.imagePos = i.imagePos;
#endif

    return o;
}