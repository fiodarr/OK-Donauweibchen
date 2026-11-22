Shader "Custom/NewUnlitUniversalRFishingRodAimOverlay"
{
    Properties
    {
        _OverlayColor ("Overlay Color", Color) = (0, 0, 0, 0.35)

        _OriginUV ("Player Screen Position", Vector) = (0.5, 0.5, 0, 0)
        _AimDirection ("Aim Direction", Vector) = (1, 0, 0, 0)

        _ConeAngle ("Cone Angle", Float) = 50
        _EdgeSoftness ("Angular Edge Softness", Float) = 3

        _RangePixels ("Range in Pixels", Float) = 500
        _RangeSoftnessPixels ("Range Edge Softness", Float) = 20

        // Required for compatibility with Unity UI masking.
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)

                float4 _OverlayColor;
                float4 _OriginUV;
                float4 _AimDirection;

                float _ConeAngle;
                float _EdgeSoftness;

                float _RangePixels;
                float _RangeSoftnessPixels;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Convert UV coordinates into screen pixels.
                float2 pixelPosition = input.uv * _ScreenParams.xy;

                // Player position in screen pixels.
                float2 origin = _OriginUV.xy * _ScreenParams.xy;

                // Vector from player to the current pixel.
                float2 toPixel = pixelPosition - origin;

                float distanceFromPlayer = length(toPixel);

                // Normalize the direction towards the pixel.
                float2 directionToPixel;

                float2 aimDirection = normalize(_AimDirection.xy);

                if (distanceFromPlayer > 0.0001)
                {
                    directionToPixel = toPixel / distanceFromPlayer;
                }
                else
                {
                    directionToPixel = aimDirection;
                }

                // Compare the aiming direction with the pixel direction.
                float alignment = dot(aimDirection, directionToPixel);

                // Calculate the angular cone.
                float halfAngle = radians(_ConeAngle * 0.5);
                float edgeAngle = radians(max(_EdgeSoftness, 0.001));

                float outerCos = cos(min(halfAngle + edgeAngle, 3.14159265));
                float innerCos = cos(max(halfAngle - edgeAngle, 0.0));

                float coneMask = smoothstep(
                    outerCos,
                    innerCos,
                    alignment
                );

                // Calculate the maximum range with a soft edge.
                float rangeSoftness = max(_RangeSoftnessPixels, 0.001);

                float rangeMask = 1.0 - smoothstep(
                    max(0.0, _RangePixels - rangeSoftness),
                    _RangePixels,
                    distanceFromPlayer
                );

                // Only reveal pixels that are inside both the cone and range.
                float reveal = coneMask * rangeMask;

                // Outside the cone: dark.
                // Inside the cone: transparent.
                float alpha = _OverlayColor.a * (1.0 - reveal);

                return half4(
                    _OverlayColor.rgb,
                    alpha * input.color.a
                );
            }

            ENDHLSL
        }
    }
}
