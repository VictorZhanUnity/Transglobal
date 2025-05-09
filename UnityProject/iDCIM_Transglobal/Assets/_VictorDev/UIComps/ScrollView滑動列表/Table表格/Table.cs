using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    [SerializeField] private List<Table_TitleBar> titleBarList = new List<Table_TitleBar>();
    [SerializeField] private ToggleGroup toggleGroup;

    public UnityEvent<string, bool> onSortKeyEvent = new UnityEvent<string, bool>();

    private void Start()
    {
        titleBarList.ForEach(titleBar =>
        {
            if (titleBar != null)
            {
                titleBar.toggleGroup = toggleGroup;
                titleBar.onSortKeyEvent.AddListener(onSortKeyEvent.Invoke);
            }
        });
    }
}
