using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VictorDev.Net.WebAPI;
using Debug = VictorDev.Common.Debug;

public class WebAPI_Test : MonoBehaviour
{
    [SerializeField] private List<WebAPI_Request> requestList;

    private void Start()
    {
        requestList.ForEach(request =>
        {
            WebAPI_Caller.CallWebAPI(request, OnSuccess, OnFailed);
        });
    }

    private void OnSuccess(long responseCode, Dictionary<string, string> jsonData)
    {
        Debug.Log($"OnSuccess [{responseCode}]");

        jsonData.ToList().ForEach(response =>
        {
            Debug.Log($"\t>>> Item [{response.Key}] - {response.Value}");
        });
    }

    private void OnFailed(long responseCode, string msg)
    {
        Debug.LogWarning($"[{responseCode}] >>> OnFailed: {msg}");
    }
}
