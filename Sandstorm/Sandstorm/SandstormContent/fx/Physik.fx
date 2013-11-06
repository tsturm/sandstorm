float4x4 World;
float4x4 View;
float4x4 Projection;

float wavePos;
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


struct VertexShaderOutput
{
    float4 Position : POSITION;
	float3 posWS : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(float4 position:POSITION)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = position;
	output.posWS = position;
	output.posWS.x = position.x*100.0f*wavePos;
	output.posWS.y = position.y*100.0f*wavePos;
	output.posWS.z = 0.0f;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return float4(input.posWS.x, input.posWS.y, input.posWS.z, 1.0f);
}

technique Physik
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
