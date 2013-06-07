//-----------------------------------------------------------------------------
// InstancedModel.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Camera settings.
float4x4 world;
float4x4 view;
float4x4 projection;

//  1 means we should only accept non-transparent pixels.
// -1 means only accept transparent pixels.
float alphaTestDirection = 1.0f;
float alphaTestThreshold = 0.95f;


texture Texture;
sampler Sampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};


struct VertexShaderInput
{
    float4 Position : POSITION0; //Base pos
    float4 TextureCoordinate : TEXCOORD0; //Base Texture
	
	float3 iPosition : POSITION1; //Instance pos
	float3 iForce : POSITION2; //Instance force
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput InstancingBBVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
	float4x4 worldViewProjection = mul(mul(world, view), projection);

	float2 offset = input.TextureCoordinate.zw;
	float3 xAxis = float3(view._11, view._21, view._31);
	float3 yAxis = float3(view._12, view._22, view._32);

	offset *= 3.0;

	float3 pos = input.iPosition.xyz + (offset.x * xAxis) + (offset.y * yAxis);

	output.Position = mul(float4(pos, 1.0f), worldViewProjection);
	output.TextureCoordinate = input.TextureCoordinate.xy;
	output.Color = float4(1,1,1, saturate(length(input.iForce)));

    return output;
}

void PixelShaderFunction(VertexShaderOutput input, out float4 outColor : COLOR)                    
{
	outColor = tex2D(Sampler, input.TextureCoordinate) * ((normalize(input.Color) + 1.0) / 2.0);
}


// Hardware instancing technique.
technique InstancingBB
{
    pass P0
    {
        VertexShader = compile vs_3_0 InstancingBBVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}