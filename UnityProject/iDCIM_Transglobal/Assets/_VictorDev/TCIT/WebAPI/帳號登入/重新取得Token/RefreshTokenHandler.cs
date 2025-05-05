using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using VictorDev.Net.WebAPI;
using VictorDev.TCIT;
using VictorDev.TCIT.WebAPI;
using Debug = VictorDev.Common.Debug;

public class RefreshTokenHandler : MonoBehaviour
{
    [Range(1, 12)] [Header(">>> 每隔幾小時更新Token")] [SerializeField]
    private int intervalHours = 8;

    [FormerlySerializedAs("_request")] [Header(">>> [WebAPI]")] [SerializeField]
    private WebAPI_Request request;
    
    private Coroutine _coroutine;

    private IEnumerator ToCountDown()
    {
        while (true)
        {
            Debug.Log($">>> 每隔{intervalHours}小時，重新取得Token - 到數計時...");
            Debug.Log($"\t下次重新取得Token時間：{DateTime.Now.AddHours(intervalHours).ToString()}");
            yield return new WaitForSeconds( 60 * 60 * 8);
            ToRefreshToken();
        }
    }

    [ContextMenu("- 進行重新取得Token")]
    private void ToRefreshToken()
    {
        if (string.IsNullOrEmpty(WebApiLoginManager.LoginInfo.refresh_token))
        {
            Debug.Log("User尚未登入取得refresh Token.");
            return;
        }

        Debug.Log("進行重新取得Token...");
        SendRawJson data = JsonConvert.DeserializeObject<SendRawJson>(request.BodyJSON);
        data.refresh_token = WebApiLoginManager.LoginInfo.refresh_token;

        request.SetRawJsonData(JsonConvert.SerializeObject(data));
        WebAPI_Caller.SendRequest(request, (responseCode, jsonString) =>
        {
            WebApiLoginManager.Parse(jsonString);
            Debug.Log("Token更新完成.");
        });
    }

    public void StopRefreshToken()
    {
        if(_coroutine!=null) StopCoroutine(_coroutine);
        Debug.Log("停止重新取得Token.");
    }

    private class SendRawJson
    {
        public string grant_type = "password";
        public string refresh_token;
        public string scope = "auto";
    }


    #region [Initialize]

    private void OnEnable()
    {
        StopRefreshToken();
        _coroutine = StartCoroutine(ToCountDown());
    }

    private void OnDisable() => StopRefreshToken();

    #endregion
}