using UnityEngine;
using UnityEngine.InputSystem;

namespace VictorDev.CameraUtils
{
    /// RTS攝影機控制器
    /// https://chatgpt.com/share/67fcb5ab-b03c-8012-b685-28ab8ee23da5
    public class RTSCameraController : MonoBehaviour
    {
        [Header(">>> LookAt對像")] public Transform target;

        [Header(">>> 移動邊界BoxCollider Trigger")]
        public BoxCollider boundsCollider;

        [Header("Target Movement Bounds")] public float distance = 10f;
        public Vector2 distanceLimits = new Vector2(1f, 50f);
        public float zoomSpeed = 0.5f;
        public float zoomSpeedMultiplier = 7f;
        public float zoomDampTime = 0.2f;

        [Header("Zoom Control")] // 🔄
        public bool enableZoom = true; // 🔄 Zoom 開關

        [Header("Rotation")] public float xSpeed = 7f;
        public float ySpeed = 7f;
        public float yMinLimit = -10f;
        public float yMaxLimit = 90f;
        public float rotationDampTime = 0.2f;

        [Header("Rotation Control")] // 🔄
        public bool enableRotation = true; // 🔄 Rotation 開關

        [Header("Movement")] public float moveSpeed = 3f;
        public float moveDampTime = 0.2f;
        public float edgeSizePercent = 0.02f;
        public bool enableEdgeMovement = false;

        [Header("Movement Speed Adjustment Based on Distance")] // 🔄
        public float
            moveSpeedMultiplier = 0.15f; // 🔄 Adjust move speed based on distance (lower values for faster movement)

        private Vector3 currentTargetPosition;
        private Vector3 targetMoveVelocity;
        private float x, y;
        private float currentDistance;
        private float distanceVelocity;
        private Vector3 rotationVelocity;
        private Bounds moveBounds;
        private bool isRotating = false;
        private bool isShiftPressed = false;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            if (target != null)
            {
                currentTargetPosition = target.position;
            }

            currentDistance = distance;

            if (boundsCollider != null)
            {
                moveBounds = boundsCollider.bounds;
            }
        }

        void Update()
        {
            if (target == null) return;

            HandleMovementInput();
            if (!isRotating) HandleEdgeMovement();
            if (enableZoom) HandleZoom(); // 🔄 Zoom 開關
            if (enableRotation) HandleRotation(); // 🔄 Rotation 開關

            ApplyPosition();
        }

        void HandleMovementInput()
        {
            Vector3 input = Vector3.zero;
            var kb = Keyboard.current;

            // 檢查Shift鍵是否被按下
            isShiftPressed = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;

            if (kb.wKey.isPressed) input += new Vector3(0, 0, 1);
            if (kb.sKey.isPressed) input += new Vector3(0, 0, -1);
            if (kb.aKey.isPressed) input += new Vector3(-1, 0, 0);
            if (kb.dKey.isPressed) input += new Vector3(1, 0, 0);
            if (kb.eKey.isPressed) input += new Vector3(0, 1, 0);
            if (kb.qKey.isPressed) input += new Vector3(0, -1, 0);

            if (input != Vector3.zero)
            {
                // 🔄 Adjust move speed based on distance
                float adjustedMoveSpeed = moveSpeed * (1f + (currentDistance * moveSpeedMultiplier));

                // 如果按住Shift鍵，加速移動
                if (isShiftPressed)
                {
                    adjustedMoveSpeed *= 3f; // Shift按下時加倍速度
                }

                Vector3 move = Quaternion.Euler(0, x, 0) * input.normalized;
                currentTargetPosition += move * adjustedMoveSpeed * Time.deltaTime;
            }
        }

        void HandleEdgeMovement()
        {
            if (!enableEdgeMovement) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float borderX = screenWidth * edgeSizePercent;
            float borderY = screenHeight * edgeSizePercent;

            Vector3 dir = Vector3.zero;

            if (mousePos.x < borderX) dir.x = -1;
            else if (mousePos.x > screenWidth - borderX) dir.x = 1;

            if (mousePos.y < borderY) dir.z = -1;
            else if (mousePos.y > screenHeight - borderY) dir.z = 1;

            if (dir != Vector3.zero)
            {
                // 🔄 Adjust move speed based on distance
                float adjustedMoveSpeed = moveSpeed * (1f + (currentDistance * moveSpeedMultiplier));

                // 如果按住Shift鍵，加速移動
                if (isShiftPressed)
                {
                    adjustedMoveSpeed *= 2f; // Shift按下時加倍速度
                }

                Vector3 move = Quaternion.Euler(0, x, 0) * dir.normalized;
                currentTargetPosition += move * adjustedMoveSpeed * Time.deltaTime;
            }
        }

        void HandleZoom()
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                float adjustedZoomSpeed = zoomSpeed + (currentDistance * zoomSpeedMultiplier);
                distance -= scroll * adjustedZoomSpeed * Time.deltaTime;
                distance = Mathf.Clamp(distance, distanceLimits.x, distanceLimits.y);
            }

            currentDistance = Mathf.SmoothDamp(currentDistance, distance, ref distanceVelocity, zoomDampTime);

            // 🔄 Check if the distance is 1 and adjust Y Min Limit accordingly
            if (Mathf.Approximately(currentDistance, 1f))
            {
                yMinLimit = -90f;
            }
            else
            {
                yMinLimit = -20f; // Default value
            }
        }

        void HandleRotation()
        {
            if (Mouse.current.rightButton.isPressed)
            {
                isRotating = true;
                Vector2 delta = Mouse.current.delta.ReadValue();
                x += delta.x * xSpeed * 0.02f;
                y -= delta.y * ySpeed * 0.02f;
                y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
            }
            else
            {
                isRotating = false;
            }

            Quaternion targetRotation = Quaternion.Euler(y, x, 0);
            Quaternion smoothRotation =
                Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime / rotationDampTime);
            transform.rotation = smoothRotation;
        }

        void ApplyPosition()
        {
            if (boundsCollider != null)
            {
                moveBounds = boundsCollider.bounds;
                currentTargetPosition.x = Mathf.Clamp(currentTargetPosition.x, moveBounds.min.x, moveBounds.max.x);
                currentTargetPosition.y = Mathf.Clamp(currentTargetPosition.y, moveBounds.min.y, moveBounds.max.y);
                currentTargetPosition.z = Mathf.Clamp(currentTargetPosition.z, moveBounds.min.z, moveBounds.max.z);
            }

            Vector3 dampedTarget = Vector3.SmoothDamp(target.position, currentTargetPosition, ref targetMoveVelocity,
                moveDampTime);
            target.position = dampedTarget;

            Vector3 offset = transform.rotation * new Vector3(0, 0, -currentDistance);
            transform.position = target.position + offset;
        }

        public void SetTarget(Vector3 position, float setDistance = -1f)
        {
            currentTargetPosition = position;
            if (setDistance > 0f)
            {
                distance = Mathf.Clamp(setDistance, distanceLimits.x, distanceLimits.y);
            }
        }

        public void FlyToPosition(Vector3 position, float setDistance = -1f)
        {
            SetTarget(position, setDistance);
        }
    }
}