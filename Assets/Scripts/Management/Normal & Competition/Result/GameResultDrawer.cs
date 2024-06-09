using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameResultDrawer : MonoBehaviour
{
    // �e�v���C���̖��O��`�悷��e�L�X�g
    [SerializeField]
    private TMP_Text character1NameText;
    [SerializeField]
    private TMP_Text character2NameText;

    // �e�v���C���̊l���Q�[���̐��ڂ�`�悷��Q�[���I�u�W�F�N�g
    [SerializeField]
    private GameObject gameResultScorePanel;

    // �e�v���C�����e�Q�[���Ŋl�������_����`�悷�邽�߂�Prefab
    [SerializeField]
    private GameObject gameScorePrefab;

    private GameObject[] gameScores;

    public IEnumerator DrawGameResult(string character1Name, string character2Name, 
        int gameAmount, 
        string[] character1ScoreResult, string[] character2ScoreResult)
    {
        character1NameText.text = character1Name;
        character2NameText.text = character2Name;

        gameScores = new GameObject[gameAmount];
        GameScoreDrawer[] gameScoreDrawers = new GameScoreDrawer[gameAmount];

        for (int i = 0; i < gameAmount; i++)
        {
            gameScores[i] = Instantiate(gameScorePrefab);
            gameScores[i].transform.SetParent(gameResultScorePanel.transform);
            gameScores[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            gameScoreDrawers[i] = gameScores[i].GetComponent<GameScoreDrawer>();
        }

        for (int i = 0; i < gameAmount; i++)
        {
            gameScoreDrawers[i].DrawGameScore(character1ScoreResult[i], character2ScoreResult[i], i + 1);
            yield return new WaitForSecondsRealtime(0.50f); 
        }
    }
}
