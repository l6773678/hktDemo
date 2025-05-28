using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 仅用来表现和将输入传输给逻辑层的类
/// </summary>
public class XoGameView : MonoBehaviour
{
    //表现序列化区域
    public Button[] cells;
    public TMP_Text[] cellTexts;
    public TMP_Text outPutText;
    public TMP_Text otherPlayerText;
    public Button restartBtn;
    public Button regretBtn;
    public Button hintBtn;

    //临时存储游戏逻辑的引用 方便使用
    private XoGameBoard xoGameBoard;

    /// <summary>
    /// 初始化各种事件
    /// </summary>
    private void Start()
    {
        restartBtn.onClick.AddListener(ResetBoard);
        
        regretBtn.onClick.AddListener((() =>
        {
            xoGameBoard.UnDoStep();
        }));
        
        hintBtn.onClick.AddListener((() =>
        {
            if(xoGameBoard.XoGameState != GameState.NoOver) return;
            var hintAi = XoGameHelper.instance.GetHint();
            if (hintAi != null)
            {
                int nextMove = hintAi.GetNextStep(xoGameBoard);
                outPutText.text = "建议落子 位置 " + nextMove;
            }
        }));
            
        for (int i = 0; i < XoGameBoard.BOARD_SIZE; i++)
        {
            int index = i;
            cells[i].onClick.RemoveAllListeners();
            cells[i].onClick.AddListener(() => OnTileClick(index));
        }

        ResetBoard();
    }

    private void OnDisable()
    {
        xoGameBoard.simpleEventCenter.UnRegister(XOGameEventEnum.PlayerDoStep,OnPlayerDoStep);
        xoGameBoard.simpleEventCenter.UnRegister(XOGameEventEnum.GameOver,OnGameOver);
        xoGameBoard.simpleEventCenter.UnRegister(XOGameEventEnum.PlayerUnDoStep,OnPlayerUnDoStep);
        xoGameBoard.simpleEventCenter.UnRegister(XOGameEventEnum.SwitchHand,OnSwitchHand);
    }
    
    private void OnEnable()
    {
        xoGameBoard = XoGameHelper.instance.xoGameBoard;

        xoGameBoard.simpleEventCenter.Register(XOGameEventEnum.PlayerDoStep,OnPlayerDoStep);
        xoGameBoard.simpleEventCenter.Register(XOGameEventEnum.GameOver,OnGameOver);
        xoGameBoard.simpleEventCenter.Register(XOGameEventEnum.PlayerUnDoStep,OnPlayerUnDoStep);
        xoGameBoard.simpleEventCenter.Register(XOGameEventEnum.SwitchHand,OnSwitchHand);
    }

    private void OnSwitchHand(SimpleEventParam obj)
    {
        outPutText.text = xoGameBoard.PlayerCur.GetPlayerName()+ " 的回合";
    }

    private void OnPlayerUnDoStep(SimpleEventParam obj)
    {
        if (obj is SimpleEventParam_PlayerUnDoStep simpleEventParam_PlayerUnDoStep)
        {
            UndoCellView(simpleEventParam_PlayerUnDoStep.UnDoStep1.PosIndex);
            UndoCellView(simpleEventParam_PlayerUnDoStep.UnDoStep2.PosIndex);
        }
    }

    private void OnGameOver(SimpleEventParam obj)
    {
        if (obj is SimpleEventParam_GameOver simpleEventParam_PlayerDoStep)
        {
            OnGameOver(simpleEventParam_PlayerDoStep.State);
        }
    }


    private void OnPlayerDoStep(SimpleEventParam obj)
    {
        if (obj is SimpleEventParam_PlayerDoStep simpleEventParam_PlayerDoStep)
        {
            cellTexts[simpleEventParam_PlayerDoStep.Step.PosIndex].text = simpleEventParam_PlayerDoStep.Step.Owner.PlayerNumber.ToString();
            cells[simpleEventParam_PlayerDoStep.Step.PosIndex].interactable = false;
        }
    }
    
    private void OnTileClick(int index)
    {
        xoGameBoard.InputAndDoStep(index);
    }
    
    private void OnGameOver(GameState state)
    {
        switch (state)
        {
            case GameState.PlayerWin:
                outPutText.text = xoGameBoard.PlayerCur.GetPlayerName()+"赢了!";
                break;
            case GameState.OverNoWinner:
                outPutText.text = "平局！";
                break;
        }
        FreezeAllCells();
    }
    
    //重置表现和棋盘
    private void ResetBoard()
    {
        for (int i = 0; i < XoGameBoard.BOARD_SIZE; i++)
        {
            cells[i].interactable = true;
            cellTexts[i].text = "";
        }
        xoGameBoard.Reset(new XoGamePlayer(xoGameBoard,1),XoGameHelper.instance.GetOtherPlayer());
        otherPlayerText.text = "对手为" + XoGameHelper.instance.OtherPlayer;
    }

    //撤销格子的表现
    private void UndoCellView(int index)
    {
        cellTexts[index].text = "";
        cells[index].interactable = true;
    }

    //锁定所有格子
    void FreezeAllCells()
    {
        foreach (Button cell in cells)
        {
            cell.interactable = false;
        }

    }

}
