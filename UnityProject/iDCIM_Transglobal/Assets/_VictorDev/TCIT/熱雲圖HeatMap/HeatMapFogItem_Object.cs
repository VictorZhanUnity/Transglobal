
using System;
using UnityEngine;

public class HeatMapFogItem_Object : MonoBehaviour//, IHeatMapFogItem
{
    public void SetColor(Color color) => RendererTarget.material.color = color;

    public void SetValue(int value) => Value = value;
    public void AdjustValue(int value) => Value += value;
    public void SetColorRange(Color minColor, Color maxColor)
    {
        throw new NotImplementedException();
    }

    /// 權重值
    public int Value { get; private set; }

    private Renderer RendererTarget => _renderer ??= GetComponent<Renderer>();
    [NonSerialized] private Renderer _renderer;
}