
float pcurve (float x, float a, float b )
{
    float k = pow(a+b,a+b) / (pow(a,a)*pow(b,b));
    return k * pow( x, a ) * pow( 1.0-x, b );
}

float3 fireRingVertOffset( float2 uv, float3 normal, sampler2D noiseTex ) {
        float2 noiseUV = float2(uv.x, uv.y * 0.5) + float2(_Time.y * 0.8, _Time.y * 0.5);
        float noise = tex2Dlod(noiseTex, float4(noiseUV, 0, 0));
        return normal * noise * 1.0;
}

fixed4 fireRingFragment( float2 uv, sampler2D gradient, sampler2D voronoise0, sampler2D voronoise1 ) 
{
    float time = _Time.y;
    
    float size0 = 8.0;
    float speed0 = 1;
    float2 uv0 = float2(uv.x, uv.y + (time * speed0));
    float up0 = tex2D(voronoise0, uv0).r;
    
    float size1 = 10.0;
    float speed1 = 2.0;
    float2 uv1 = float2(uv.x - (time * 0.5), uv.y + ((time * speed1) + 20));
    float up1 = tex2D(voronoise1, uv1).r;

    float curve = pcurve(uv.y, 4.0, 1.0);
    float r = saturate((up1 * up0) + curve) + up1 * 0.18;
    r *= pcurve(uv.y, 2.0, 1.0) * 0.8;

    float3 rgb = tex2D(gradient, float2((1.0 - r), 1));
    float a = pcurve(r, 3.0, 2.0) * sqrt(1.0 - uv.y);
    return fixed4(rgb * r, saturate(a));
}