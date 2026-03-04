Shader "Custom/WaterMask"
{
    // ──────────────────────────────────────────────
    // 배 내부에 배치하여 물 셰이더를 마스킹하는 셰이더
    // 화면에는 아무것도 렌더링하지 않고
    // Stencil Buffer에 값만 기록합니다.
    // ──────────────────────────────────────────────

    Properties
    {
        // 에디터에서 Stencil 값을 조절할 수 있도록 노출
        _StencilRef ("Stencil Reference", Int) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "WaterMask"

            // ── 색상 출력 없음 (화면에 보이지 않음) ──
            ColorMask 0

            // ── 깊이 버퍼 기록 안 함 (다른 오브젝트에 영향 없음) ──
            ZWrite Off
            ZTest LEqual

            // ── Stencil: 이 영역에 마스크 값 기록 ──
            Stencil
            {
                Ref [_StencilRef]
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 아무것도 출력하지 않음 (ColorMask 0이므로 실제로 기록되지 않음)
                return half4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
