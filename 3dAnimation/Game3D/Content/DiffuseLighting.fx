#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float3 lightPosition;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(mul(mul(input.Position, World), View), Projection);
	float4 fragPos = mul(mul(mul(World, View), Projection), input.Position);

	float4 norm = normalize(mul(World, input.Normal));
	float4 lightDir = normalize(float4(lightPosition, 0) - float4(fragPos[0], fragPos[1], fragPos[2], 1));

	float diff = max(dot(norm, lightDir)+0.5, 0);
	float4 diffusse = diff * float4(1, 1, 1, 1);

	output.Color = input.Color * (float4(0,0,0,0) + diffusse);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 result = input.Color;

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