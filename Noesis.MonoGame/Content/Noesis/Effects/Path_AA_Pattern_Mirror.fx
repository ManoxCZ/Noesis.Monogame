#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0

#include "ShaderVS-DesktopGL.hlsl"
#include "ShaderPS-DesktopGL.hlsl"
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

#include "ShaderVS-WindowsDX.hlsl"
#include "ShaderPS-WindowsDX.hlsl"
#endif

technique
{
   pass
   {
       VertexShader = compile VS_SHADERMODEL MainVS();
       PixelShader = compile PS_SHADERMODEL MainPS();
   }
};
