#include "ShaderVS.hlsl"
#include "ShaderPS.hlsl"

technique
{
   pass
   {
       VertexShader = compile VS_SHADERMODEL MainVS();
       PixelShader = compile PS_SHADERMODEL MainPS();
   }
};
