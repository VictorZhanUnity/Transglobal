using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Table_TitleBar : MonoBehaviour
{
    [SerializeField] private Toggle toggleDESC, toggleVisible;
    public ToggleGroup toggleGroup { set => toggleVisible.group = value; }

    public UnityEvent<string, bool> onSortKeyEvent { get; set; } = new UnityEvent<string, bool>();

    private void Start()
    {
        toggleDESC.onValueChanged.AddListener((isDesc) =>
        {
            if (toggleDESC.gameObject.activeSelf == false) return;
            string keyName = name.Split("-")[1].Trim();
            onSortKeyEvent.Invoke(keyName, isDesc);
        });
    }
}
