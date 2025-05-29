using UnityEngine;

public class MouseDragRotate : MonoBehaviour
{
    public float rotationSpeed = 20f; // 旋轉速度
    private float lastMouseX; // 上一次的滑鼠X座標

    void Update()
    {
        // 檢查滑鼠左鍵是否按下
        if (Input.GetMouseButton(0)) // 0是左鍵
        {
            // 記錄滑鼠的當前X座標
            float currentMouseX = Input.mousePosition.x;

            // 計算滑鼠的移動量
            float deltaX = currentMouseX - lastMouseX;

            Debug.Log($"deltaX: {deltaX}");
            
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Vector3 centerPos = meshRenderer.bounds.center;
            
            // 根據滑鼠的移動來改變物體的旋轉
            // 使用 RotateAround 函數，確保以物體的中心點為旋轉軸
            transform.RotateAround(centerPos, Vector3.up, deltaX * rotationSpeed * Time.deltaTime);

            // 更新最後一次的滑鼠X座標
            lastMouseX = currentMouseX;
            
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }
}