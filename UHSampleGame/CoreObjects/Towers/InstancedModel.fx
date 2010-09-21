//-----------------------------------------------------------------------------
// InstancedModel.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// The maximum number of instances we can support when using the VFetchInstancing
// or ShaderInstancing technique is limited by the number of vertex shader constant
// registers. Shader model 2.0 has 256 constant registers: after using a couple for
// the camera and light settings, this leaves enough for 60 4x4 matrices. You will
// need to decrease this value if you add other shader parameters that also require
// constant registers, or if you want to use the ShaderInstancing technique with
// vertex shader 1.1 (which only has 96 constant registers). If you change this value,
// you must also update the constant at the top of InstancedModelPart.cs to match!

#define MAX_SHADER_MATRICES 60


// Array of instance transforms used by the VFetch and ShaderInstancing techniques.
float4x4 InstanceTransforms[MAX_SHADER_MATRICES];

// Single instance transform used by the NoInstancing technique.
float4x4 NoInstancingTransform;


// Camera settings.
float4x4 View;
float4x4 Projection;


// This sample uses a simple Lambert lighting model.
float3 LightDirection = normalize(float3(-1, -1, -1));
float3 DiffuseLight = 1.25;
float3 AmbientLight = 0.25;


texture Texture;

sampler Sampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};


// Vertex shader helper function shared between the different instancing techniques.
VertexShaderOutput VertexShaderCommon(VertexShaderInput input,
                                      float4x4 instanceTransform)
{
    VertexShaderOutput output;

    // Apply the world and camera matrices to compute the output position.
    float4 worldPosition = mul(input.Position, instanceTransform);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // Compute lighting, using a simple Lambert model.
    float3 worldNormal = mul(input.Normal, instanceTransform);
    
    float diffuseAmount = max(-dot(worldNormal, LightDirection), 0);
    
    float3 lightingResult = saturate(diffuseAmount * DiffuseLight + AmbientLight);
    
    output.Color = float4(lightingResult, 1);

    // Copy across the input texture coordinate.
    output.TextureCoordinate = input.TextureCoordinate;

    return output;
}


// On either platform, when instancing is disabled we can read
// the world transform directly from an effect parameter.
VertexShaderOutput NoInstancingVertexShader(VertexShaderInput input)
{
    return VertexShaderCommon(input, NoInstancingTransform);
}


#ifdef XBOX360


// On Xbox, we can use the GPU "vfetch" instruction to implement
// instancing. We perform arithmetic on the input index to compute
// both the vertex and instance indices.
int VertexCount;

VertexShaderOutput VFetchInstancingVertexShader(int index : INDEX)
{
    int vertexIndex = (index + 0.5) % VertexCount;
    int instanceIndex = (index + 0.5) / VertexCount;

    float4 position;
    float4 normal;
    float4 textureCoordinate;

    asm
    {
        vfetch position,          vertexIndex, position0
        vfetch normal,            vertexIndex, normal0
        vfetch textureCoordinate, vertexIndex, texcoord0
    };

    VertexShaderInput input;

    input.Position = position;
    input.Normal = normal;
    input.TextureCoordinate = textureCoordinate;

    return VertexShaderCommon(input, InstanceTransforms[instanceIndex]);
}


#else


// On Windows, we can use an array of shader constants to implement
// instancing. The instance index is passed in as part of the vertex
// buffer data, and we use that to decide which world transform should apply.
VertexShaderOutput ShaderInstancingVertexShader(VertexShaderInput input,
                                                float instanceIndex : TEXCOORD1)
{
    return VertexShaderCommon(input, InstanceTransforms[instanceIndex]);
}


// On Windows shader 3.0 cards, we can use hardware instancing, reading
// the per-instance world transform directly from a secondary vertex stream.
VertexShaderOutput HardwareInstancingVertexShader(VertexShaderInput input,
                                                float4x4 instanceTransform : TEXCOORD1)
{
    return VertexShaderCommon(input, transpose(instanceTransform));
}


#endif


// All the different instancing techniques share this same pixel shader.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return tex2D(Sampler, input.TextureCoordinate) * input.Color;
}


// Used on both platforms, for rendering without instancing.
technique NoInstancing
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 NoInstancingVertexShader();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}


#ifdef XBOX360


// Xbox instancing technique.
technique VFetchInstancing
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VFetchInstancingVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}


#else


// Windows instancing technique for shader 2.0 cards.
technique ShaderInstancing
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 ShaderInstancingVertexShader();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}


// Windows instancing technique for shader 3.0 cards.
technique HardwareInstancing
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}


#endif
