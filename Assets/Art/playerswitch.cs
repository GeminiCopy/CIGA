using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("子对象引用")]
    public GameObject player1;
    public GameObject player2;

    private bool isPlayer1Active = true;

    void Start()
    {
        // 初始化状态：Player1 显示，Player2 隐藏
        SetActivePlayer(true);
    }

    void Update()
    {
        // 检测 Shift 键（左或右）是否被按下
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isPlayer1Active = !isPlayer1Active;
            SetActivePlayer(isPlayer1Active);
        }
    }

    void SetActivePlayer(bool showPlayer1)
    {
        if (player1 != null) player1.SetActive(showPlayer1);

        if (player2 != null) player2.SetActive(!showPlayer1);
    }
}
