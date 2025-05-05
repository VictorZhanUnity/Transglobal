using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using VictorDev.Common;

public class CommandPatternTest : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 30f;
    public Ease ease = Ease.OutQuad;

    public GameObject prefab;
    
    void Update()
    {
        CheckKey(KeyCode.W, MoveForward);
        CheckKey(KeyCode.S, MoveBack);
        CheckKey(KeyCode.A, MoveLeft);
        CheckKey(KeyCode.D, MoveRight);
        // 檢查是否有點擊事件
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false) // 0 代表左鍵
        {
            CreateCubeAtMousePosition();
        }
    }

    private void CreateCubeAtMousePosition()
    {
        // 構建射線
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 發射射線並檢查是否碰撞到物體
        if (Physics.Raycast(ray, out hit))
        {
            GameObject obj = null;
            void toDo()
            {
                Vector3 newPoint = hit.point;
                newPoint.y += 1;
                // 在射線擊中的位置創建Cube
                obj = Instantiate(prefab, newPoint, Quaternion.identity);
            }

            void unDo()
            {
                Destroy(obj);
            }
            
            CommandManager.Execute(toDo, unDo);
        }
    }

    public void UnDo() => CommandManager.Undo();
    public void Redo() => CommandManager.Redo();

    public void OnMoveHandler(Vector3 direction)
    {
        direction *= Time.deltaTime * moveSpeed;
        /*Action toDo = () => player.Translate(direction);
        Action unDo = () => player.Translate(direction * -1);*/
        Action toDo = () => player.DOMove(player.position + direction*moveSpeed, 0.5f).SetEase(Ease.OutQuad);
        Action unDo = () => player.DOMove(player.position + direction*moveSpeed*-1, 0.5f).SetEase(Ease.OutQuad);
        CommandManager.Execute(toDo, unDo);
    }

    public void MoveForward() => OnMoveHandler(Vector3.forward);
    public void MoveBack() => OnMoveHandler(Vector3.back);
    public void MoveLeft() => OnMoveHandler(Vector3.left);
    public void MoveRight() => OnMoveHandler(Vector3.right);

    private bool CheckKey(KeyCode key, Action action)
    {
        if (Input.GetKey(key))
        {
            action.Invoke();
            return true;
        }

        return false;
    }
}