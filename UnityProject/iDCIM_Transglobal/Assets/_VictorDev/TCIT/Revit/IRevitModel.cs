/// <summary>
/// [接口] 供elementId與system兩個參數
/// </summary>
public interface IRevitModel
{
    /// <summary>
    /// 模型ID值
    /// </summary>
    string elementId { get; }

    /// <summary>
    /// 裝置類型
    /// </summary>
    string system { get; }
}
