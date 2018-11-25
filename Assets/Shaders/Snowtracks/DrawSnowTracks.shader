Shader "Tahar/DrawSnowTracks"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_BrushCoordinate("Brush location in UV coordinates", vector) = (0.0, 0.0, 0.0, 0.0)
		_BrushSize("Width of the tracks that will be drawn in the snow", Range(0.0, 0.25)) = 0.01
		_Snowfall("Speed at which existing tracks fill back up", Range(1.0, 500.0)) = 300.0
	}
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vertex_shader_main
			#pragma fragment fragment_shader_main

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _BrushSize;
			float _Snowfall;
			float4 _MainTex_ST;
			fixed4 _BrushCoordinate;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vertex_shader_main (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			// Replaces if (x < y)
			float when_lt(float x, float y) {
				return max(sign(y - x), 0.0);
			}
			
			fixed4 fragment_shader_main (v2f i) : SV_Target
			{
				// Base color of the object
				fixed4 color = tex2D(_MainTex, i.uv);

				// Distance in UV space to the draw coordinate
				float dist = distance(_BrushCoordinate, i.uv);

				fixed4 drawValue = fixed4(0.0, 0.0, 0.0, 0.0);

				// For now, the R value is either 1 or 0
				// TODO: Add variable strength based on ray length
				drawValue.r = 1.0 * when_lt(dist, _BrushSize);

				// Returns the brush color (red channel) combined with a snowfall value that acts as if a constant
				// layer of snow is being added on top of the existing snow, thus, creating the illusion of
				// heavy snowfall.
				return saturate(color + drawValue - fixed4(1.0 / _Snowfall * unity_DeltaTime.x, 0.0, 0.0, 0.0));
			}
			ENDCG
		}
	}
}
