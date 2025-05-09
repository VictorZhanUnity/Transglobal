using System.Collections.Generic;
using UnityEngine;

namespace VictorDev.MaterialUtils
{
    /// 處理3D物件的材質替換
    public static class MaterialReplacer
    {
        #region Replace Material

        /// 替換物件及其底下每層所有子物件的材質 {排除的對像(選填)}
        public static void ReplaceMaterialRecursively(Transform target, Material material,
            List<Transform> excludeTargets = null)
        {
            if (excludeTargets != null)
            {
                //當目標對像不在排除名單內時
                if (excludeTargets.Contains(target) == false)
                {
                    ReplaceMaterial(target, material);
                    // 遞迴處理所有子物件
                    foreach (Transform child in target)
                    {
                        ReplaceMaterialRecursively(child, material, excludeTargets);
                    }
                }
            }
            else
            {
                //當沒有排除名單時，直接替換
                ReplaceMaterial(target, material);
                // 遞迴處理所有子物件
                foreach (Transform child in target)
                {
                    ReplaceMaterialRecursively(child, material);
                }
            }


            // 只要是排除對像，其對像底下所有子物件皆被排除
            /*if (excludeTargets != null && excludeTargets.Contains(target) == false)
            {
                ReplaceMaterial(target, material);
                // 遞迴處理所有子物件
                foreach (Transform child in target)
                {
                    ReplaceMaterialRecursively(child, material, excludeTargets);
                }
            }*/
        }

        /// 替換Targets(陣列)為指定材質
        public static void ReplaceMaterial(List<Transform> targets, Material replaceMaterial) =>
            targets.ForEach(target => ReplaceMaterial(target, replaceMaterial));

        /// 替換Targets(陣列)為指定材質
        public static void ReplaceMaterial(Transform target, Material replaceMaterial)
        {
            if (target.TryGetComponent(out Renderer render))
            {
                // 如果尚未保存原始材質，將它的材質陣列存儲到字典中
                _originalMaterials.TryAdd(target, render.sharedMaterials);

                // 進行材質替換
                if (render.sharedMaterials.Length > 1)
                {
                    // 如果有多個材質，建立新的材質陣列
                    Material[] newMaterials = new Material[render.sharedMaterials.Length];
                    for (int i = 0; i < newMaterials.Length; i++)
                    {
                        // 替換為指定的材質
                        newMaterials[i] = replaceMaterial;
                    }

                    // 套用新的材質陣列
                    render.materials = newMaterials;
                }
                else
                {
                    // 如果只有一個材質，直接替換
                    render.material = replaceMaterial;
                }
            }
        }

        #endregion

        #region Restore Material

        /// 復原全部對像的原始材質
        public static void RestoreAllMaterials()
        {
            foreach (var kvp in _originalMaterials)
            {
                RestoreMaterial(kvp.Key);
            }

            /*// 將鍵保存到 List 中
            List<Transform> keys = _originalMaterials.Keys.ToList();
            //用倒序遍尋，變動Dictionary內容排列就不會報錯
            for (int i =  keys.Count-1; i >=0; i++)
            {
                RestoreMaterial(keys[i]);
            }*/
        }

        /// 復原對像(陣列)的原始材質，並從Dictionary裡移除
        public static void RestoreMaterial(List<Transform> targets) =>
            targets.ForEach(target => RestoreMaterial(target));

        /// 復原對像的原始材質，並從Dictionary裡移除
        public static void RestoreMaterial(Transform target)
        {
            if (_originalMaterials.TryGetValue(target, out Material[] materials))
            {
                if (target.TryGetComponent(out Renderer render))
                {
                    render.materials = materials;
                }
            }

            // _originalMaterials.Remove(target);
        }

        #endregion

        #region Components

        /// 存儲每個物件及其原始材質的字典 {物件Transform, 材質陣列}
        private static Dictionary<Transform, Material[]> _originalMaterials = new Dictionary<Transform, Material[]>();

        #endregion
    }
}