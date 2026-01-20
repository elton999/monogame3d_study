#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define MAX_BONES 90

float4x4 World;
float4x4 View;
float4x4 Projection;

Texture2D SpriteTexture;

matrix Bones[MAX_BONES];

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
    float4 Joints : BLENDINDICES0;
    float4 Weights : BLENDWEIGHT0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD1;
};

float4 LambertShading(float4 colorRefl, float lightInt, float4 normal, float4 lightDir)
{
    return colorRefl * lightInt * max(0, dot(normal, lightDir));
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    // Skinning 
    float4 skinnedPos = float4(0, 0, 0, 0);
    skinnedPos += mul(input.Position, Bones[int(input.Joints.x)]) * input.Weights.x;
    skinnedPos += mul(input.Position, Bones[int(input.Joints.y)]) * input.Weights.y;
    skinnedPos += mul(input.Position, Bones[int(input.Joints.z)]) * input.Weights.z;
    skinnedPos += mul(input.Position, Bones[int(input.Joints.w)]) * input.Weights.w;

    // World → View → Projection
    float4 worldPos = mul(skinnedPos, World);
    float4 viewPos = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);

    output.TextureCoordinates = input.TextureCoordinates;
    output.Color = input.Color;

    return output;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 result = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    return result;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};