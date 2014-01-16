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
sampler TextureSampler = sampler_state { texture = <heightMap>; magfilter = POINT; minfilter = POINT; mipfilter=POINT; AddressU = Clamp; AddressV = Clamp;};

float heightScale;
bool displayContours;
float contourSpacing;
float4 color0;
float4 color1;
float4 color2;
float4 color3;
float textureWidth;
float textureHeight;


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

PixelToFrame TerrainPS(VertexToPixel Input) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = Input.Color;

	//Quelle hierfür: http://idav.ucdavis.edu/~okreylos/ResDev/SARndbox/
	if(displayContours)
	{
		float2 fragCoord = Input.TexCoord;
		float pixelOffset = 1.0f/textureWidth;
		float contourLineFactor = 1.0f/contourSpacing;
		
		/* Calculate the contour line interval containing each pixel corner by evaluating the half-pixel offset elevation texture: */
		float corner0=floor(tex2D(TextureSampler,float2(fragCoord.x,fragCoord.y)).r*256.0f*contourLineFactor);
		float corner1=floor(tex2D(TextureSampler,float2(fragCoord.x+pixelOffset,fragCoord.y)).r*256.0f*contourLineFactor);
		float corner2=floor(tex2D(TextureSampler,float2(fragCoord.x,fragCoord.y+pixelOffset)).r*256.0f*contourLineFactor);
		float corner3=floor(tex2D(TextureSampler,float2(fragCoord.x+pixelOffset,fragCoord.y+pixelOffset)).r*256.0f*contourLineFactor);
	
		/* Find all pixel edges that cross at least one contour line: */
		int edgeMask=0;
		int numEdges=0;
		if(corner0!=corner1)
			{
			edgeMask+=1;
			++numEdges;
			}
		if(corner2!=corner3)
			{
			edgeMask+=2;
			++numEdges;
			}
		if(corner0!=corner2)
			{
			edgeMask+=4;
			++numEdges;
			}
		if(corner1!=corner3)
			{
			edgeMask+=8;
			++numEdges;
			}
	
		/* Check for all cases in which the pixel should be colored as a topographic contour line: */
		if(numEdges>2||edgeMask==3||edgeMask==12||(numEdges==2&&fmod(floor(fragCoord.x)+floor(fragCoord.y),2.0)==0.0))
		{
			/* Topographic contour lines are rendered in black: */
			Output.Color=float4(Input.Color.rgb * 0.6,1.0);
		}
	}

	return Output;
}

technique Terrain
{
	pass PassMap
	{   
		VertexShader = compile vs_3_0 TerrainVS();
		PixelShader  = compile ps_3_0 TerrainPS();
	}
}