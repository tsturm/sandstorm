//------------------------------------------------------
//--                                                  --
//--		   www.riemers.net                    --
//--   		    Basic shaders                     --
//--		Use/modify as you like                --
//--                                                  --
//------------------------------------------------------

struct VertexToPixel
{
    float4 Position   	: POSITION;
	float4 Color		: COLOR0;    
    float2 TexCoord		: TEXCOORD0;
	float4 PosWS		: TEXCOORD1;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
	
};

//------- Constants --------
float4x4 viewMatrix;
float4x4 projMatrix;
float4x4 worldMatrix;


//------- Texture Samplers --------

Texture heightMap;
float heightScale;
bool displayContours;
float contourSpacing;
float4 color0;
float4 color1;
float4 color2;
float4 color3;
sampler TextureSampler = sampler_state { texture = <heightMap>; magfilter = POINT; minfilter = POINT; mipfilter=POINT; AddressU = Clamp; AddressV = Clamp;};

//------- Technique: Terrain --------

VertexToPixel TerrainVS( float4 inPos : POSITION, float2 inTexCoord: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;

	float4x4 viewProjection = mul (viewMatrix, projMatrix);
	float4x4 worldViewProjection = mul (worldMatrix, viewProjection);

	float4x4 worldView = mul(worldMatrix, viewMatrix);

	float4 pos = inPos;
	float height = tex2Dlod(TextureSampler, float4(inTexCoord, 0, 0)).r;
	pos.y = height * heightScale;

	Output.Position = mul(pos, worldViewProjection);
	Output.PosWS = pos;
	Output.TexCoord = inTexCoord;

	if(height < 0.25) {
		Output.Color = lerp(color0, color1, height*4);
	} else if(height >= 0.25 && height < 0.4) {
		Output.Color = lerp(color1, color2, (height-0.25)*4);
	} else if(height >= 0.4) {
		Output.Color = lerp(color2, color3, (height-0.5)*4);
	}


	return Output;    
}

PixelToFrame TerrainPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = PSIn.Color;

	if(displayContours && fmod(PSIn.PosWS.y, contourSpacing) < 1.0)
	{
		Output.Color = float4(0.2 * PSIn.Color.rgb, 1.0);
	}

	return Output;
}

technique Terrain
{
	pass Pass0
	{   
		VertexShader = compile vs_3_0 TerrainVS();
		PixelShader  = compile ps_3_0 TerrainPS();
	}
}