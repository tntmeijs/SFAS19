Shader "Tahar/SnowTracks" {
	Properties{
		_TessellationFactor("Tessellation", Range(1,64)) = 16
		_DisplacementStrength("Displacement", Range(0, 1.0)) = 0.3

		_SnowSurfaceTexture("Surface texture on top of the snow", 2D) = "white" {}
		_SnowGroundTexture("Surface texture underneath the snow", 2D) = "white" {}
		_DisplacementTexture("Disp Texture", 2D) = "red" {}

		_SnowSurfaceBaseColor("Base color of the snow surface", color) = (1.0, 1.0, 1.0, 1.0)
		_SnowGroundBaseColor("Base color of the ground underneath the snow", color) = (1.0, 1.0, 1.0, 1.0)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 300

			CGPROGRAM
			#pragma surface surface_shader_main BlinnPhong addshadow fullforwardshadows vertex:vertex_shader_main tessellate:tessellate_shader_main nolightmap
			#pragma target 4.6

			float _TessellationFactor;
			float _DisplacementStrength;
			
			sampler2D _SnowSurfaceTexture;
			sampler2D _SnowGroundTexture;
			sampler2D _DisplacementTexture;

			fixed4 _SnowSurfaceBaseColor;
			fixed4 _SnowGroundBaseColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			float4 tessellate_shader_main()
			{
				return _TessellationFactor;
			}

			void vertex_shader_main(inout appdata v)
			{
				float displacementAmount = tex2Dlod(_DisplacementTexture, float4(v.texcoord.xy,0,0)).r * _DisplacementStrength;

				// Displace the current vertex along its normal
				v.vertex.xyz -= v.normal * displacementAmount;

				// To keep the lowest points in the snow on the same level as the collider, every vertex has to be
				// moved up by the depth (vertical distance) of the snow tracks.
				v.vertex.xyz += v.normal * _DisplacementStrength;
			}

			struct Input
			{
				float2 uv_SnowSurfaceTexture;
			};

			void surface_shader_main(Input IN, inout SurfaceOutput o)
			{
				// The base color is used to give the snow a custom color
				half4 snowSurfaceColor = tex2D(_SnowSurfaceTexture, IN.uv_SnowSurfaceTexture) * _SnowSurfaceBaseColor;
				half4 snowGroundColor = tex2D(_SnowGroundTexture, IN.uv_SnowSurfaceTexture) * _SnowGroundBaseColor;

				// Current depth of the snow layer
				float displacementAmount = tex2D(_DisplacementTexture, IN.uv_SnowSurfaceTexture);

				// The ground and surface textures should blend based on the depth between the two
				half4 combinedTextureColor = lerp(snowSurfaceColor, snowGroundColor, displacementAmount);

				o.Albedo = combinedTextureColor.rgb;
				o.Specular = 0.2;
				o.Gloss = 1.0;
			}
			ENDCG
		}
			FallBack "Diffuse"
}