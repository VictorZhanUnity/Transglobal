
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using VictorDev.TCIT;
using Debug = VictorDev.Common.Debug;

public class RealtimeDataMediator : MonoBehaviour, RealtimeDataManager.IRealtimeDataReceiver
{
    public UnityEvent<float> currentRtAvgChanged = new UnityEvent<float>();
    public UnityEvent<float> currentRhAvgChanged = new UnityEvent<float>();
    
    private List<Data_Blackbox> _realtimeData;
    
    public void ReceiveRealtimeData(List<Data_Blackbox> data)
    {
        _realtimeData = data;
        CaculateAvergeValue("/RT/", currentRtAvgChanged);
        CaculateAvergeValue("/RH/", currentRhAvgChanged);
    }

    /// 計算即時溫度RT平均值
    private void CaculateAvergeValue(string keyword, UnityEvent<float> invokeEvent = null)
    {
        var filterData = _realtimeData.Where(data => data.tagName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        float avg = filterData.Average(data=> data.value ?? 0);
        invokeEvent?.Invoke(avg);
        Debug.Log($"{keyword} Average Value: {avg}", this, EmojiEnum.Brain);
    }
}
