Shader "Unlit/MovementOptions"
{
    Properties
    {
        _RhythmTime ("Time [0.0-1.0]", Range(0.0, 1.0)) = 0.0
        _Color ("Color", vector) = (1, 0, 0, 1) 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vertex_input
            {
                float4 vertex : POSITION;
            };

            struct fragment_input
            {
                float4 vertex : SV_POSITION;
            };

            float _RhythmTime;

            fragment_input vert (vertex_input v)
            {
                if (v.vertex.x == 1.0 || v.vertex.x == -1)
                {
                    v.vertex.x = lerp(0.333333, 1.0, _RhythmTime) * v.vertex.x;
                } else if(v.vertex.z == 1.0 || v.vertex.z == -1.0)
                {
                    v.vertex.z = lerp(0.333333, 1.0, _RhythmTime) * v.vertex.z;
                }
                fragment_input o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 _Color;

            fixed4 frag (fragment_input i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
