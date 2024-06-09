using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    // �e�v���C���̖��O
    public static string character1Name = "Character 1";
    public static string character2Name = "Character 2";

    // ���Q�[����悩���i�[
    public static GameSize gameSize = GameSize.nineGames;

    // ���ۂɉ��Q�[���s���������i�[
    public static int gameAmount = 6;

    // �e�v���C���̊e�Q�[���ɂ�����l�����_
    public static string[] character1ScoreResult = { "15", "A", "40", "A", "40" ,"40"};
    public static string[] character2ScoreResult = { "40", "D", "0", "0" , "0" , "0" };

    // �e�v���C���̊l���Q�[����
    public static int character1GameCount = 5;
    public static int character2GameCount = 1;

    // �J�ڂ��Ă����V�[���̖��O
    public static string previousSceneName;

    // �e�v���C���̊l���Q�[���̐��ڂ�`�悷��N���X
    [SerializeField]
    private GameResultDrawer gameResultDrawer;

    // �e�v���C���̍ŏI�I�Ȋl���Q�[�����Ə������b�Z�[�W��`�悷��N���X
    [SerializeField]
    private FinalResultDrawer finalResultDrawer;

    private Animator animator;

    // CSV�Ƀf�[�^���Z�b�g���������Ǘ�����ϐ�
    private bool isSetCSVData = false;

    IEnumerator Start()
    {
        Application.targetFrameRate = SystemParameters.fps;

        animator = GetComponent<Animator>();

        // ���҂̖��O���i�[
        string winner = DecideWinner(character1GameCount, character2GameCount);

        if (!isSetCSVData)
        {
            // �Q�[���f�[�^��CSV�t�@�C���ɃZ�b�g
            RecordData(winner);
            CSVDataManager.SetData(GameData.csv);
            isSetCSVData = true;
        }

        // �e�v���C���̊l���Q�[���̐��ڂ�`�悷��
        yield return StartCoroutine(gameResultDrawer.DrawGameResult(character1Name, character2Name, gameAmount, character1ScoreResult, character2ScoreResult));

        // ���o�̂��߁A0.5�b�ҋ@����
        yield return new WaitForSecondsRealtime(0.50f);

        // �e�v���C���̍ŏI�I�Ȋl���Q�[�����Ə������b�Z�[�W��`�悷��
        finalResultDrawer.DrawFinalResult(character1GameCount, character2GameCount, winner);

        animator.SetTrigger("DrawResult");
    }

    public void LoadHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(previousSceneName);
    }

    public void GetCSV()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer) { CSVDataManager.GetData(); }
        else { CSVDataManager.ExportCSV(); }
    }

    private string DecideWinner(int character1GameCount, int character2GameCount)
    {
        string winner = "";

        if (character1GameCount == (int)gameSize) { winner = character1Name; }
        else if (character2GameCount == (int)gameSize) { winner = character2Name; }

        return winner;
    }

    private void RecordData(string winnerName)
    {
        if (GameData.csv == null) { return; }

        GameData.csv.winner = winnerName;
        GameData.csv.character1GameCount = GameData.character1GameCount;
        GameData.csv.character2GameCount = GameData.character2GameCount;
    }
}
