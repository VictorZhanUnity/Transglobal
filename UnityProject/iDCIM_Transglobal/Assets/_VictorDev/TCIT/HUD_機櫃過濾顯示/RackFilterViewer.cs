using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDEV.Revit;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using VictorDev.ColorUtils;
using VictorDev.MaterialUtils;
using Random = UnityEngine.Random;

/// HUD_機櫃條件過濾顯示
public class RackFilterViewer : MonoBehaviour, DeviceDataManager.IReceiverRackModelDataExtended
{
    [Header("[設定] - 機櫃過濾顏色符合條件優劣")] public List<ColorHelper.ColorLevel> colorLevels = new()
    {
        new(60f, ColorHelper.HexToColor(0x00FF00)),
        new(80f, ColorHelper.HexToColor(0xFFCE00)),
        new(100f, ColorHelper.HexToColor(0xFF0000)),
    };

    /// 接收機房的機櫃與其設備資料
    public void ReceiverData(List<RackModelDataExtended> data) => _rackDataList = data;

    [Button]
    private void Test_ReceiveSelectedDeviceData() => ReceiveSelectedDeviceData(selectedDeviceData);
    
    /// 接收目前選擇的設備資料
    public void ReceiveSelectedDeviceData(DeviceModelDataExtended deviceData)
    {
        selectedDeviceData = deviceData;
        gameObject.SetActive(selectedDeviceData != null);
        ToFilterRack();
    }

    #region 條件過濾機櫃顯示處理

    /// 過濾機櫃
    private void ToFilterRack()
    {
        if (selectedDeviceData == null) return;
        
        if (!ToggleWatt.isOn && !ToggleWeight.isOn && !ToggleHeightU.isOn)
        {
            ResetRacks();
        }
        else
        {
            _rackDataList.ForEach(rack =>
            {
                bool isSuitable = rack.IsDeviceSuitable(selectedDeviceData);
                ChangeRackHeight(rack, isSuitable);
                ChangeRackColor(rack, isSuitable);
            });
        }
    }

    /// 依照是否符合條件而調整機櫃大小
    private void ChangeRackHeight(RackModelDataExtended rackData, bool isSuitable)
    {
        DOTween.Kill(rackData.Model);
        rackData.Model
            .DOScaleY(isSuitable ? 1 : MinScale, TweenDuration * (isSuitable ? 1 : 0.5f))
            .SetEase(isSuitable ? EaseOut : EaseIn)
            .SetDelay(Random.Range(0f, TweenDuration)).SetAutoKill(true);
    }

    /// 依照是否符合條件而調整機櫃顏色與是否透明
    private void ChangeRackColor(RackModelDataExtended rackData, bool isSuitable)
    {
        int rackMaterialIndex = rackData.Model.name.Contains("ATEN") ? 7 : 0;
        int rackHoleIndex = rackData.Model.name.Contains("ATEN") ? 9 : 6;

        if (rackData.Model.name.Contains("DAMAC", StringComparison.OrdinalIgnoreCase))
        {
            rackMaterialIndex = 2;
            rackHoleIndex = -1;
        }

        Material[] mats = rackData.Model.GetComponent<MeshRenderer>().materials;

        for (int i = 0; i < mats.Length; i++)
        {
            Color color = mats[i].color;
            if (i == rackMaterialIndex) //當目前mat為機櫃外殼時處理
            {
                if (isSuitable == false)
                    color = rackData.Model.name.Contains("ATEN")
                        ? ColorHelper.HexToColor(0x515151)
                        : ColorHelper.HexToColor(0x565656);
                else
                {
                    //根據filter選項來取得剩餘資源百分八
                    List<float> filterUsagePercentList = new List<float>();
                    if (ToggleWatt.isOn) filterUsagePercentList.Add(rackData.UsagePercentOfWatt);
                    if (ToggleWeight.isOn) filterUsagePercentList.Add(rackData.UsagePercentOfWeight);
                    if (ToggleHeightU.isOn) filterUsagePercentList.Add(rackData.UsagePercentOfHeightU);

                    float percentage = filterUsagePercentList.Sum(value => value) / filterUsagePercentList.Count;
                    if (!ToggleWatt.isOn && !ToggleWeight.isOn && !ToggleHeightU.isOn)
                    {
                        color = rackData.Model.name.Contains("ATEN")
                            ? ColorHelper.HexToColor(0x515151)
                            : ColorHelper.HexToColor(0x565656);
                    }
                    else
                    {
                        color = ColorHelper.GetColorFromPercentage(percentage, colorLevels);
                    }
                }
            }

            color.a = isSuitable ? 0 : 1;
            if (rackMaterialIndex > 0 && isSuitable || i == rackHoleIndex) MaterialHelper.SetTransparentMode(mats[i]);
            else MaterialHelper.SetOpaqueMode(mats[i]);

            DOTween.Kill(mats[i]);
            mats[i].DOColor(color, TweenDuration).SetEase(isSuitable ? EaseOut : EaseIn).SetAutoKill(true);
        }
    }

    #endregion

    [Button]
    /// 重置所有機櫃樣式
    public void ResetRacks()
    {
        _rackDataList?.ForEach(rack =>
        {
            ChangeRackHeight(rack, true);
            ChangeRackColor(rack, false);
        });
    }

    #region Initialize

    private void Start()
    {
        ToggleWatt.onValueChanged.AddListener(_ => ToFilterRack());
        ToggleWeight.onValueChanged.AddListener(_ => ToFilterRack());
        ToggleHeightU.onValueChanged.AddListener(_ => ToFilterRack());
    }

    private void OnDisable()
    {
        ToggleWatt.onValueChanged.RemoveListener(_ => ToFilterRack());
        ToggleWeight.onValueChanged.RemoveListener(_ => ToFilterRack());
        ToggleHeightU.onValueChanged.RemoveListener(_ => ToFilterRack());
        ResetRacks();
    }

    #endregion

    #region Variables

    /// Dotween設定
    private float MinScale => 0.02f;

    private float TweenDuration => 0.3f;
    private Ease EaseOut => Ease.OutQuad;
    private Ease EaseIn => Ease.InQuad;

    /// 所有機櫃資料
    private List<RackModelDataExtended> _rackDataList;

    [Header("目前選擇的設備資料")]
    [SerializeField] private DeviceModelDataExtended selectedDeviceData;

    private Transform Container => _container ??= transform.Find("Container").Find("HLayout");
    [NonSerialized] private Transform _container;

    private Toggle ToggleWatt => _toggleWatt ??= Container.Find("ToggleWatt").GetComponent<Toggle>();
    [NonSerialized] private Toggle _toggleWatt;

    private Toggle ToggleWeight => _toggleWeight ??= Container.Find("ToggleWeight").GetComponent<Toggle>();
    [NonSerialized] private Toggle _toggleWeight;

    private Toggle ToggleHeightU => _toggleHeightU ??= Container.Find("ToggleHeightU").GetComponent<Toggle>();
    [NonSerialized] private Toggle _toggleHeightU;

    #endregion
}