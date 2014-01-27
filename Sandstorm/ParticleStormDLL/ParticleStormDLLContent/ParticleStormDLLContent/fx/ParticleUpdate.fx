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
float MapWidth;
float MapHeight;
float HeightScale;

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

bool particleOnMap(float3 p, float height)
{
	float mapWidthOver2 = MapWidth/2;
	float mapHeightOver2 = MapHeight/2;

	return (p.x >= -mapWidthOver2 && p.x <= mapWidthOver2 && p.z >= -mapHeightOver2 && p.z <= mapHeightOver2 && p.y >= height);
}

float2 getHeightmapTexCoord(float3 p)
{
	float mapWidthOver2 = MapWidth/2;
	float mapHeightOver2 = MapHeight/2;

	return float2( (p.x + mapWidthOver2) / MapWidth, (p.z + mapHeightOver2) / MapWidth);
}

float3 calculateNormal(sampler sam, float2 uv)
{
	float texelOffsetU = 1.0/MapWidth;
	float texelOffsetV = 1.0/MapHeight;

	//Coeffiecent for smooth/sharp Normals
    float coef = 84.0;
	
	float s1 = tex2D(sam, float2(uv.x - texelOffsetU, uv.y)).x;	
	float s2 = tex2D(sam, float2(uv.x, uv.y - texelOffsetV)).x;
	float s3 = tex2D(sam, float2(uv.x + texelOffsetU, uv.y)).x;
	float s4 = tex2D(sam, float2(uv.x, uv.y + texelOffsetV)).x;

	//Calculate the Normal
	float3 normal = float3((s1 - s3) * coef, 2.0f, (s2 - s4) * coef);
	
	//Return normalized Normal
	return normalize(normal);
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
			//position = nextFloat3(PositionMin, PositionMax, Input.TexCoord, 12.4732);
			position = float3(99999, 99999, 99999);
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

			//===========================================================================
			// Physics stuff
			//===========================================================================

			//Get the related heightmap texture coordinate
			float2 heightmapTexCoord = getHeightmapTexCoord(position);

			//Get height at particle position 
			float height = tex2D(TerrainSampler, heightmapTexCoord).x * HeightScale;

			//Check if the Position is outside of our Map
			if (particleOnMap(position, height) == false)
			{
				//Kick it away
				position = float3(999999, 999999, 999999);
			}
			else 
			{
				//Calculate next particle position
				float3 nextPosition = position + velocity * ElapsedTime;

				//Get the related heightmap texture coordinate
				heightmapTexCoord = getHeightmapTexCoord(nextPosition);

				//Get height at next particle position 
				height = tex2D(TerrainSampler, heightmapTexCoord).x * HeightScale;

				//Check if the next Position is inside of our Map
				if (particleOnMap(nextPosition, height))
				{
					//Get normal at next particle position
					float3 normal = calculateNormal(TerrainSampler, heightmapTexCoord);

					//Calculate distance between the particle and the map
					float distance = nextPosition.y - height;

					//Calculate density
					float density = clamp(distance / 100.0, 0.0, 1.0);

					//
					normal = float3(0.0, density, 0.0) + ((1.0-density) * normal);

					//
					velocity = (velocity - (2.0 * dot(velocity, normal) * normal));

					//
					velocity.y += 0.5-density * 15;
				}
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