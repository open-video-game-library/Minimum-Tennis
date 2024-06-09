using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinalResultDrawer : MonoBehaviour
{
    // �e�v���C���̊l���Q�[������`�悷��e�L�X�g
    [SerializeField]
    private TMP_Text pointText;

    // ���������v���C���̖��O��`�悷��e�L�X�g
    [SerializeField]
    private TMP_Text winnerNameText;

    public void DrawFinalResult(int character1GameCount, int character2GameCount, string winnerName)
    {
        pointText.text = character1GameCount + "-" + character2GameCount;
        winnerNameText.text = winnerName;
    }
}
