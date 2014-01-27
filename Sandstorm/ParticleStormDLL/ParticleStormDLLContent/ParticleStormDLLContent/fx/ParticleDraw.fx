float4x4 ViewMatrix;
float4x4 projMatrix;

texture Positions;
sampler positionSampler = sampler_state
{
    Texture   = <Positions>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture Colors;
sampler ColorSampler = sampler_state
{
    Texture   = <Colors>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture Sizes;
sampler SizesSampler = sampler_state
{
    Texture   = <Sizes>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture diffuseMap;
sampler diffuseSampler = sampler_state
{
    Texture   = <diffuseMap>;
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

struct VSInput
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position: POSITION;
	float2 TexCoord : TEXCOORD0;
	float Life : TEXCOORD1;
	float4 Color : COLOR0;
};

VSOutput DrawVS(VSInput Input, float2 positionTexCoord : BLENDWEIGHT)
{	
	VSOutput Output = (VSOutput)0;

	float3 xAxis = float3(ViewMatrix._11, ViewMatrix._12, ViewMatrix._13);
	float3 yAxis = float3(ViewMatrix._21, ViewMatrix._22, ViewMatrix._23);
	float3 zAxis = float3(ViewMatrix._31, ViewMatrix._32, ViewMatrix._33);
	float3 position = Input.Position.xyz;

	float4 realPosition = tex2Dlod(positionSampler, float4(positionTexCoord, 0.0, 0.0));
	float4 size = tex2Dlod(SizesSampler, float4(positionTexCoord, 0.0, 0.0));	

	//Scale and Translate Billboard
	position = position * size.x * 0.5 + ((realPosition.x * xAxis) + (realPosition.y * yAxis) + (realPosition.z * zAxis));

	//Align Billboard to viewpoint
	Output.Position = mul(float4(position + ViewMatrix[3].xyz , 1), projMatrix);

	Output.TexCoord = Input.TexCoord;
	Output.Color = tex2Dlod(ColorSampler, float4(positionTexCoord, 0.0, 0.0));
	Output.Life = realPosition.w;

	return Output;    
}

float4 DrawPS(VSOutput Input) : COLOR
{
	return Input.Color;
}

float4 DrawTexturedPS(VSOutput Input) : COLOR
{
	return Input.Color * tex2D(diffuseSampler, Input.TexCoord);
}

technique Draw
{
	pass PassMap
	{   
		VertexShader = compile vs_3_0 DrawVS();
		PixelShader  = compile ps_3_0 DrawPS();
	}
}

technique DrawTextured
{
	pass PassMap
	{   
		VertexShader = compile vs_3_0 DrawVS();
		PixelShader  = compile ps_3_0 DrawTexturedPS();
	}
}