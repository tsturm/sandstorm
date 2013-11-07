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
	float3 posWS : TEXCOORD1;
};

VertexShaderOutput getParticleTexPos(float4 position:POSITION)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Position = position;
	output.posWS = position;
	output.posWS.x = position.x;
	output.posWS.y = position.y;
	output.posWS.z = position.z;

    return output;
}

float4 moveParticle(VertexShaderOutput input) : COLOR0
{
    //return float4(input.posWS.x/2, input.posWS.y, input.posWS.z, 1.0f);
	float4 mapPos = tex2D(positionSampler, float2(input.posWS.x,input.posWS.y));
	float4 mapForce = tex2D(forceSampler, float2(input.posWS.x,input.posWS.y));
	mapPos.x += mapForce.x;
	mapPos.y += mapForce.y;
	mapPos.z += mapForce.z;
	return mapPos;
}

float4 applyForces(VertexShaderOutput input) : COLOR0
{
	float4 mapForce = tex2D(forceSampler, float2(input.posWS.x,input.posWS.y));
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
}

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
