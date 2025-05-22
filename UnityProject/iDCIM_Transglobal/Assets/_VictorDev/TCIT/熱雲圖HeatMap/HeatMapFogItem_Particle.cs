using System;
using UnityEngine;

public class HeatMapFogItem_Particle : MonoBehaviour, IHeatMapFogItem
{
    public void SetColor(Color color)
    {
        var mainModule = ParticleSystem.main;
        mainModule.startColor = color;
        ParticleSystem.Play();
    }

    public void SetValue(int value) => Value = value;
    public void AdjustValue(int value) => Value += value;

    /// 權重值
    public int Value { get; private set; }

    private ParticleSystem ParticleSystem => _particleSystem ??= GetComponent<ParticleSystem>();
    [NonSerialized] private ParticleSystem _particleSystem;
}