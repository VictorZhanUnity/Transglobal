using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VictorDev.MaterialUtils
{
    public class MeshGroupFinder : MonoBehaviour
    {
        [ContextMenu("- 尋找相鄰的模型")]
        private void FindClosetMeshGroup()
        {
            modelFindGroup.Clear(); // 清空之前的搜尋結果

            if (targetModel == null)
            {
                Debug.LogError("請設定 Target Model！");
                return;
            }

            // 初始化搜尋隊列和已檢查的集合
            Queue<GameObject> toCheck = new Queue<GameObject>();
            HashSet<GameObject> checkedObjects = new HashSet<GameObject>();

            // 將目標物件加入隊列
            toCheck.Enqueue(targetModel);
            checkedObjects.Add(targetModel);

            // 開始搜索
            while (toCheck.Count > 0)
            {
                // 取出隊列中的第一個物件
                GameObject current = toCheck.Dequeue();

                // 將其加入結果列表
                modelFindGroup.Add(current.transform);

                // 獲取當前物件的 Renderer 和邊界
                Renderer currentRenderer = current.GetComponent<Renderer>();
                if (currentRenderer == null) continue;
                Bounds currentBounds = currentRenderer.bounds;

                // 遍歷場景中的所有物件
                foreach (GameObject obj in AllObjects)
                {
                    // 跳過已檢查的物件
                    if (checkedObjects.Contains(obj)) continue;

                    // 確保該物件有 Renderer
                    Renderer objRenderer = obj.GetComponent<Renderer>();
                    if (objRenderer == null) continue;

                    currentBounds.Expand(BoundsExpansion);
                    // 先判斷邊界是否相交
                    if (currentBounds.Intersects(objRenderer.bounds))
                    {
                        // 再判斷包含關鍵字與排除關鍵字
                        if (keywords.Any(word => obj.name.Contains(word)) == false ||
                            keywordExclude.Any(word => obj.name.Contains(word)))
                        {
                            continue;
                        }

                        // 將物件加入隊列並標記為已檢查
                        toCheck.Enqueue(obj);
                        checkedObjects.Add(obj);
                        //obj.tag = tagName;
                    }
                }
            }

            // 後檢查名稱條件
            //group = group.Where(target=>keywords.Any(word=>target.name.Contains(word))).ToList();

            Debug.Log($"找到一組包含 {modelFindGroup.Count} 個物件的群組。");
        }

        [ContextMenu("- 顯示管線(不會先恢復所有模型材質)")]
        //  public void ToShow() => ModelMaterialHandler.ToShow(modelFindGroup);

        [ContextMenu("- 添加Collider")]
        public void AddCollider()
        {
            modelFindGroup.ForEach(target =>
            {
                if (target.TryGetComponent(out MeshCollider meshCollider) == false)
                {
                    target.gameObject.AddComponent<MeshCollider>();
                }
            });
        }

        [ContextMenu("- 移除Collider")]
        public void RemoveCollider()
        {
            modelFindGroup.ForEach(target =>
            {
                if (target.TryGetComponent(out MeshCollider meshCollider))
                {
                    DestroyImmediate(meshCollider);
                }
            });
        }

        #region Components

        // 目標物件，從此物件開始依規則向外偵測
        [Header(">>> 目標物件")] public GameObject targetModel;

        // 指定的名稱條件 (只有名稱包含此字串的物件才會被納入群組)
        [Header("需包含任一關鍵字")] public List<string> keywords = new List<string>()
        {
            "彎頭", "異徑", "T_接頭", "三通"
        };

        // 需排除的關鍵字
        [Header("需排除的關鍵字")] public List<string> keywordExclude;

        // 找到的鄰近群組清單 (在 Inspector 顯示)
        [Header("[僅顯示] - 相鄰的模型物件")] [SerializeField]
        private List<Transform> modelFindGroup = new List<Transform>();
        public List<Transform> ModelFindGroup => modelFindGroup;

        /// 每個物件的偵測範圍
        private const float BoundsExpansion = 0.00001f;

        /// 場景上所有物件，以PosY遞減排序
        private List<GameObject> AllObjects =>
            _allObjects ??= FindObjectsByType<GameObject>(FindObjectsSortMode.None).OrderByDescending(obj=> obj.transform.position.y).ToList();
        private List<GameObject> _allObjects;

        #endregion
    }
}