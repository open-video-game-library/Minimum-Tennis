using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ParameterSettingManager : MonoBehaviour, ITaskManager
{
    public static int rallyCount;
    public static int maxRallyCount;

    private Shot ballServePlayer;

    private readonly float ballServeSpan = 3.0f;
    private float ballServeCount;
    private bool isBallServed;

    private bool miss;
    private bool prepareOnce;

    [SerializeField]
    private Image resetPanel;
    private float panelAlpha;

    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text highScoreText;

    [SerializeField]
    private CameraResetManager cameraResetManager;

    [SerializeField]
    private GameObject[] parameterSliders;
    private IResetParameter[] changeParameters;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = SystemParameters.fps;

        InitializeTask();
        rallyCount = 0;
        maxRallyCount = 0;

        // �{�[�����T�[�u����v���C����character2�ɐݒ�
        ballServePlayer = TaskData.character2.GetComponent<Shot>();

        scoreText.text = rallyCount.ToString();
        highScoreText.text = "High Score: " + maxRallyCount.ToString();

        resetPanel.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        panelAlpha = resetPanel.color.a;

        changeParameters = new IResetParameter[parameterSliders.Length];
        for (int i = 0; i < parameterSliders.Length; i++) { changeParameters[i] = parameterSliders[i].GetComponent<IResetParameter>(); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TaskData.controllable = TaskData.taskState == TaskState.Playing;

        switch (TaskData.taskState)
        {
            case TaskState.Start:
                ManageStartState();
                break;
            case TaskState.Prepare:
                ManagePrepareState();
                break;
            case TaskState.Playing:
                ManagePlayingState();
                break;
            case TaskState.End:
                ManageEndState();
                break;
        }
    }

    public void InitializeTask()
    {
        // Set this object's interface as task manager
        TaskData.taskManagerInterface = gameObject.GetComponent<ITaskManager>();

        // Set default parameter to "TaskData"
        TaskData.character1 = CharacterGenerator.GetCharacter(0);
        TaskData.character2 = CharacterGenerator.GetCharacter(1);

        // Set character to initial position
        TaskData.character1.transform.position = TaskData.character1DefalutPosition;
        TaskData.character2.transform.position = TaskData.character2DefalutPosition;

        Parameters.taskType = TaskType.setting;
        TaskData.taskState = TaskState.Start;

        TaskData.foul = FoulState.NoFoul;
        TaskData.lastShooter = null;
        TaskData.isOut = false;
        TaskData.isNet = false;
        TaskData.rallyCount = 0;
        TaskData.ballBoundCount = 0;
        TaskData.ballAmount = 0;

        TaskData.controllable = false;
        TaskData.pause = false;
    }

    public void SwitchTaskState(TaskState nextState)
    {
        TaskData.taskState = nextState;
    }

    private void ManageStartState()
    {
        SwitchTaskState(TaskState.Playing);
    }

    private void ManagePrepareState()
    {
        if (prepareOnce) { return; }
        prepareOnce = true;
        StartCoroutine(SwitchPlayingAfterDelay());
    }

    private void ManagePlayingState()
    {
        // ����s�\��Ԃ������̓p�����[�^�ҏW��ʂ��J����Ă���Ƃ�
        if (!TaskData.controllable || TaskData.pause)
        {
            ballServeCount = 0.0f;

            // �������������Ď�����
            if (JudgeFoul() && !miss)
            {
                miss = true;
                StartCoroutine(SwitchPrepareAfterDelay());
            }

            return;
        }

        // �X�R�A�����X�V
        rallyCount = TaskData.rallyCount;
        scoreText.text = rallyCount.ToString();
        if (rallyCount > maxRallyCount)
        {
            // �ō��X�R�A���X�V�����Ƃ�
            maxRallyCount = rallyCount;
            highScoreText.text = "High Score: " + maxRallyCount.ToString();
        }

        // �������������Ď�����
        if (JudgeFoul() && !miss)
        {
            miss = true;
            StartCoroutine(SwitchPrepareAfterDelay());
        }

        // �ΐ푊�肪�����Ń{�[�����T�[�u����܂ŃJ�E���g
        if (!isBallServed)
        {
            ballServeCount += Time.deltaTime;
            if (ballServeCount >= ballServeSpan)
            {
                ballServePlayer.ServeBall();
                ballServeCount = 0.0f;
                isBallServed = true;

                if (TaskData.foul == FoulState.NoFoul) { TaskData.lastShooter = ballServePlayer.gameObject.name; }
            }
        }
    }

    private void ManageEndState()
    {

    }

    IEnumerator SwitchPrepareAfterDelay()
    {
        // Foul�����o���Ă���Prepare��ԂɈڍs����܂ł̏���

        yield return new WaitForSeconds(1f); // 1�b�ҋ@
        SwitchTaskState(TaskState.Prepare);
    }

    IEnumerator SwitchPlayingAfterDelay()
    {
        // Prepare��ԂɂȂ��Ă���Playing��ԂɈڍs����܂ł̏���

        while (panelAlpha < 1.0f)
        {
            panelAlpha += 0.050f;
            resetPanel.color = new Color(0.0f, 0.0f, 0.0f, panelAlpha);
            yield return new WaitForSeconds(0.010f); // ���x����
        }

        cameraResetManager.Teleport(new Vector3(0.0f, 30.0f, -80.0f));

        TaskData.character1.transform.position = TaskData.character1DefalutPosition;
        TaskData.character2.transform.position = TaskData.character2DefalutPosition;

        TaskData.foul = FoulState.NoFoul;
        TaskData.lastShooter = null;
        TaskData.isOut = false;
        TaskData.isNet = false;
        TaskData.ballBoundCount = 0;
        TaskData.ballAmount = 0;

        TaskData.rallyCount = 0;
        rallyCount = TaskData.rallyCount;
        scoreText.text = rallyCount.ToString();

        while (0.0f < panelAlpha)
        {
            panelAlpha -= 0.050f;
            resetPanel.color = new Color(0.0f, 0.0f, 0.0f, panelAlpha);
            yield return new WaitForSeconds(0.010f); // ���x����
        }

        resetPanel.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        panelAlpha = resetPanel.color.a;

        SwitchTaskState(TaskState.Playing);
        isBallServed = false;

        miss = false;
        prepareOnce = false;
    }

    public void ResetParameters()
    {
        // Interface�o�R�Ńp�����[�^��������
        for (int i = 0; i < parameterSliders.Length; i++) { changeParameters[i].ResetParameter(); }
    }

    public void BackHome()
    {
        InitializeTask();
        SceneManager.LoadScene("HomeScene");
    }

    private bool JudgeFoul()
    {
        if (TaskData.isNet) { TaskData.foul = FoulState.Net; }
        else if (TaskData.isOut) { TaskData.foul = FoulState.Out; }
        else if (TaskData.ballBoundCount >= 2) { TaskData.foul = FoulState.TwoBounds; }

        return TaskData.foul != FoulState.NoFoul;
    }

    public Vector3 DecideArrivalPosition(TaskBallController ballController)
    {
        Vector3 defaltTargetPoint = new Vector3(0.0f, 0.0f, -20.0f);

        return defaltTargetPoint;
    }

    public float DecideLateralSpeed(GameObject ball, TaskBallController ballController, float ballSpeedY, Vector3 targetPosition)
    {
        Vector3 ballPosition = ball.transform.position;
        float gravity = ballController.Gravity;
        float arrivalTime = (ballSpeedY + Mathf.Sqrt(ballSpeedY * ballSpeedY + 2.0f * gravity * ballPosition.y)) / gravity;

        float disX = targetPosition.x - ball.transform.position.x;
        return disX / arrivalTime;
    }

    public float DecideDepthSpeed(GameObject ball, TaskBallController ballController, float ballSpeedY, Vector3 targetPosition)
    {
        Vector3 ballPosition = ball.transform.position;
        float gravity = ballController.Gravity;
        float arrivalTime = (ballSpeedY + Mathf.Sqrt(ballSpeedY * ballSpeedY + 2.0f * gravity * ballPosition.y)) / gravity;

        float disZ = ball.transform.position.z - targetPosition.z;
        return disZ / arrivalTime;
    }
}
