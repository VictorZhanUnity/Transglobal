using System;
using _VictorDEV.Revit;
using TMPro;
using UnityEngine;

public class RackSpaceDisplayer : MonoBehaviour
{
    /// 接收機櫃資料
    public void ReceiveRackData(RackModelDataExtended rackModelDataExtended) => RackData = rackModelDataExtended;

    /// 設定為第幾U
    public void SetULocation(int uLevel)
    {
        ULevel = uLevel;
        TxtLocation.SetText($"U{ULevel}");
        transform.localPosition = StartPos + new Vector3(0, PosYHeight * (ULevel - 1), 0);
        name = $"機櫃空間 - U{uLevel}";
    }

    /// 顯示U層
    public void ShowULevel()
    {
        if (IsPinULocationVisible == false) UIObject.gameObject.SetActive(true);
    }

    /// 隱藏U層
    public void HideULevel()
    {
        if (IsPinULocationVisible == false) UIObject.gameObject.SetActive(false);
    }

    private void OnMouseExit()
    {
        RackData.AvailableUDisplayer.ForEach(displayer=> displayer.HideULevel());
    }

    private void Start() => HideULevel();

    #region Variables

    /// 機櫃資料
    public RackModelDataExtended RackData { get; private set; }

    /// 第幾U層
    public int ULevel { get; private set; }
    
    /// 是否定駐顯示U層數
    public bool IsPinULocationVisible
    {
        get => _isPinULocationVisible;
        set
        {
            _isPinULocationVisible = value;
            UIObject.gameObject.SetActive(_isPinULocationVisible);
        }
    }

    private bool _isPinULocationVisible = false;

    /// 機櫃裡第1U的起始位置
    private Vector3 StartPos => new Vector3(-0.00172f, 0.0992f, -0.3775994f);

    /// 每1U的高度Y
    private float PosYHeight => 0.0444973f;

    private Transform UIObject => _uIObject ??= transform.GetChild(0);
    private Transform _uIObject;

    private TextMeshProUGUI TxtLocation =>
        _txtLocation ??= UIObject.Find("TxtLocation").GetComponent<TextMeshProUGUI>();

    private TextMeshProUGUI _txtLocation;

    #endregion
}