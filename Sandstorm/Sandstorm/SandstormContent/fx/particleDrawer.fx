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
float3	 BBSize;
bool	debug = false;

//  1 means we should only accept non-transparent pixels.
// -1 means only accept transparent pixels.
float alphaTestDirection = 1.0f;
float alphaTestThreshold = 0.2f;

texture positionMap;
sampler positionSampler  = sampler_state
{
    Texture   = <positionMap>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


texture Texture;
sampler Sampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


struct VertexShaderInput
{
    float4 Position : POSITION0; //Base pos
    float4 TextureCoordinate : TEXCOORD0; //Base Texture
	
	float2 iPosition : POSITION1; //Instance pos
//	float2 iForce : POSITION2; //Instance force
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


VertexShaderOutput InstancingBBVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
		
	
	float posX = input.iPosition.x;
	float posY = input.iPosition.y;
	
	float4 realPosition = tex2Dlod ( positionSampler, float4(posX,posY,0,0));
	
	
	//position Billboard
	float4x4 worldViewProjection = mul(mul(world, view), projection);
		
	float2 offset = input.TextureCoordinate.zw;
	float3 xAxis = float3(view._11, view._21, view._31);
	float3 yAxis = float3(view._12, view._22, view._32);
	float3 pos = realPosition.xyz + (offset.x * xAxis) + (offset.y * yAxis);

	output.Position = mul(float4(pos, 1.0f), worldViewProjection);
	output.TextureCoordinate = input.TextureCoordinate.xy;
	output.Color = float4(1,1,1,1);//float4(1,1,1, saturate(length(input.iForce)));	

	if(debug)
	{
		/*float l = length(input.iForce);
		if(l > 2.0f)
		{
			col.x = col.x+lerp(0.0f,0.3f,1/l);
			output.Color = HSVtoRGB(col);
		}*/
	}
	
	output.Color = float4(posX,posY,0.0f,1.0f);//normalize(float4(pos,0.5f-abs(pos.y))); //alte einfaerbung

    return output;
}

void PixelShaderFunction(VertexShaderOutput input, out float4 outColor : COLOR)                    
{
	outColor = (debug == true) ? input.Color : tex2D(Sampler, input.TextureCoordinate) * ((normalize(input.Color) + 1.0) / 2.0);
}



VertexShaderOutput PhysikVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
	
	output.Position = float4(1,1,1,1);
	output.TextureCoordinate = float2(1,1);;
	output.Color = float4(1,1,1,1);
    return output;
}
void PhysikShaderFunction(VertexShaderOutput input, out float4 outColor : COLOR)                    
{
	outColor = float4(1.0f,1.0f,1.0f,1.0f);
}

// Hardware instancing technique.
technique InstancingBB
{
    pass P0
    {
        VertexShader = compile vs_3_0 InstancingBBVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
	
   /* pass P1 //Physik
    {
        VertexShader = compile vs_3_0 PhysikVertexShader();
        PixelShader = compile ps_3_0 PhysikShaderFunction();
    }*/
}