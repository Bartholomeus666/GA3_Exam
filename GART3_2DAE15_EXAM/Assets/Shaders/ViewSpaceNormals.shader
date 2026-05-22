Shader "Hidden/ViewSpaceNormals"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Name "ViewSpaceNormals"

            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalVS   : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalVS = normalize(mul((float3x3)UNITY_MATRIX_MV, input.normalOS));
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                return float4(normalize(input.normalVS), 1.0);
            }

            ENDHLSL
        }
    }
}
