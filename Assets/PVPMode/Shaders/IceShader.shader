// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Attack/Frozen"
{
	Properties
	{
		_MainTex("main tex",2D) = "black"{}
		_IceTex("ice tex",2D) = "black"{}
		_RimColor("rim color",Color) = (1,1,1,1)
		_RimPower("rim power",range(1,10)) = 2
		_IcePower("ice power",range(0,10)) = 1 
		_IceColor("ice color",Color) = (0,0,1,1)
	}

		SubShader
	{
		Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include"UnityCG.cginc"

		struct v2f
	{
		float4 vertex:POSITION;
		float4 uv:TEXCOORD0;
		float4 NdotV:COLOR;
	};

	sampler2D _MainTex;
	sampler2D _IceTex;
	float4 _RimColor;
	float _RimPower;
	float4 _IceColor;
	float _IcePower;

	v2f vert(appdata_base v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		float3 V = WorldSpaceViewDir(v.vertex);
		V = mul(unity_WorldToObject,V);
		o.NdotV.x = saturate(dot(v.normal,normalize(V)));
		return o;
	}

	half4 frag(v2f IN) :COLOR
	{
		float icePower = _IcePower / 10;
		half4 c = (1- icePower)*tex2D(_MainTex,IN.uv) + icePower *_IceColor*tex2D(_IceTex,IN.uv);
		c.rgb += pow((1 - IN.NdotV.x) ,_RimPower)* _RimColor.rgb;
		return c;
	}
		ENDCG
	}
	}
		FallBack "Diffuse"
}