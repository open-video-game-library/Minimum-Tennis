using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class RallyingTaskManager : MonoBehaviour, ITaskManager
{
    public static readonly int timeLimit = 90;
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

    private float remainingTime;

    private Animator animator;

    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text highScoreText;
    [SerializeField]
    private GameObject timeTextObject;
    private TMP_Text timeText;

    [SerializeField]
    private CameraResetManager cameraResetManager;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = SystemParameters.fps;

        InitializeTask();
        rallyCount = 0;
        maxRallyCount = 0;

        // �{�[�����T�[�u����v���C����character2�ɐݒ�
        ballServePlayer = TaskData.character2.GetComponent<Shot>();

        animator = GetComponentInParent<Animator>();

        timeText = timeTextObject.GetComponent<TMP_Text>();

        scoreText.text = rallyCount.ToString();
        highScoreText.text = "High Score: " + maxRallyCount.ToString();

        timeTextObject.SetActive(true);
        remainingTime = timeLimit;
        timeText.text = FormatTime(remainingTime);

        resetPanel.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        panelAlpha = resetPanel.color.a;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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

        Parameters.taskType = TaskType.rallying;
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

    }

    private void ManagePrepareState()
    {
        if (prepareOnce) { return; }
        prepareOnce = true;
        StartCoroutine(SwitchPlayingAfterDelay());
    }

    private void ManagePlayingState()
    {
        // ����s�\��Ԃ������̓|�[�Y��ʂ��J����Ă���Ƃ�
        if (!TaskData.controllable || TaskData.pause)
        {
            ballServeCount = 0.0f;
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

        // �������Ԃ��߂����Ƃ�
        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;
            timeText.text = FormatTime(remainingTime);

            // TaskResultManager�N���X�փX�R�A��n��
            TaskResultManager.taskScore = maxRallyCount;

            SwitchTaskState(TaskState.End);
        }
        else if (isBallServed && !miss) 
        {
            // �c�莞�Ԃ����炷
            remainingTime -= Time.deltaTime;
            timeText.text = FormatTime(remainingTime);
        }

        // �������������Ď�����
        if (JudgeFoul())
        {
            if (remainingTime > 0.0f)
            {
                if (!miss)
                {
                    miss = true;
                    StartCoroutine(SwitchPrepareAfterDelay());
                }
            }
            else { SwitchTaskState(TaskState.End); }
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
        animator.SetTrigger("Finish");
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

    private string FormatTime(float totalSeconds)
    {
        // float����int�֕ϊ�
        int intSeconds = (int)totalSeconds;
        // �����擾
        int minutes = intSeconds / 60;
        // �b���擾
        int remainingSeconds = intSeconds % 60;
        // �~���b���擾
        int milliseconds = (int)((totalSeconds - intSeconds) * 100);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, remainingSeconds, milliseconds);
    }

    private bool JudgeFoul()
    {
        if (TaskData.isNet) { TaskData.foul = FoulState.Net; }
        else if (TaskData.isOut) { TaskData.foul = FoulState.Out; }
        else if (TaskData.ballBoundCount >= 2) { TaskData.foul = FoulState.TwoBounds; }

        return TaskData.foul != FoulState.NoFoul;
    }

    public Vector3 DecideArrivalPosition(TaskBallController ball)
    {
        Vector3 defaltTargetPoint = new Vector3(0.0f, 0.0f, -20.0f);

        return defaltTargetPoint;
    }

    public float DecideLateralSpeed(GameObject ballOjbect, TaskBallController ball, float ballSpeedY, Vector3 targetPosition)
    {
        Vector3 ballPosition = ballOjbect.transform.position;
        float gravity = ball.Gravity;
        float arrivalTime = (ballSpeedY + Mathf.Sqrt(ballSpeedY * ballSpeedY + 2.0f * gravity * ballPosition.y)) / gravity;

        float disX = targetPosition.x - ballOjbect.transform.position.x;
        return disX / arrivalTime;
    }

    public float DecideDepthSpeed(GameObject ballObject, TaskBallController ball, float ballSpeedY, Vector3 targetPosition)
    {
        Vector3 ballPosition = ballObject.transform.position;
        float gravity = ball.Gravity;
        float arrivalTime = (ballSpeedY + Mathf.Sqrt(ballSpeedY * ballSpeedY + 2.0f * gravity * ballPosition.y)) / gravity;

        float disZ = ballObject.transform.position.z - targetPosition.z;
        return disZ / arrivalTime;
    }
}
