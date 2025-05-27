using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace VictorDev.Common
{
    /// GameObject物件處理
    public static class ObjectHelper
    {
        /// 檢查目標物件是否為Null
        public static bool CheckTargetsIsNull(params object[] targets)
        {
            bool result = false;
            foreach (var target in targets)
            { 
                if (target is Object unityObj && unityObj == null)
                {
                    result = true;
                }
                else if (target == null)
                {
                    result = true;
                }
            }
            return result;
        }
        
        /// 刪除目標物件底下所有子物件
        public static void RemoveAllChildInParent(Transform parent)
        {
            List<GameObject> objectList = parent.Cast<Transform>().Select(obj=>obj.gameObject).ToList();
            objectList.ForEach(obj=>DestoryObject(obj));
            objectList.Clear();
        }
        
        /// 刪除目標物件
        public static void DestoryObject(GameObject obj)
        {
            if(Application.isPlaying) GameObject.Destroy(obj);
            else GameObject.DestroyImmediate(obj);
        }
        
        /// ==================================================================================
        
        /// 根據關鍵字，針對目標物件底下所有子物件進行比對，找出名字包含關鍵字的子物件
        public static List<Transform> FindObjectsByKeyword(Transform target, string keyword,
            bool isCaseSensitive = false)
            => FindObjectsByKeywords(target, new List<string>() { keyword }, isCaseSensitive);


        /// 根據關鍵字，針對目標物件底下所有子物件進行比對，找出名字包含關鍵字的子物件
        public static List<Transform> FindObjectsByKeywords(Transform target, List<string> keywords,
            bool isCaseSensitive = false)
        {
            void FindObjectsRecursively(Transform parent, List<string> keywords, List<Transform> result)
            {
                string childName, key;
                foreach (Transform child in parent)
                {
                    // 檢查名稱是否包含任意關鍵字
                    foreach (string keyword in keywords)
                    {
                        childName = (isCaseSensitive) ? child.name : child.name.ToLower();
                        key = (isCaseSensitive) ? keyword : keyword.ToLower();

                        if (childName.Contains(key))
                        {
                            result.Add(child);
                            break; // 如果符合任意一個關鍵字，跳出內層迴圈
                        }
                    }

                    // 遞歸檢查子物件
                    FindObjectsRecursively(child, keywords, result);
                }
            }

            List<Transform> matchingObjects = new List<Transform>();
            FindObjectsRecursively(target, keywords, matchingObjects);
            return matchingObjects;
        }


        /// [泛型] 新增Collider型別到目標物件上
        /// <para>+ 泛型Collider只需傳入隨意一個實例化即可，例如: new BoxCollider()</para>
        public static void AddColliderToObjects<T>(List<Transform> targetObjects, T collider,
            bool removeExisting = true) where T : Collider
        {
            if (removeExisting) RemoveColliderFromObjects(targetObjects);
            targetObjects.ForEach(obj => obj.gameObject.AddComponent<T>());
        }


        /// [泛型] 移除Collider型別到目標物件上
        public static void RemoveColliderFromObjects(List<Transform> targetObjects)
            => targetObjects.ForEach(obj =>
            {
                Collider[] existingColliders = obj.GetComponents<Collider>();
                foreach (var collider in existingColliders)
                {
#if UNITY_EDITOR
                    Object.DestroyImmediate(collider);
#else
                   Object.Destroy(collider);
#endif
                }
            });


        /// 檢查B的包圍盒是否完全位於A的包圍盒內
        public static bool IsModelBCompletelyInsideModelA(Collider modelA, Collider modelB)
            => modelA.bounds.Contains(modelB.bounds.min) && modelA.bounds.Contains(modelB.bounds.max);


        /// 檢查B是否部分在A內
        public static bool IsModelBPartiallyInsideModelA(Collider modelA, Collider modelB)
            => modelA.bounds.Intersects(modelB.bounds);


        /// 將底下所有子物件，依照Y軸高底進行排序
        public static void SortingChildByPosY(Transform target)
        {
            // 將子物件依 Y 座標排序（從高到低）
            var sortedChildren = target.Cast<Transform>()
                .OrderByDescending(child => child.position.y)
                .ToList();
            // 更新 Sibling Index
            for (int i = 0; i < sortedChildren.Count; i++)
            {
                sortedChildren[i].SetSiblingIndex(i);
            }
        }

        /// 檢查List裡面是否有實作T類別，將不符合的從List裡移除
        public static List<MonoBehaviour> CheckTypoOfList<T>(List<MonoBehaviour> list) where T : class
        {
            #region 取得上一層呼叫的資訊

            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();

            #endregion

            for (int i = 0; i < list.Count; i++)
            {
                MonoBehaviour target = list[i];
                if (target != null && target is not T item)
                {
                    Debug.Log(
                        $">>> [接收器：{method.DeclaringType?.Name}] - 物件：{{{target.name}}} 並沒有實作{typeof(T).Name}, 已從列表移除.");
                    list.Remove(target);
                }
            }

            return list;
        }

        /// 檢查List裡面是否有繼承T類別，將不符合的從List裡移除
        public static List<MonoBehaviour> CheckSubTypoOfList<T>(List<MonoBehaviour> list) where T : class
        {
            #region 取得上一層呼叫的資訊

            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();

            #endregion

            for (int i = 0; i < list.Count; i++)
            {
                MonoBehaviour target = list[i];
                if (target != null && target.GetType().IsSubclassOf(typeof(T)) == false)
                {
                    Debug.Log(
                        $">>> [接收器：{method.DeclaringType?.Name}] - 物件：{{{target.name}}} 並非繼承{typeof(T).Name}, 已從列表移除.");
                    list.Remove(target);
                }
            }

            return list;
        }

        /// 取得場景上所有包含T類別的物件(包括inactive)
        public static List<T> FindAllObjectOfType<T>() where T : class
        {
            IEnumerable<GameObject> GetAllChildren(GameObject parent)
            {
                yield return parent;
                foreach (Transform child in parent.transform)
                {
                    foreach (var descendant in GetAllChildren(child.gameObject))
                    {
                        yield return descendant;
                    }
                }
            }

            List<T> result = SceneManager.GetActiveScene()
                .GetRootGameObjects() // 取得所有根物件
                .SelectMany(GetAllChildren).Where(obj => obj.GetComponent<T>() != null)
                .Select(target => target.GetComponent<T>()).ToList(); // 遍歷所有子物件
            return result;
        }

        /// 檢查與Target模型相交之模型
        public static List<GameObject> GetModelInBound(GameObject target, List<GameObject> models,
            List<string> keywordExclude = null, List<string> keywordInclude = null)
        {
            Debug.Log($":> [ObjectHelper] - 檢查與{target.name}相交之模型... ");

            List<GameObject> result = null;
            if (target.TryGetComponent(out Collider targetCollider))
            {
                result = new List<GameObject>();

                Bounds objectBounds = targetCollider.bounds;
                Collider[] colliders = models.Select(model => model.GetComponent<Collider>()).ToArray(); // 找到所有Collider

                foreach (Collider col in colliders)
                {
                    //檢查Collider是否有交互
                    if (col.gameObject != target && objectBounds.Intersects(col.bounds))
                    {
                        result.Add(col.gameObject);
                    }
                }

                // 判斷排除關鍵字
                if (keywordExclude != null)
                {
                    result = result.Where(model =>
                        !keywordExclude.Any(keyword =>
                            model.name.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();
                }

                // 判斷包含關鍵字
                if (keywordInclude != null)
                {
                    result = result.Where(model =>
                            keywordInclude.Any(keyword =>
                                model.name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }
            }

            Debug.Log($":> [ObjectHelper] - 檢查與{target.name}相交之模型... Done.");
            return result;
        }

        /// 以Target模型尺吋，建立同尺吋的Cube模型
        public static GameObject CreateBoundingCube(GameObject target, Material material = null,
            float sizeOffset = 0.01f)
        {
            if (target.TryGetComponent(out Renderer renderer) == false)
            {
                Debug.LogError("[ObjectHelper] - target模型沒有 Renderer，無法獲取邊界");
                return null;
            }

            // 獲取模型的邊界尺寸
            Vector3 boundsSize = renderer.bounds.size;

            // 創建新的 Cube
            GameObject boundObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boundObj.transform.SetParent(target.transform, true);
            boundObj.name = "BoundObject";

            // 設置 Cube 的大小與模型一致
            boundObj.transform.localScale = boundsSize + new Vector3(sizeOffset, sizeOffset, sizeOffset);

            // 設置 Cube 的位置，使其中心與模型一致
            boundObj.transform.position = renderer.bounds.center;

            // 可選：更改顏色讓 Cube 容易識別
            if (material != null) boundObj.GetComponent<MeshRenderer>().material = material;

            return boundObj;
        }

        /// 依字串取得目標類別實例的參數值
        public static string GetParameratorOfClass<T>(T target, string paramName)
        {
            // 取得欄位值
            FieldInfo fieldInfo = typeof(T).GetField(paramName);
            if (fieldInfo != null)
            {
                object fieldValue = fieldInfo.GetValue(target);
                return fieldValue.ToString();
            }
            return null;
        }
    }
}