using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.Android.Types;
using UnityEngine;
using VictorDev.Common;
using VictorDev.TCIT.WebAPI;

namespace VictorDev.TCIT
{
    public class RealtimeDataManager : MonoBehaviour
    {
        [Button]
        private void GetRealtimeData()
        {
            resultRealtimeData?.Clear();
            BuildSendDataTags();
            WebApiGetRealtimeData.GetRealtimeData(_sendDataTags, OnSuccessHandler);
        }

        private void OnSuccessHandler(List<Data_Blackbox> data)
        {
            resultRealtimeData = data;
            receivers.OfType<IRealtimeDataReceiver>().ToList().ForEach(receiver=> receiver.ReceiveRealtimeData(resultRealtimeData));
        }

        private void BuildSendDataTags()
        {
            _sendDataTags = new WebApiGetRealtimeData.SendDataTags();
            _sendDataTags.tags = sendTagNames.Split(",").Select(str => str.Trim().Trim('"'))
                .Where(s => !string.IsNullOrEmpty(s)).ToList();
        }

        private void OnValidate() => receivers = ObjectHelper.CheckTypoOfList<IRealtimeDataReceiver>(receivers);

        #region Variables
        [Header("[資料Receiver]")] [SerializeField]
        private List<MonoBehaviour> receivers;

        [Header("[API回傳資料結果]")] [SerializeField]
        private List<Data_Blackbox> resultRealtimeData;

        [TextArea(1, 10)] [SerializeField] private string sendTagNames;
        private WebApiGetRealtimeData.SendDataTags _sendDataTags;
        #endregion
        
        public interface IRealtimeDataReceiver
        {
            void ReceiveRealtimeData(List<Data_Blackbox> data);
        }
    }
}