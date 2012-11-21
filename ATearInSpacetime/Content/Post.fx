uniform extern texture ScreenTexture;

sampler textureSampler = sampler_state
{
	Texture = <ScreenTexture>;	
	minfilter = LINEAR;
};

shared float t;
shared float2 offset;

float4 FilmGrain(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color;
	offset.x *= 3.0/4.0;
	color.a = 1;
	color.r = tex2D(textureSampler,texCoord+offset).r;
	color.b = tex2D(textureSampler,texCoord+offset+float2(0.0025,0)).b;
	color.g = tex2D(textureSampler,texCoord+offset+float2(0,0.003)).g;
	float x = texCoord.x * texCoord.y * t * 1000;
	x = fmod(x, 13) * fmod(x, 123);	
	x = fmod(x, 0.01);
	x *= 100;
	
	color.rgb *= (((texCoord.x*341.333333)%1)+1)/2;
	//color.g *= ((((texCoord.x+0.0025)*3401.333333)%1)+1)/2;
	
	float3 lerpfactor = pow(color.rgb, 0.15);
	color.rgb = lerpfactor * color.rgb + (1-lerpfactor)*x*color.rgb*2;
	
	return color * 1.25f;
}

technique T1
{
	pass P1
	{
		PixelShader = compile ps_2_0 FilmGrain();
	}
		
}