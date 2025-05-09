using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using VictorDev.Common;
using VictorDev.MaterialUtils;

namespace VictorDev.ObjectUtils
{
    [Serializable]
    public class ModelFinder:MonoBehaviour
    {
        [Button]
        public void FindTargetObjects() => findModels = ModelMaterialHandler.FindTargetObjects(objKeyWords);

        [Button]
        public void AddColliderToObjects() => ObjectHelper.AddColliderToObjects(findModels, new BoxCollider());

        [Button]
        public void RemoveColliderFromObjects() => ObjectHelper.RemoveColliderFromObjects(findModels);

        public bool IsOn
        {
            set
            {
                if(value) ToShow();
                else ToHide();
            }
        }
        
        public void ToShow() => ModelMaterialHandler.ReplaceMaterialWithExclude(findModels.ToHashSet());
        public void ToHide() => ModelMaterialHandler.RestoreOriginalMaterials();
        
        #region Variables
        [Header("[Name關鍵字]")]
        [SerializeField] List<string> objKeyWords;
        
        [Header(">>> 搜尋到的模型")]
        [SerializeField] List<Transform> findModels;
        
        /// 搜尋到的模型
        public List<Transform> FindModels => findModels;
        #endregion
    }
}