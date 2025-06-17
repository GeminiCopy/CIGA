using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("�Ӷ�������")]
    public GameObject player1;
    public GameObject player2;

    private bool isPlayer1Active = true;

    void Start()
    {
        // ��ʼ��״̬��Player1 ��ʾ��Player2 ����
        SetActivePlayer(true);
    }

    void Update()
    {
        // ��� Shift ��������ң��Ƿ񱻰���
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
