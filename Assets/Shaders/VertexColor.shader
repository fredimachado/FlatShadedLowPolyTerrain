Shader "Custom/Vertex Color"
{
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "AutoLight.cginc"
	#include "Lighting.cginc"
	ENDCG

	SubShader
	{
		Pass
		{
			Lighting On
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma target 2.0

			struct appdata_t {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float4 tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float3 lightDirection : TEXCOORD1;
				float3 viewDirection : TEXCOORD2;
				fixed3 normalWorld : TEXCOORD3;
				LIGHTING_COORDS(4,6)
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;

				TANGENT_SPACE_ROTATION;
				o.lightDirection = mul(rotation, ObjSpaceLightDir(v.vertex));
				o.viewDirection = mul(rotation, ObjSpaceViewDir(v.vertex));

				o.normalWorld = normalize( mul(half4(v.normal, 0.0), unity_WorldToObject).xyz );
			 
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				 
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 normal = i.normalWorld.xyz * 2.0 - 1.0;

				float NdotL = dot(i.color.rgb, i.lightDirection);
				half atten = LIGHT_ATTENUATION(i);

				float brightness = max(dot(-i.lightDirection, normal), 0.0);
				return  UNITY_LIGHTMODEL_AMBIENT + (i.color * NdotL * atten * (_LightColor0 + (brightness * _LightColor0)));
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
 }
