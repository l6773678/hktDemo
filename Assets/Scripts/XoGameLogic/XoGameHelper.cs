using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

/// <summary>
/// 用于配置不同玩家类型的枚举
/// </summary>
public enum PlayerType
{
    [InspectorName("两人手动对战")] Player,
    [InspectorName("随机策略的AI")] RandomAI,
    [InspectorName("简单策略的AI")] NormalAI,
    [InspectorName("具有特定行为方式的AI")] SpecialTragedyAI
}

/// <summary>
/// 标记谁先手的枚举
/// </summary>
public enum FirstPlayerType
{
    [InspectorName("你先手")] Player1,
    [InspectorName("对方先手")] Player2,
}

/// <summary>
/// 提示按钮采用的AI策略的枚举
/// </summary>
public enum HintType
{
    [InspectorName("随机策略的AI")] RandomAI,
    [InspectorName("简单策略的AI")] NormalAI,
    [InspectorName("具有特定行为方式的AI")] SpecialTragedyAI
}

//一个简单的全局类，方便管理整个游戏的进行
public class XoGameHelper : MonoBehaviour
{
    [Header("决定你的对手")]
    public PlayerType OtherPlayer;
    [Header("谁先手")]
    public FirstPlayerType firstPlayerHand;

    [Header("用于提示的AI策略")]
    public HintType hintType;
    
    [HideInInspector]
    public GameObject GameView;

    public static XoGameHelper instance;
    public readonly XoGameBoard xoGameBoard = new XoGameBoard();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var goCanvas = GameObject.Find("Canvas");
        if (goCanvas != null)
        {
            Object.Instantiate(GameView, goCanvas.transform);
        }
    }

    public XoGamePlayer GetOtherPlayer()
    {
        switch (OtherPlayer)
        {
            case PlayerType.Player:
                return new XoGamePlayer(xoGameBoard, 2);
            case PlayerType.RandomAI:
                return new XoGameAIPlayer(new BaseAIGetNextXoGamePos(2, 1), xoGameBoard, 2);
            case PlayerType.NormalAI:
                return new XoGameAIPlayer(new NormalAIGetNextXoGamePos(2, 1), xoGameBoard, 2);
            case PlayerType.SpecialTragedyAI:
                return new XoGameAIPlayer(new SpecialTragedyAIGetNextXoGamePos(2, 1), xoGameBoard, 2);
        }

        return null;
    }

    public BaseAIGetNextXoGamePos GetHint()
    {
        int selfPlayerNumber = xoGameBoard.PlayerCur.PlayerNumber;
        int otherPlayerNumber;
        if (selfPlayerNumber == 1)
        {
            otherPlayerNumber = 2;
        }
        else
        {
            otherPlayerNumber = 1;
        }

        switch (hintType)
        {
            case HintType.RandomAI:
                return new BaseAIGetNextXoGamePos(selfPlayerNumber, otherPlayerNumber);
            case HintType.NormalAI:
                return new NormalAIGetNextXoGamePos(selfPlayerNumber, otherPlayerNumber);
            case HintType.SpecialTragedyAI:
                return new SpecialTragedyAIGetNextXoGamePos(selfPlayerNumber, otherPlayerNumber);
        }

        return null;
    }
}