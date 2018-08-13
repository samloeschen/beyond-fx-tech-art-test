float3 hash3( float2 p )
{
    float3 q = float3( dot(p,float2(127.1,311.7)), 
                dot(p,float2(269.5,183.3)), 
                dot(p,float2(419.2,371.9)) );
    return frac(sin(q)*43758.5453);
}


float voronoise( in float2 x, float u, float v )
{
    float2 p = floor(x);
    float2 f = frac(x);
        
    float k = 1.0+63.0*pow(1.0-v,4.0);
    
    float va = 0.0;
    float wt = 0.0;
    for( int j=-2; j<=2; j++ )
    for( int i=-2; i<=2; i++ )
    {
        float2 g = float2( float(i),float(j) );
        float3 o = hash3( p + g )*float3(u,u,1.0);
        float2 r = g - f + o.xy;
        float d = dot(r,r);
        float ww = pow( 1.0-smoothstep(0.0,1.414,sqrt(d)), k );
        va += o.z*ww;
        wt += ww;
    }
    
    return va/wt;
}

float pcurve (float x, float a, float b )
{
    float k = pow(a+b,a+b) / (pow(a,a)*pow(b,b));
    return k * pow( x, a ) * pow( 1.0-x, b );
}

fixed4 fireRingFragment( float2 uv ) 
{
    float time = _Time.y;
    
    float size0 = 4.0;
    float speed0 = 1.5;
        
    float2 uvScaling = float2(2.5, 1.0);

    float2 uv0 = float2(uv.x, uv.y + (time * speed0)) * uvScaling * size0;
    float up0 = voronoise(uv0, 1.0, 0.45);

    
    float size1 = 8.0;
    float speed1 = 2.0;
    
    float2 uv1 = float2(uv.x, uv.y + ((time * speed1) + 100)) * uvScaling * size1;
    float up1 = voronoise(uv1, 0.8, 0.5);

    float size2 = 2.0;
    float speed2 = 6.0;
    

    float2 uv2 = float2(uv.x, uv.y + (time + 200 * speed2)) * uvScaling * size2;
    // float up2 = voronoise(uv2, 0.8, 0.5);

    float curve = pcurve(uv.y, 5.0, 1.0);
    float r = (up1 * up0) + curve;
    // r = up0;

    r *= curve;
    r *= 1.1;

    // r = up1;
    
    return fixed4(r, r, r, r);
}