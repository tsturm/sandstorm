float ElapsedTime;
float ActiveParticles;
float TotalParticles;
float RenderTargetSize;
float3 PositionMin;
float3 PositionMax;
float3 VelocityMax;
float3 VelocityMin;
float LifeMin;
float LifeMax;
float StartSizeMin;
float StartSizeMax;
float EndSizeMin;
float EndSizeMax;
float4 StartColorMin;
float4 StartColorMax;
float4 EndColorMin;
float4 EndColorMax;
float3 ExternalForces;
float4 Field;

texture Terrain;
sampler TerrainSampler = sampler_state
{
    Texture   = <Terrain>;
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

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

float4 nextFloat4(float4 min, float4 max, float2 uv, float seed)
{
	float4 randomized;

	randomized.x = rand(uv*seed) * (max.x - min.x) + min.x;
	seed += 10.2487;
	randomized.y = rand(uv*seed) * (max.y - min.y) + min.y;
	seed += 7.7311;
	randomized.z = rand(uv*seed) * (max.z - min.z) + min.z;
	seed += 13.4578;
	randomized.w = rand(uv*seed) * (max.w - min.w) + min.w;

	return randomized;
}

float4 unpackColor(float color)
{
	float r = color / 16777216.0;
	float g = (color / 65536.0);
	float b = (color / 256.0) - 16776960.0;
	float a = color - 4294967040.0;
}

inline float4 EncodeFloatRGBA( float v ) {
  float4 enc = float4(1.0, 255.0, 65025.0, 160581375.0) * v;
  enc = frac(enc);
  enc -= enc.yzww * float4(1.0/255.0,1.0/255.0,1.0/255.0,0.0);
  return enc;
}

inline float DecodeFloatRGBA( float4 rgba ) {
  return dot( rgba, float4(1.0, 1/255.0, 1/65025.0, 1/160581375.0) );
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
			velocity = nextFloat3(VelocityMin, VelocityMax, Input.TexCoord, 1.5784);
			position = nextFloat3(PositionMin, PositionMax, Input.TexCoord, 12.4732);
			life = lifeFull = nextFloat(LifeMin, LifeMax, Input.TexCoord, 7.1581);
			float startSize = nextFloat(StartSizeMin, StartSizeMax, Input.TexCoord, 34.5424);
			float endSize = nextFloat(EndSizeMin, EndSizeMax, Input.TexCoord, 2.7522);
			size = float4(startSize, startSize, endSize, 0.0);
			startColor = color = nextFloat4(StartColorMin, StartColorMax, Input.TexCoord, 5.4548);
			endColor = nextFloat4(EndColorMin, EndColorMax, Input.TexCoord, 9.6472);
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

			float3 nextPosition = position + velocity * ElapsedTime;
			float heightPositionParticle = nextPosition.y;
			float heightPositionMap = tex2D(TerrainSampler, float2(nextPosition.x/420.0+0.5,nextPosition.z/420.0+0.5)).x*100;
			float distance = heightPositionParticle - heightPositionMap;
			if(distance<=0.0f)
			{
				float3 vec0 = float3(nextPosition.x,tex2D(TerrainSampler, float2(nextPosition.x/420.0+0.5,nextPosition.z/420.0+0.5)).x*100,nextPosition.z);
				float3 vec1 = float3(nextPosition.x+1,tex2D(TerrainSampler, float2((nextPosition.x+1)/420.0+0.5,nextPosition.z/420.0+0.5)).x*100,nextPosition.z);
				float3 vec2 = float3(nextPosition.x,tex2D(TerrainSampler, float2(nextPosition.x/420.0+0.5,(nextPosition.z+1)/420.0+0.5)).x*100,nextPosition.z+1);
				float3 normal = -normalize(cross(vec1-vec0,vec2-vec0));//float3(0.0f,1.0f,0.0f));
				velocity = (velocity - ((2.0f * dot(velocity, normal)) * normal));
				float friction = 1.0f;
				velocity = (1.0f-friction)*velocity;
				velocity += normal*abs(distance);
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