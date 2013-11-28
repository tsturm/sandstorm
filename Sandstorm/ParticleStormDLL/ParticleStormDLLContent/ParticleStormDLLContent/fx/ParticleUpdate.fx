float ElapsedTime;
float ActiveParticles;
float TotalParticles;
float RenderTargetSize;
float3 PositionMin;
float3 PositionMax;
float LifeMin;
float LifeMax;
float StartSizeMin;
float StartSizeMax;
float EndSizeMin;
float EndSizeMax;
float3 ExternalForces;
float4 Field;

texture Positions;
sampler PositionSampler = sampler_state
{
    Texture   = <Positions>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture Velocities;
sampler VelocitySampler = sampler_state
{
    Texture   = <Velocities>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture Sizes;
sampler SizesSampler = sampler_state
{
    Texture   = <Sizes>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture Colors;
sampler ColorSampler = sampler_state
{
    Texture   = <Colors>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture StartColors;
sampler StartColorSampler = sampler_state
{
    Texture   = <StartColors>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture EndColors;
sampler EndColorSampler = sampler_state
{
    Texture   = <EndColors>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

struct VSInput
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position: POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct PSOutput
{
    float4 Position: COLOR0;
	float4 Velocity : COLOR1;
	float4 Size : COLOR2;
	float4 Color : COLOR3;
};

float rand(float2 co){
      return (frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453));
}

float nextFloat(float min, float max, float2 uv, float seed)
{
	return rand(uv*seed) * (max - min) + min;
}

float3 nextFloat3(float3 min, float3 max, float2 uv, float seed)
{
	float3 randomized;

	randomized.x = rand(uv*seed) * (max.x - min.x) + min.x;
	seed += 10.2487;
	randomized.y = rand(uv*seed) * (max.y - min.y) + min.y;
	seed += 7.7311;
	randomized.z = rand(uv*seed) * (max.z - min.z) + min.z;

	return randomized;
}

float3 nextFloat4(float4 min, float4 max, float2 uv, float seed)
{
	float4 randomized;

	randomized.x = rand(uv*seed) * (max.x - min.x) + min.x;
	seed += 10.2487;
	randomized.y = rand(uv*seed) * (max.y - min.y) + min.y;
	seed += 7.7311;
	randomized.z = rand(uv*seed) * (max.z - min.z) + min.z;
	seed += 12.1498;
	randomized.w = rand(uv*seed) * (max.z - min.z) + min.z;

	return randomized;
}

VSOutput PhysicsVS(VSInput Input)
{	
	VSOutput Output = (VSOutput)0;
	Output.Position = float4(Input.Position.xy, 0.0, 1.0);
	Output.TexCoord = Input.TexCoord;

	return Output;    
}

PSOutput PhysicsPS(VSOutput Input) : COLOR
{
	PSOutput Output = (PSOutput)0;
	float4 size = tex2D(SizesSampler, Input.TexCoord);
	float4 velocityData = tex2D(VelocitySampler, Input.TexCoord);
	float4 positionData = tex2D(PositionSampler, Input.TexCoord);

	float4 color = tex2D(ColorSampler, Input.TexCoord);
	float4 startColor = tex2D(StartColorSampler, Input.TexCoord);
	float4 endColor = tex2D(EndColorSampler, Input.TexCoord); 

	float3 position = positionData.xyz;
	float3 velocity = velocityData.xyz;
	float life = positionData.w;
	float lifeFull = velocityData.w;


	if(Input.TexCoord.y * RenderTargetSize + Input.TexCoord.x <= (ActiveParticles-1.0) * (RenderTargetSize/TotalParticles))
	{
		//Decrement life time
		life -= ElapsedTime;

		//Check if dead
		if(life <= 0.0)
		{
			velocity = nextFloat3(float3(-2, 10, -2), float3(2, 15, 2), Input.TexCoord, 1.5784);
			position = nextFloat3(PositionMin, PositionMax, Input.TexCoord, 12.4732);
			life = nextFloat(LifeMin, LifeMax, Input.TexCoord, 7.1581);
			float startSize = nextFloat(StartSizeMin, StartSizeMax, Input.TexCoord, 34.5424);
			float endSize = nextFloat(EndSizeMin, EndSizeMax, Input.TexCoord, 2.7522);
			size = float4(startSize, startSize, endSize, 0.0);
		} 
		else 
		{
			float ageRatio = 1.0 - (life / lifeFull);

			float sizeDelta = size.z - size.y;
			size.x = size.y + (sizeDelta * ageRatio);

			float4 colorDelta = endColor - startColor;
			color = startColor + (colorDelta * ageRatio);

			float3 fieldDir = Field.xyz - position;
			float3 fieldForce = normalize(fieldDir) * (Field.w / length(fieldDir));



			velocity += ElapsedTime * (ExternalForces + fieldForce);

			position += velocity * ElapsedTime;

			if(position.y<-20.0f)//kollision
			{
				float3 normal = float3(0.0f,1.0f,0.0f);
				velocity = (velocity - ((2.0f * dot(velocity, normal)) * normal));
				float friction = 0.1f;
				//velocity = (1.0f-friction)*velocity;
			}
		}
	}

	Output.Position = float4(position, life);
	Output.Velocity = float4(velocity, lifeFull);
	Output.Size = size;
	Output.Color = color;

	return Output;
}

technique Update
{
	pass PassMap
	{   
		VertexShader = compile vs_3_0 PhysicsVS();
		PixelShader  = compile ps_3_0 PhysicsPS();
	}
}