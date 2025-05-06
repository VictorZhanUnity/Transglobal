using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using VictorDev.Advanced;
using VictorDev.Common;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.MaterialUtils
{
    public class ModelMaterialHandler : SingletonMonoBehaviour<ModelMaterialHandler>
    {
        [Header(">>> 指定要替換的材質")] [SerializeField]
        private Material replaceMaterial;

        [Header(">>> 指定要替換的物件對像")] [SerializeField]
        private Transform targetTransform;

        [Space(20)] [Header("[僅顯示用] 物件對像下的每個Transform物件")] [SerializeField]
        private List<DictionaryVisualizer<Transform, Material[]>> materialDictionaryVisualize;

        [Header("[僅顯示用] 排除替換之外Transfomr對像")] [SerializeField]
        private List<Transform> excludeList;

        /// 存儲每個物件及其原始材質的字典 {物件Transform, 材質陣列}
        private Dictionary<Transform, Material[]> originalMaterials { get; set; } =
            new Dictionary<Transform, Material[]>();

        /// 根據關鍵字，針對目標物件底下所有子物件進行比對，找出名字包含關鍵字的子物件
        public static List<Transform> FindTargetObjects(List<string> keyWords)
        {
            Debug.Log($"FindTargetObjects... \n{string.Join(",", keyWords)}", Instance, EmojiEnum.DataBox);
            return ObjectHelper.FindObjectsByKeywords(Instance.targetTransform, keyWords)
                .Where(target => target.GetComponent<MeshRenderer>() != null).ToList();
        }

        /// 根據關鍵字，針對目標物件底下所有子物件進行比對，找出名字包含關鍵字的子物件
        public static List<Transform> FindTargetObjects(string keyWord) =>
            ObjectHelper.FindObjectsByKeyword(Instance.targetTransform, keyWord);

        ///  替換當前物件及所有子物件的材質
        ///  <para>+ 用HashSet以便材質的比對</para>
        ///  <para>+ 會先自動復原全部材質</para>
        public static void ReplaceMaterialWithExclude(HashSet<Transform> exlcudeTargets = null,
            Material material = null)
        {
            RestoreOriginalMaterials();
            Instance.ReplaceMaterialRecursively(Instance.targetTransform, material ?? Instance.replaceMaterial,
                exlcudeTargets);
        }

        public static void ReplaceMaterial(HashSet<Transform> targets = null)
        {
            RestoreOriginalMaterials();
            targets.ToList().ForEach(target =>
            {
                if (target.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    // 如果尚未保存原始材質，將它存儲到字典中
                    if (!Instance.originalMaterials.ContainsKey(target))
                    {
                        Instance.originalMaterials[target] = renderer.sharedMaterials;
                        Instance.materialDictionaryVisualize.Add(
                            new DictionaryVisualizer<Transform, Material[]>(target, renderer.sharedMaterials));
                    }

                    // 判斷是否有多個材質
                    if (renderer.sharedMaterials.Length > 1)
                    {
                        // 如果有多個材質，建立新的材質陣列
                        Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

                        for (int i = 0; i < newMaterials.Length; i++)
                        {
                            // 替換為指定的材質
                            newMaterials[i] = Instance.replaceMaterial;
                        }

                        // 設定新的材質陣列
                        renderer.materials = newMaterials;
                    }
                    else
                    {
                        // 如果只有一個材質，直接替換
                        renderer.material = Instance.replaceMaterial;
                    }

                    //關閉Collider
                    if (target.TryGetComponent<Collider>(out Collider collider))
                    {
                        collider.enabled = false;
                    }
                }
            });
        }

        /// [遞迴] 替換物件及其子物件的材質
        /// <para>+ 排除的對像(選填)</para>
        private void ReplaceMaterialRecursively(Transform objTransform, Material material,
            HashSet<Transform> exlcudeTargets = null)
        {
            if (exlcudeTargets != null)
            {
                excludeList.Clear();
                exlcudeTargets.ToList().ForEach(target => excludeList.Add(target));
            }

            // 取得該物件的 Renderer 組件
            // 如果物件有 Renderer 組件，替換材質
            if (objTransform.TryGetComponent<Renderer>(out Renderer renderer))
            {
                //不屬於排除對像，則替換材質
                if (exlcudeTargets != null && exlcudeTargets.Contains(objTransform) == false)
                {
                    // 如果尚未保存原始材質，將它存儲到字典中
                    if (!originalMaterials.ContainsKey(objTransform))
                    {
                        originalMaterials[objTransform] = renderer.sharedMaterials;
                        materialDictionaryVisualize.Add(
                            new DictionaryVisualizer<Transform, Material[]>(objTransform, renderer.sharedMaterials));
                    }

                    // 判斷是否有多個材質
                    if (renderer.sharedMaterials.Length > 1)
                    {
                        // 如果有多個材質，建立新的材質陣列
                        Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

                        for (int i = 0; i < newMaterials.Length; i++)
                        {
                            // 替換為指定的材質
                            newMaterials[i] = material;
                        }

                        // 設定新的材質陣列
                        renderer.materials = newMaterials;
                    }
                    else
                    {
                        // 如果只有一個材質，直接替換
                        renderer.material = material;
                    }

                    //關閉Collider
                    if (objTransform.TryGetComponent<Collider>(out Collider collider))
                    {
                        collider.enabled = false;
                    }
                }
            }

            // 遞迴處理所有子物件
            foreach (Transform child in objTransform)
            {
                if (exlcudeTargets != null && exlcudeTargets.Contains(objTransform) == false)
                {
                    ReplaceMaterialRecursively(child, material, exlcudeTargets);
                }
            }
        }


        [ContextMenu("- 恢復原始材質")]
        ///  恢復原始材質
        public static void RestoreOriginalMaterials()
        {
            foreach (var kvp in Instance.originalMaterials)
            {
                Transform objTransform = kvp.Key;
                Material[] originalMats = kvp.Value;

                // 取得物件的 Renderer 組件
                // 如果有 Renderer 組件，恢復原本的材質
                if (objTransform != null && objTransform.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    renderer.materials = originalMats;
                    //開啟Collider
                    if (objTransform.TryGetComponent<Collider>(out Collider collider))
                    {
                        collider.enabled = true;
                    }
                }
            }
        }


        //===================================== NEW
        /// <summary>
        /// 将材质设置为透明模式
        /// </summary>
        public static void SetTransparentMode(Material targetMaterial)
        {
            targetMaterial.SetFloat("_Mode", 3); // 设置模式为 Transparent
            targetMaterial.SetOverrideTag("RenderType", "Transparent");
            targetMaterial.EnableKeyword("_ALPHABLEND_ON");
            targetMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            targetMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            targetMaterial.SetInt("_ZWrite", 0); // 关闭深度写入
            targetMaterial.renderQueue = (int)RenderQueue.Transparent; // 设置渲染队列为透明层
        }

        /// <summary>
        /// 将材质设置为不透明模式
        /// </summary>
        public static void SetOpaqueMode(Material targetMaterial)
        {
            targetMaterial.SetFloat("_Mode", 0); // 设置模式为 Opaque
            targetMaterial.SetOverrideTag("RenderType", "Opaque");
            targetMaterial.DisableKeyword("_ALPHABLEND_ON");
            targetMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            targetMaterial.SetInt("_DstBlend", (int)BlendMode.Zero);
            targetMaterial.SetInt("_ZWrite", 1); // 开启深度写入
            targetMaterial.renderQueue = (int)RenderQueue.Geometry; // 设置渲染队列为几何层
        }
    }
}