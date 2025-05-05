public static class EmojiHelper
{
    public static string GetEmoji(EmojiEnum emoji = EmojiEnum.Info)
    { 
        return emoji switch
        {
            EmojiEnum.None => "",
            
            // ✅ 成功 / 資訊
            EmojiEnum.Info => "ℹ️",
            EmojiEnum.Success => "✅",
            EmojiEnum.Search => "🔍",
            EmojiEnum.Download => "📥",
            EmojiEnum.Upload => "📤",
            EmojiEnum.FileBox => "🗃️",
            EmojiEnum.DataBox => "📦",
            EmojiEnum.BlueBook => "📘",
            EmojiEnum.GreenCircle => "🟢",
            EmojiEnum.Brain => "🧠",

            // ⚠️ 警告
            EmojiEnum.Warning => "⚠️",
            EmojiEnum.YellowCircle => "🟡",
            EmojiEnum.Bell => "🔔",
            EmojiEnum.FireExtinguisher => "🧯",
            EmojiEnum.Clock => "🕒",

            // ❌ 錯誤
            EmojiEnum.Error => "❌",
            EmojiEnum.RedCircle => "🔴",
            EmojiEnum.Boom => "💥",
            EmojiEnum.Forbidden => "🚫",
            EmojiEnum.Lock => "🔒",

            // 🛠️ 除錯
            EmojiEnum.Wrench => "🔧",
            EmojiEnum.Tools => "🛠️",
            EmojiEnum.Lab => "🧪",
            EmojiEnum.Thread => "🧵",
            EmojiEnum.Folder => "🗂️",

            // 🖥️ 系統
            EmojiEnum.Monitor => "🖥️",
            EmojiEnum.Gear => "⚙️",
            EmojiEnum.Plug => "🔌",
            EmojiEnum.Battery => "🔋",
            EmojiEnum.Satellite => "🛰️",

            // 👤 使用者
            EmojiEnum.User => "👤",
            EmojiEnum.Robot => "🤖",
            EmojiEnum.Handshake => "🤝",
            EmojiEnum.Wave => "👋",
            EmojiEnum.Eyes => "👀",

            // 🔄 狀態
            EmojiEnum.Refresh => "🔄",
            EmojiEnum.Loading => "⏳",
            EmojiEnum.Done => "✔️",
            EmojiEnum.Pause => "⏸️",

            // 🔒 安全
            EmojiEnum.Shield => "🛡️",
            EmojiEnum.Key => "🔑",
            EmojiEnum.Door => "🚪",
            EmojiEnum.Skull => "☠️",

            // 🎯 任務
            EmojiEnum.Target => "🎯",
            EmojiEnum.Rocket => "🚀",
            EmojiEnum.Hammer => "🔨",
            EmojiEnum.Bug => "🐞",
            EmojiEnum.Star => "⭐",

            _ => "❓"
        };
    }
}
public enum EmojiEnum
{
    None,
    
    // ✅ 成功 / 資訊
    Info,               // ℹ️
    Success,            // ✅
    Search,             // 🔍
    Download,           // 📥
    Upload,             // 📤
    FileBox,            // 🗃️
    DataBox,            // 📦
    BlueBook,           // 📘
    GreenCircle,        // 🟢
    Brain,              // 🧠

    // ⚠️ 警告
    Warning,            // ⚠️
    YellowCircle,       // 🟡
    Bell,               // 🔔
    FireExtinguisher,   // 🧯
    Clock,              // 🕒

    // ❌ 錯誤 / 異常
    Error,              // ❌
    RedCircle,          // 🔴
    Boom,               // 💥
    Forbidden,          // 🚫
    Lock,               // 🔒

    // 🛠️ 除錯 / 建構
    Wrench,             // 🔧
    Tools,              // 🛠️
    Lab,                // 🧪
    Thread,             // 🧵
    Folder,             // 🗂️

    // 🖥️ 系統 / 顯示
    Monitor,            // 🖥️
    Gear,               // ⚙️
    Plug,               // 🔌
    Battery,            // 🔋
    Satellite,          // 🛰️

    // 👤 使用者 / 行為
    User,               // 👤
    Robot,              // 🤖
    Handshake,          // 🤝
    Wave,               // 👋
    Eyes,               // 👀

    // 🔄 狀態 / 動作
    Refresh,            // 🔄
    Loading,            // ⏳
    Done,               // ✔️
    Pause,              // ⏸️

    // 🔒 安全 / 權限
    Shield,             // 🛡️
    Key,                // 🔑
    Door,               // 🚪
    Skull,              // ☠️

    // 🎯 任務 / 驅動
    Target,             // 🎯
    Rocket,             // 🚀
    Hammer,             // 🔨
    Bug,                // 🐞
    Star,               // ⭐
}