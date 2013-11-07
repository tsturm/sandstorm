float4x4 World;
float4x4 View;
float4x4 Projection;

texture positionMap;
texture forceMap;

sampler positionSampler  = sampler_state
{
    Texture   = <positionMap>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

sampler forceSampler  = sampler_state
{
    Texture   = <forceMap>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


struct VertexShaderOutput
{
    float4 Position : POSITION;
	float2 texCoord : TEXCOORD0;
};

VertexShaderOutput getParticleTexPos(float4 position:POSITION, float2 texCoord:TEXCOORD0)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Position = position;
	output.texCoord = texCoord;

    return output;
}

float4 moveParticle(VertexShaderOutput input) : COLOR0
{
	float4 mapPos = tex2D(positionSampler, input.texCoord);
	return mapPos;
}

/*float4 applyForces(VertexShaderOutput input) : COLOR0
{
	//float4 mapForce = tex2D(forceSampler, float2(input.posWS.x,input.posWS.y));
	mapForce.y += -0.01f;
	return mapForce;
}

technique Forces
{
	pass applyForces
    {
        VertexShader = compile vs_3_0 getParticleTexPos();
        PixelShader = compile ps_3_0 applyForces();
    }
}*/

technique Move
{
	pass Move
    {
        VertexShader = compile vs_3_0 getParticleTexPos();
        PixelShader = compile ps_3_0 moveParticle();
    }
	/*pass Collision
    {
        VertexShader = compile vs_3_0 getParticleTexPos();
        PixelShader = compile ps_3_0 moveParticle();
    }*/
}
