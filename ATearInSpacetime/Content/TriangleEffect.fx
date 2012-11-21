
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = input.Position;
	output.Position.x *= 3.0/4.0;
	output.Color = input.Color;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	input.Color.rgb *= input.Color.a;
    return input.Color;
}

float4 PixelShaderFunctionDouble(VertexShaderOutput input) : COLOR0
{
	input.Color.rgb *= input.Color.a;
	input.Color.rgb += (float3(1,1,1)-input.Color.rgb)*input.Color.rgb;
    return input.Color;
}


technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
    
    pass Pass2
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunctionDouble();
    }
}
