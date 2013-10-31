float4x4 World;
float4x4 View;
float4x4 Projection;

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
	float2 TexCoord : TEXCOORD0;
	float3 posWS : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(float4 position:POSITION, float2 tex:TEXCOORD0)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = position;
	output.posWS = position;
	output.TexCoord = tex;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return float4(sin(input.posWS.x)*10.0f, cos(input.posWS.y)*10.0f, cos(input.posWS.z)*10.0f, 1.0f);
}

technique Physik
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
