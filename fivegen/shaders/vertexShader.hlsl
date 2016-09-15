struct VSOut {
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

float4x4 worldViewProj;

VSOut main(float4 position: POSITION, float4 color : COLOR){
	VSOut output;
	output.position = mul(position, worldViewProj);
	output.color = color;
	return output;
}