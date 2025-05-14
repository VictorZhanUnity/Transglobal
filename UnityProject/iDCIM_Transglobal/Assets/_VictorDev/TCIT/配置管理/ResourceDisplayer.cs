using System;
using _VictorDEV.Revit;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VictorDev.Advanced;
using VictorDev.DoTweenUtils;

public class ResourceDisplayer : MonoBehaviour
{
    [NonSerialized] private DeviceModelDataExtended _selectedDeviceModelData;

    /// 增加資源
    public void AddResource(DeviceModelDataExtended data)
    {
        _selectedDeviceModelData = data;
        PercentWatt.AddValue(data.information.watt);
        PercentWeight.AddValue(data.information.weight);
        PercentHeightU.AddValue(data.information.heightU);
        Cancel();
    }
    
    /// 減少資源
    public void DecreaseResource(DeviceModelDataExtended data)
    {
        string json = JsonUtility.ToJson(data);
        DeviceModelDataExtended newData = JsonUtility.FromJson<DeviceModelDataExtended>(json);

        newData.information.watt *= -1;
        newData.information.weight *= -1;
        newData.information.heightU *= -1;
        
        AddResource(newData);
    }

    /// 設定預備變更設備所耗的資源
    public void PrepareEditDevice(DeviceModelDataExtended data)
    {
        if(PrepareWatt.gameObject.activeSelf == false) PrepareWatt.ToShow();
        if(PrepareWeight.gameObject.activeSelf == false) PrepareWeight.ToShow();
        if(PrepareHeightU.gameObject.activeSelf == false) PrepareHeightU.ToShow();
        TxtPrepareWatt.SetText((data.information.watt * -1).ToString());
        TxtPrepareWeight.SetText((data.information.weight * -1).ToString());
        TxtPrepareHeightU.SetText((data.information.heightU * -1).ToString());

        TxtPrepareWatt.DOFade(1, 0.3f).SetEase(Ease.OutBounce).From(0);
        TxtPrepareWeight.DOFade(1, 0.3f).SetEase(Ease.OutBounce).From(0);
        TxtPrepareHeightU.DOFade(1, 0.3f).SetEase(Ease.OutBounce).From(0);
    }

    /// 取消預備變更設備所耗的資源
    public void Cancel()
    {
        _selectedDeviceModelData = null;
        PrepareWatt.gameObject.SetActive(false);
        PrepareWeight.gameObject.SetActive(false);
        PrepareHeightU.gameObject.SetActive(false);
    }

    private void Start() => Cancel();

    private ImagePercentSlider PercentWatt =>
        _percentWatt ??= transform.Find("ProgressBar_電力").GetComponent<ImagePercentSlider>();

    [NonSerialized] private ImagePercentSlider _percentWatt;

    private ImagePercentSlider PercentWeight =>
        _percentWeight ??= transform.Find("ProgressBar_負重").GetComponent<ImagePercentSlider>();

    [NonSerialized] private ImagePercentSlider _percentWeight;

    private ImagePercentSlider PercentHeightU =>
        _percentHeightU ??= transform.Find("ProgressBar_機櫃空間").GetComponent<ImagePercentSlider>();

    [NonSerialized] private ImagePercentSlider _percentHeightU;

    private DotweenFade2DWithEnabled PrepareWatt =>
        _prepareWatt ??= PercentWatt.transform.Find("Prepare").GetComponent<DotweenFade2DWithEnabled>();

    [NonSerialized] private DotweenFade2DWithEnabled _prepareWatt;

    private DotweenFade2DWithEnabled PrepareWeight => _prepareWeight ??=
        PercentWeight.transform.Find("Prepare").GetComponent<DotweenFade2DWithEnabled>();

    [NonSerialized] private DotweenFade2DWithEnabled _prepareWeight;

    private DotweenFade2DWithEnabled PrepareHeightU => _prepareHeightU ??=
        PercentHeightU.transform.Find("Prepare").GetComponent<DotweenFade2DWithEnabled>();

    [NonSerialized] private DotweenFade2DWithEnabled _prepareHeightU;


    private TextMeshProUGUI TxtPrepareWatt => _txtPrepareWatt ??=
        PrepareWatt.transform.Find("TxtPrepareWatt").GetComponent<TextMeshProUGUI>();

    [NonSerialized] private TextMeshProUGUI _txtPrepareWatt;

    private TextMeshProUGUI TxtPrepareWeight => _txtPrepareWeight ??=
        PrepareWeight.transform.Find("TxtPrepareWeight").GetComponent<TextMeshProUGUI>();

    [NonSerialized] private TextMeshProUGUI _txtPrepareWeight;

    private TextMeshProUGUI TxtPrepareHeightU => _txtPrepareHeightU ??=
        PrepareHeightU.transform.Find("TxtPrepareHeightU").GetComponent<TextMeshProUGUI>();

    [NonSerialized] private TextMeshProUGUI _txtPrepareHeightU;
}