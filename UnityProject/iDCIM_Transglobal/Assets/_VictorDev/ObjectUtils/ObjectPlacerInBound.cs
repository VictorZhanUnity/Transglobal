using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VictorDev.Common;

public class ObjectPlacerInBound : MonoBehaviour
{
    [Button]
    public void PlaceObjects()
    {
        if (ObjectHelper.CheckTargetsIsNull(BoxColliderTarget, objectToPlace)) return;

        RemoveAllChildren();

        // 取得區域的大小與起點位置（世界座標）
        Vector3 size = BoxColliderTarget.size;
        Vector3 center = BoxColliderTarget.center;
        Vector3 worldStart = BoxColliderTarget.transform.TransformPoint(center - size * 0.5f);
        Vector3 worldSize = Vector3.Scale(size, BoxColliderTarget.transform.lossyScale);

        for (float x = 0; x <= worldSize.x; x += spacing.x)
        {
            for (float y = 0; y <= worldSize.y; y += spacing.y)
            {
                for (float z = 0; z <= worldSize.z; z += spacing.z)
                {
                    Vector3 localPos = new Vector3(x, y, z);
                    Vector3 worldPos = worldStart + localPos;

                    // 檢查是否在 BoxCollider 的範圍內（可略過）
                    if (BoxColliderTarget.bounds.Contains(worldPos))
                    {
                        GameObject obj = Instantiate(objectToPlace, worldPos, Quaternion.identity);
                        obj.name += $"_{x},{y},{z}";
                        obj.transform.localScale = Vector3.one * scaleFactor;
                        obj.transform.parent = transform;
                        
                        onEachObjectPlaced?.Invoke(obj.transform);
                    }
                }
            }
        }
    }

    [Button]
    public void RemoveAllChildren() => ObjectHelper.RemoveAllChildInParent(transform);
    
    #region Variables
    
    [Header("[Event] - 每置放一個Item時Invoke")]
    public UnityEvent<Transform> onEachObjectPlaced = new();
    
    [Header("要放置的物件對像")]  
    public GameObject objectToPlace;

    [Foldout("[設定] - 間隔距離與物件Scale尺吋")]
    public Vector3 spacing = new Vector3(1f, 1f, 1f); // 每個軸向的間距
    [Foldout("[設定] - 間隔距離與物件Scale尺吋")]
    public float scaleFactor = 1f;
    
    /// 已放置的物件Item
    public List<Transform> ObjectList => transform.Cast<Transform>().ToList();
     
    private BoxCollider BoxColliderTarget => _boxCollider ??= GetComponent<BoxCollider>();
    [NonSerialized] private BoxCollider _boxCollider;
    #endregion
}