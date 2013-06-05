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

//http://ploobs.com.br/?p=1499

float3 Hue(float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R,G,B));
}

float4 HSVtoRGB(in float3 HSV)
{
    return float4(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z,1);
}

float4 RGBtoHSV(in float3 RGB)
{
    float3 HSV = 0;
    HSV.z = max(RGB.r, max(RGB.g, RGB.b));
    float M = min(RGB.r, min(RGB.g, RGB.b));
    float C = HSV.z - M;
    if (C != 0)
    {
        HSV.y = C / HSV.z;
        float3 Delta = (HSV.z - RGB) / C;
        Delta.rgb -= Delta.brg;
        Delta.rg += float2(2,4);
        if (RGB.r >= HSV.z)
            HSV.x = Delta.b;
        else if (RGB.g >= HSV.z)
            HSV.x = Delta.r;
        else
            HSV.x = Delta.g;
        HSV.x = frac(HSV.x / 6);
    }
    return float4(HSV,1);
}



// Vertex shader helper function shared between the two techniques.
VertexShaderOutput InstancingBBVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
	float4x4 worldViewProjection = mul(mul(world, view), projection);
	
	float2 offset = input.TextureCoordinate.zw;
	float3 xAxis = float3(view._11, view._21, view._31);
	float3 yAxis = float3(view._12, view._22, view._32);

	float3 pos = input.iPosition.xyz + (offset.x * xAxis) + (offset.y * yAxis);

	output.Position = mul(float4(pos, 1.0f), worldViewProjection);
	output.TextureCoordinate = input.TextureCoordinate.xy;

	output.Color = float4(1.0f,1.0f,1.0f,1.0f);

	float3 col = float3(0.0f,1.0f,1.0f);//green

	float l = length(input.iForce);
	if(l > 2.0f)
	{
		col.x = col.x+lerp(0.0f,0.3f,1/l);//
		output.Color = HSVtoRGB(col);
	}
	
	//output.Color = normalize(float4(pos,0.5f-abs(pos.y))); //alte einfaerbung

    return output;
}



// Both techniques share this same pixel shader.
void PixelShaderFunction(VertexShaderOutput input,out float4 outColor : COLOR)                    
{
	//outColor = tex2D(Sampler, input.TextureCoordinate) * input.Color;

	// Apply the alpha test.
	//clip((outColor.a - alphaTestThreshold) * alphaTestDirection);
	//clip(outColor.a);

	outColor = input.Color;//input.Color.a;
	//TODO: Blending
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