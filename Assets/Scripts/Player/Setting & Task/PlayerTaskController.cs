using UnityEngine;

public class PlayerTaskController : MonoBehaviour
{
    // �قȂ���̓f�o�C�X���狤�ʂ̓��͐M�����󂯎�邽�߂̎󂯌�
    private IInputDevice inputDevice;

    // �������쎞�̈ړ��E�V���b�g�̃A���S���Y�����Ǘ�����AI
    private PlayerTaskAI ai;

    // ���g��1P��2P�����i�[
    [System.NonSerialized]
    public Players player;

    // �ړ����͗p�p�����[�^
    private float x;
    private float z;

    // �V���b�g�p�p�����[�^
    private bool normalShot;
    private bool lobShot;
    private bool fastShot;
    private bool dropShot;
    private bool aiShot;

    // �������쎞��p�̕ϐ�
    private Vector3 aiShotPower;

    // �|�[�Y��ʋN���p
    private bool escape;

    // �ړ�����p�p�����[�^
    private Move move;

    // �V���b�g����p�p�����[�^
    private Shot shot;
    private bool isHit;
    private string lastShooter;

    // Start is called before the first frame update
    void Start()
    {
        inputDevice = GetComponent<IInputDevice>();

        ai = GetComponent<PlayerTaskAI>();

        move = GetComponent<Move>();
        move.movableArea = TaskData.movableArea[(int)player];

        shot = GetComponent<Shot>();
        if (player == Players.p1) { shot.opponentCourtArea = TaskData.playersCourtArea[(int)Players.p2]; }
        else if (player == Players.p2) { shot.opponentCourtArea = TaskData.playersCourtArea[(int)Players.p1]; }
    }

    // Update is called once per frame
    void Update()
    {
        // Shot�N���X�����e�C�N�o�b�N�p�̕ϐ��ɁAAI�̏������ʂ���������
        shot.Takeback(ai.takebackFore, ai.takebackBack, ai.autoMoveLateralDirection);

        // �v���C�����R���g���[���\��Ԃɂ���Ƃ�
        if (TaskData.controllable)
        {
            // �|�[�Y���͂���������A�|�[�Y��ʂ��J��or����
            if (escape) { TaskData.pause = !TaskData.pause; }

            // �ړ����鏈��
            Move();
        }
        else
        {
            // �ړ����~�߂�
            move.StopPlayer();

            // �{�[����ł��Ă��Ȃ���Ԃɂ���
            isHit = false;
        }
    }

    void LateUpdate()
    {
        // �N�������_�����ꍇ�A�������́A�v���C�����R���g���[���\��ԂɂȂ��Ƃ�
        if (TaskData.foul != FoulState.NoFoul || !TaskData.controllable)
        {
            lastShooter = null;
            return;
        }

        // TaskData������lastShooter�ƁA���g������lastShooter���قȂ�Ƃ�
        if (TaskData.lastShooter != name && lastShooter == name)
        {
            // TaskData����lastShooter�Ɏ��g������lastShooter���㏑�����čX�V����
            TaskData.lastShooter = lastShooter;
            lastShooter = null;
        }
        else { lastShooter = null; }
    }

    void FixedUpdate()
    {
        // �����ňړ����������ꍇ
        if (ai.autoMove)
        {
            x = ai.x;
            z = ai.z;
        }
        // ���͂ňړ����������ꍇ
        else
        {
            x = inputDevice.GetMoveInput(player).x;
            z = inputDevice.GetMoveInput(player).y;
        }

        // �����ŃV���b�g���������ꍇ
        if (ai.autoShot)
        {
            aiShot = ai.shot;
            aiShotPower = ai.shotPower;
        }
        // ���͂ŃV���b�g���������ꍇ
        else
        {
            normalShot = inputDevice.GetNormalShotInput(player);
            lobShot = inputDevice.GetLobShotInput(player);
            fastShot = inputDevice.GetFastShotInput(player);
            dropShot = inputDevice.GetDropShotInput(player);
        }

        if (inputDevice != null) { escape = inputDevice.GetEscapeInput(player); }
        else if (player == Players.p1) { escape = Input.GetKeyDown(KeyCode.Escape); }
    }

    void OnTriggerStay(Collider other)
    {
        // �Փ˂���Object��Ball�łȂ��ꍇ�A�������́A���łɃ{�[����ł��Ă����ꍇ
        if (!TaskData.controllable && !other.gameObject.CompareTag("Ball") || isHit) { return; }

        GameObject ballObject = other.gameObject;

        // �V���b�g��ł���
        Shot(ballObject);
    }

    void OnTriggerExit(Collider other)
    {
        // Collider����o�Ă�����Object��Ball�̏ꍇ�A�{�[����ł��Ă��Ȃ���Ԃɂ���
        if (other.gameObject.CompareTag("Ball")) { isHit = false; }
    }

    private void Move()
    {
        move.MovePlayer(x, z);
    }

    private void Shot(GameObject ballObject)
    {
        if (TaskData.lastShooter != name)
        {
            float ballHight = ballObject.transform.position.y;

            // �����ŃV���b�g���s���ꍇ
            if (aiShot) { shot.AIShot(ballObject, aiShotPower, Parameters.charactersDominantHand[(int)player]); }
            // �v���C�����O�q�|�W�V�����ɂ���Ƃ�
            else if (TaskData.courtArea.zNegativeLimit / 2.0f < transform.position.z && transform.position.z < TaskData.courtArea.zPositiveLimit / 2.0f)
            {
                if (ballHight > 10.0f)
                {
                    // �X�}�b�V����ł�
                    if (normalShot || lobShot || fastShot || dropShot) { shot.Smash(ballObject); }
                }
                else if (ballHight > 2.50f)
                {
                    // �{���[��ł�
                    if (normalShot || lobShot || fastShot || dropShot) { shot.Volley(ballObject, Parameters.charactersDominantHand[(int)player]); }
                }
            }
            else
            {
                // ���͂ɉ����ĈقȂ�V���b�g��ł�
                if (lobShot) { shot.LobShot(ballObject, Parameters.charactersDominantHand[(int)player]); }
                else if (fastShot) { shot.FastShot(ballObject, Parameters.charactersDominantHand[(int)player]); }
                else if (dropShot) { shot.DropShot(ballObject, Parameters.charactersDominantHand[(int)player]); }
                else if (normalShot) { shot.NormalShot(ballObject, Parameters.charactersDominantHand[(int)player]); }
            }

            // �S�V���b�g���ʂ̏���
            if (normalShot || lobShot || fastShot || dropShot || aiShot)
            {
                isHit = true;

                TaskData.ballBoundCount = 0;
                TaskData.rallyCount++;

                if (Parameters.taskType == TaskType.moving || Parameters.taskType == TaskType.hitting)
                {
                    // TaskBallController��1P�v���C���ɂ���ă{�[�����ł��ꂽ���Ƃ���������
                    ballObject.GetComponent<TaskBallController>().isHitByPlayer = true;
                }

                if (TaskData.foul == FoulState.NoFoul) { lastShooter = name; }
            }
        }
    }
}
