Shader "01/Particle"
{
    SubShader
    {
        Pass
        {
            Tags
            {
                "RenderType" = "Opaque"
            }
            
            Blend SrcAlpha One

            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #pragma target 5.0

            struct Particle
            {
                float3 position;
                float3 velocity;
                float life;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float life : LIFE;
            };

            // particles' data
            StructuredBuffer<Particle> _ParticleBuffer;

            v2f vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
            {
                v2f o = (v2f)0;

                // Color
                float life = _ParticleBuffer[instance_id].life;
                float lerpVal = life * 0.25f;
                o.color = fixed4(1.0f - lerpVal + 0.1, lerpVal + 0.1, 1.0f, lerpVal);

                // Position
                o.position = UnityObjectToClipPos(float4(_ParticleBuffer[instance_id].position, 1.0f));

                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                return i.color;
            }
            ENDCG
        }
    }
}