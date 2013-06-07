//-----------------------------------------------------------------------------
// Copyright (c) 2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------
//
// The alpha testing code in the pixel shader is taken from:
// http://create.msdn.com/en-US/education/catalog/sample/billboard
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Globals.
//-----------------------------------------------------------------------------

float4x4 world;
float4x4 view;
float4x4 projection;

float2 billboardSize;
float animationTime;
float animationScaleFactor;

//  1 means we should only accept non-transparent pixels.
// -1 means only accept transparent pixels.
float alphaTestDirection = 1.0f;
float alphaTestThreshold = 0.95f;



// Vertex shader input structure describes the start position and
// velocity of the particle, and the time at which it was created,
// along with some random values that affect its size and rotation.
struct VertexShaderInput
{
    float4 Position : POSITION;
};


// Vertex shader output structure specifies the position and color of the particle.
struct VertexShaderOutput
{
	float4 Position  : POSITION;
};



//-----------------------------------------------------------------------------
// Textures.
//-----------------------------------------------------------------------------

texture colorMap;
sampler colorMapSampler = sampler_state
{
	Texture = <colorMap>;
    MinFilter = Anisotropic;
	MagFilter = Linear;
    MipFilter = Linear;
    MaxAnisotropy = 16;
};

//-----------------------------------------------------------------------------
// Vertex shaders.
//-----------------------------------------------------------------------------

void VS_BillboardingCameraAligned(in  float4 inPosition  : POSITION,
                                  in  float4 inTexCoord  : TEXCOORD0,
			                      out float4 outPosition : POSITION,
			                      out float2 outTexCoord : TEXCOORD0)
{
	float4x4 worldViewProjection = mul(mul(world, view), projection);
	
	float2 offset = inTexCoord.zw;
	float3 xAxis = float3(view._11, view._21, view._31);
	float3 yAxis = float3(view._12, view._22, view._32);

	float animationDisplacement = inPosition.w * sin(animationTime) * animationScaleFactor;
	float3 pos = inPosition.xyz + ((offset.x + animationDisplacement) * xAxis) + (offset.y * yAxis);

	outPosition = mul(float4(pos, 1.0f), worldViewProjection);
	outTexCoord = inTexCoord.xy;
}


//-----------------------------------------------------------------------------
// Pixel shaders.
//-----------------------------------------------------------------------------

void PS_Billboarding(in  float2 inTexCoord : TEXCOORD0,
                     out float4 outColor   : COLOR)
{
	outColor = tex2D(colorMapSampler, inTexCoord);

	//outColor = float4(1,0,1,1);
	// Apply the alpha test.
	//clip((outColor.r - alphaTestThreshold) * alphaTestDirection);

}


// Custom vertex shader animates particles entirely on the GPU.
VertexShaderOutput ParticleVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;
	float3 pos = input.Position;
	output.Position = float4(pos, 1.0f);
	output.Position = float4(pos, 1.0f);
    
    return output;
}


//-----------------------------------------------------------------------------
// Techniques.
//-----------------------------------------------------------------------------

technique BillboardingCameraAligned
{
	pass P0
	{
		VertexShader = compile vs_2_0 ParticleVertexShader();		
	}
	pass P1
	{
		VertexShader = compile vs_2_0 VS_BillboardingCameraAligned();
		PixelShader = compile ps_2_0 PS_Billboarding();
	}
}
