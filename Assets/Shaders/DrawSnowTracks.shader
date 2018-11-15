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
			#pragma vertex vert
			#pragma fragment frag

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);

				float dist = distance(_BrushCoordinate, i.uv);

				fixed4 drawValue = fixed4(0.0, 0.0, 0.0, 0.0);

				if (dist < _BrushSize)
					drawValue.r = 1.0;

				return saturate(color + drawValue - fixed4(1.0 / _Snowfall * unity_DeltaTime.x, 0.0, 0.0, 0.0));
			}
			ENDCG
		}
	}
}
