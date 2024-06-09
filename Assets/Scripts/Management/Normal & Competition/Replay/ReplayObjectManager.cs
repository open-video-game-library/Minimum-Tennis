using UnityEngine;

public enum ReplayState
{
    Idle,
    Recording,
    Recorded,
    Replay,
    End
}

public class ReplayObjectManager : MonoBehaviour
{
    // �ȉ���replayObjectControllers��replayBallController��Interface�K�p���ł���

    [System.NonSerialized]
    public ReplayObjectController[] replayObjectControllers = new ReplayObjectController[2];

    [System.NonSerialized]
    public ReplayBallController replayBallController;

    private readonly float replaySpeed = 0.50f;
    private readonly float replayTime = 5.0f;

    private float worldTime;

    private float localTime;
    private float startTime;
    private float endTime;

    [System.NonSerialized]
    public ReplayState state;

    // Start is called before the first frame update
    void Start()
    {
        worldTime = 0.0f;
        state = ReplayState.Idle;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        worldTime += Time.deltaTime;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case ReplayState.Idle:
                break;
            case ReplayState.Recording:
                Record();
                break;
            case ReplayState.Recorded:
                break;
            case ReplayState.Replay:
                Replay();
                break;
        }
    }

    public void StartRecord()
    {
        startTime = worldTime;
        Time.timeScale = 1.0f;

        for (int i = 0; i < replayObjectControllers.Length; i++)
        {
            if (replayObjectControllers[i].animationRecorder)
            {
                // �A�j���[�V�����̋L�^���J�n����
                replayObjectControllers[i].animationRecorder.StartRecord();
            }
        }

        // ��Ԃ�J��
        state = ReplayState.Recording;
        Debug.Log("�L�^�X�^�[�g�F�L�^�J�n������" + worldTime.ToString("N2") + "�b���_����");
    }

    private void Record()
    {
        // ���v���C�f�[�^�̋L�^���ɌĂ΂��֐�
        for (int i = 0; i < replayObjectControllers.Length; i++)
        {
            // �v���C���̈ʒu�Ɖ�]���L�^����
            if (replayObjectControllers[i]) { replayObjectControllers[i].RecordTransform(worldTime); }
        }

        // �{�[���̈ʒu�Ɖ�]���L�^����
        replayBallController.RecordTransform(worldTime);
    }

    public void StopRecord()
    {
        endTime = worldTime;

        for (int i = 0; i < replayObjectControllers.Length; i++)
        {
            if (!replayObjectControllers[i].enabled) { replayObjectControllers[i].gameObject.SetActive(true); }
            if (replayObjectControllers[i].animationRecorder) { replayObjectControllers[i].transform.parent = null; }
            if (replayObjectControllers[i].animationRecorder) { replayObjectControllers[i].animationRecorder.StopRecord(); }
        }

        // ��Ԃ�J��
        state = ReplayState.Recorded;
        Debug.Log("�L�^�X�g�b�v�F�L�^���Ԃ�" + (endTime - startTime).ToString("N2") + "�b");
    }

    public void StartReplay()
    {
        // ���v���C���̍Đ����x��ς���
        Time.timeScale = replaySpeed;

        if (endTime - replayBallController.BackToExistTime(endTime, startTime) >= replayTime) { localTime = endTime - replayTime; }
        else { localTime = replayBallController.BackToExistTime(endTime, startTime); }

        for (int i = 0; i < replayObjectControllers.Length; i++)
        {
            // �A�j���[�V�������L�^���Ă���ꍇ�́A�A�j���[�V�����̃��v���C�̍Đ����J�n����
            if (replayObjectControllers[i].animationRecorder) { replayObjectControllers[i].animationRecorder.StartPlayback(endTime - localTime); }
        }

        // ��Ԃ�J��
        state = ReplayState.Replay;
        Debug.Log("���v���C�X�^�[�g�F�J�n���Ԃ�" + localTime.ToString("N2") + "�b���_����");
    }

    // ���v���C�f�[�^�̍Đ����ɌĂ΂��֐�
    private void Replay()
    {
        for (int i = 0; i < replayObjectControllers.Length; i++)
        {
            // �v���C���I�u�W�F�N�g�̃��v���C�Đ�
            replayObjectControllers[i].Replay(localTime);
        }

        // �{�[���I�u�W�F�N�g�̃��v���C�Đ�
        replayBallController.Replay(localTime);

        localTime += Time.deltaTime;

        if (endTime - startTime >= replayTime) { if (localTime > Mathf.Min(endTime - startTime, replayTime) + endTime - replayTime) { CancelReplay(); } }
        else { if (localTime > Mathf.Min(endTime - startTime, replayTime) + startTime) { CancelReplay(); } }
    }

    public void CancelReplay()
    {
        for (int i = 0; i < replayObjectControllers.Length; i++)
        {
            if (replayObjectControllers[i].animationRecorder)
            {
                // �A�j���[�V�������L�^���Ă���ꍇ�́A�A�j���[�V�����̃��v���C���~����
                replayObjectControllers[i].animationRecorder.StopPlayback();
            }
        }

        // ���v���C���̍Đ����x���猳�ɖ߂�
        Time.timeScale = 1.0f;

        // ��Ԃ�J��
        state = ReplayState.End;
        Debug.Log("���v���C�I��");
    }
}
