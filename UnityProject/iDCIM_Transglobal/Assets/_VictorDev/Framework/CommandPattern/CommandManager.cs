using System;
using System.Collections.Generic;
using UnityEngine.Events;
using VictorDev.Common;
using Debug = VictorDev.Common.Debug;

/// [Command設計模式]
/// <para>+ 返回上一步/復原下一步 的機制</para>
/// <para>+ 直接呼叫Execute函式即可，傳入execute與undo的Action</para>
/// <para>+ 或是呼叫Execute函式，傳入自訂的ICommand內容</para>
public class CommandManager : SingletonMonoBehaviour<CommandManager>
{
    /// 執行並儲存命令
    public static void Execute(Action executeAction, Action undoAction)
    {
        ICommand command = new GenericCommand(executeAction, undoAction);
        Execute(command);
    }

    /// 執行特定Command並儲存命令
    public static void Execute(ICommand command)
    {
        command.Execute();
        Instance._commandStack.Push(command);
        Instance._undoStack.Clear();
        Instance.isUndoEnabled.Invoke(Instance._commandStack.Count > 0);
        InvokeEvent();
    }

    /// 取消並返回上一步命令
    public static void Undo()
    {
        if (Instance._commandStack.Count == 0)
        {
            Debug.Log(">>> 已無UnDo步驟記錄!");
            return;
        }

        ICommand command = Instance._commandStack.Pop();
        command.Undo();
        Instance._undoStack.Push(command);
        InvokeEvent();
        Debug.Log(">>> Undo");
    }

    /// 復原並執行命令
    public static void Redo()
    {
        if (Instance._undoStack.Count == 0)
        {
            Debug.Log(">>> 已無ReDo步驟記錄!");
            return;
        }

        ICommand command = Instance._undoStack.Pop();
        command.Execute();
        Instance._commandStack.Push(command);
        InvokeEvent();
        Debug.Log(">>> Redo");
    }

    // 清空命令暫存
    public static void Clear()
    {
        Instance._commandStack.Clear();
        Instance._undoStack.Clear();
    }

    #region Events
    public UnityEvent<bool> isUndoEnabled = new UnityEvent<bool>();
    public UnityEvent<bool> isRedoEnabled = new UnityEvent<bool>();

    private static void InvokeEvent()
    {
        Instance.isUndoEnabled.Invoke(Instance._commandStack.Count > 0);
        Instance.isRedoEnabled.Invoke(Instance._undoStack.Count > 0);
    }
    private void Start() => InvokeEvent();
    #endregion
    
    #region Components

    /// 已執行的命令，Stack：先進後出
    public Stack<ICommand> _commandStack = new Stack<ICommand>();

    /// 被復原的命令
    private Stack<ICommand> _undoStack = new Stack<ICommand>();

    #endregion

    /// 預設的Command類型
    /// <para>+ 僅承載execute與undo的Action</para>
    public class GenericCommand : ICommand
    {
        public void Execute() => _executeAction?.Invoke();
        public void Undo() => _undoAction?.Invoke();

        #region Initialize

        private Action _executeAction, _undoAction;

        public GenericCommand(Action executeAction, Action undoAction)
        {
            _executeAction = executeAction;
            _undoAction = undoAction;
        }

        #endregion
    }
}