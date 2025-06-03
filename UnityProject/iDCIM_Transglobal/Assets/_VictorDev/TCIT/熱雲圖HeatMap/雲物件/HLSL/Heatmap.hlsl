void CustomCubeGrid_float(
float3 WorldPos,
float3 _GridSize,
float3 _CubeSize,
float _Spacing,
float3 _ColorLow,
float3 _ColorHigh,
float _HeightScale,
float _EmissionStrength,
float _Time,
out float4 Result)
{
    // 🔧 初始化 Result 為預設透明黑
    Result = float4(0, 0, 0, 0);

    float3 cellSize = _CubeSize + _Spacing;
    float3 gridPos = WorldPos / cellSize;
    float3 cellIndex = floor(gridPos);
    float3 localPos = frac(gridPos) * cellSize;

    // 若不在格子範圍內就直接 return（已預先初始化）
    if (any(cellIndex < 0) || any(cellIndex >= _GridSize))
        return;

    float heightValue = sin(_Time + cellIndex.x * 0.5 + cellIndex.z * 0.5);
    heightValue = saturate(heightValue) * _HeightScale;

    float alpha = step(localPos.x, _CubeSize.x) *
                  step(localPos.z, _CubeSize.z) *
                  step(localPos.y, _CubeSize.y * heightValue);

    float3 color = lerp(_ColorLow, _ColorHigh, heightValue);
    float3 emission = color * _EmissionStrength;

    Result = float4(emission, alpha);
}
