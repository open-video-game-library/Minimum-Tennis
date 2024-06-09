using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{
    // ���v���C�̃V�X�e���𐧌䂷��I�u�W�F�N�g
    [SerializeField]
    private GameObject replayManagerObject;

    // ���v���C�̃V�X�e���𐧌䂷��N���X
    private ReplayObjectManager replayObjectManager;

    // �A�j���[�V�������L�^���邽�߂�Prefab
    [SerializeField]
    private GameObject animationRecorderPrefab;

    // �v���C���Ƒΐ푊����i�[
    private readonly GameObject[] players = new GameObject[2];
    private readonly GameObject[] playerRecorders = new GameObject[2];

    // �{�[�����i�[
    private GameObject ball;

    // ���v���C�Đ��p�̃R�s�[���Ɏg���{�[����Prefab
    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private CameraTypeManager cameraTypeManager;

    [SerializeField]
    private ReplayCamera replayCamera;

    [SerializeField]
    private Image fadePanel;

    private Animator animator;

    private bool canCancelReplay;

    // Start is called before the first frame update
    void Start()
    {
        replayObjectManager = replayManagerObject.GetComponent<ReplayObjectManager>();
        animator = GetComponent<Animator>();

        // �v���C���̃I�u�W�F�N�g�ɁAReplayObjectController���A�^�b�`����
        SetPlayersReplay();

        // �v���C���̃I�u�W�F�N�g���ƂɁAAnimationRecorder�𐶐�
        GenerateRecorders();

        // ���v���C�Đ��p�̃{�[���𐶐�
        SetBallReplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameData.gameState == GameState.Playing && replayObjectManager.state != ReplayState.Recording) { replayObjectManager.StartRecord(); }
        else if (GameData.gameState == GameState.Prepare && replayObjectManager.state == ReplayState.Recording) { replayObjectManager.StopRecord(); }
    }

    public void Manage()
    {
        if (replayObjectManager.state == ReplayState.Recorded)
        {
            // ���v���C���Đ����Ă��Ȃ��ꍇ�A�����ōĐ����J�n����
            StartCoroutine(StartReplay());
        }
        else if (replayObjectManager.state == ReplayState.Replay)
        {
            // ���v���C�̃L�����Z��
            if (canCancelReplay 
                && (GameData.replayCancel || (Parameters.inputMethod[0] == InputMethod.none && Parameters.inputMethod[1] == InputMethod.none
                && Input.anyKeyDown))) { replayObjectManager.CancelReplay(); }
        }
        else if (replayObjectManager.state == ReplayState.End)
        {
            // ���v���C�̍Đ����I��������A���v���C���[�h���I������
            StartCoroutine(EscapeReplay());
            return;
        }
    }

    private void SetPlayersReplay()
    {
        // �v���C�����̓ǂݍ���
        players[0] = GameData.character1;
        players[1] = GameData.character2;

        for (int i = 0; i < players.Length; i++)
        {
            // ReplayObjectController���A�^�b�`
            players[i].AddComponent<ReplayObjectController>();

            // ReplayObjectManager�ɁA�A�^�b�`����ReplayObjectController��o�^
            replayObjectManager.replayObjectControllers[i] = players[i].GetComponent<ReplayObjectController>();
        }
    }

    private void SetBallReplay()
    {
        ball = Instantiate(ballPrefab);
        
        replayObjectManager.replayBallController = replayManagerObject.GetComponent<ReplayBallController>();
        replayObjectManager.replayBallController.replayBall = ball;

        // ���v���C���̃J�����̒ǐՑΏۂ�ݒ�
        replayCamera.SetTrackingObject(replayObjectManager.replayBallController.replayBall);

        replayObjectManager.replayBallController.replayBall.SetActive(false);
    }

    private void GenerateRecorders()
    {
        for (int i = 0; i < playerRecorders.Length; i++)
        {
            // Prefab����I�u�W�F�N�g�𐶐����A�q�I�u�W�F�N�g�ɐݒ�
            playerRecorders[i] = Instantiate(animationRecorderPrefab);
            playerRecorders[i].transform.parent = replayManagerObject.transform;

            // ��������AnimationRecorder�ɁA�v���C��������Animator��o�^
            AnimationRecorder animationRecorder = playerRecorders[i].GetComponent<AnimationRecorder>();
            animationRecorder.animator = players[i].GetComponent<Animator>();

            // �v���C��������ReplayObjectController�ɁA��������AnimationRecorder��o�^
            replayObjectManager.replayObjectControllers[i].animationRecorder = playerRecorders[i].GetComponent<AnimationRecorder>();
        }
    }

    private IEnumerator StartReplay()
    {
        canCancelReplay = false;

        // ���v���C�̏�Ԃ��ꎞ�I��Idle�ɐݒ�
        replayObjectManager.state = ReplayState.Idle;

        // �Ó]����
        fadePanel.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        // ���o�̂��߁A0.25�b�����Ó]�����܂ܑҋ@
        yield return new WaitForSeconds(0.25f);

        // �J������ʏ�p���烊�v���C�p�ɐ؂�ւ���
        animator.SetTrigger("ChangeMainToReplay");
        if (Parameters.playMode == PlayMode.competition) { cameraTypeManager.SwitchCameraType(PlayMode.normal); }

        // ���v���C�̍Đ����J�n
        replayObjectManager.StartReplay();

        // 1�t���[���ҋ@
        yield return null;

        // ���]�̉��o
        for (int i = 0; i < Application.targetFrameRate / 2; i++)
        {
            fadePanel.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - i * (float)(2.0f / Application.targetFrameRate));

            // 1�t���[���ҋ@
            yield return null;
        }

        // ���]��ԂŌŒ�
        fadePanel.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        canCancelReplay = true;
    }

    private IEnumerator EscapeReplay()
    {
        // ���v���C�̏�Ԃ��ꎞ�I��Idle�ɐݒ�
        replayObjectManager.state = ReplayState.Idle;

        // ���v���C�Đ��p�̃{�[���I�u�W�F�N�g���A�N�e�B�u��Ԃɂ���
        replayObjectManager.replayBallController.replayBall.SetActive(false);

        // �Ó]����
        fadePanel.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        // �J���������v���C�p����ʏ�p�ɐ؂�ւ���
        animator.SetTrigger("ChangeReplayToMain");
        if (Parameters.playMode == PlayMode.competition) { cameraTypeManager.SwitchCameraType(Parameters.playMode); }

        // 1�t���[���ҋ@
        yield return null;

        // �v���C���̍Ĕz�u���̈ړ��ɃJ�������҂�����t���Ă����悤�ɂ���
        cameraTypeManager.TeleportCamera(Parameters.playMode);

        if (GameData.character1GameCount != (int)Parameters.gameSize && GameData.character2GameCount != (int)Parameters.gameSize)
        {
            GameData.character1.GetComponent<ICharacterMover>().ResetCharacterPosition(new Vector3(8.0f * (int)GameData.servePosition, 0.0f, -49.0f));
            GameData.character2.GetComponent<ICharacterMover>().ResetCharacterPosition(new Vector3(-8.0f * (int)GameData.servePosition, 0.0f, 49.0f));
        }

        // ���o�̂��߁A0.25�b�����Ó]�����܂ܑҋ@
        yield return new WaitForSeconds(0.25f);

        // �Q�[���̐i�s�ɉ����āA���ɑJ�ڂ���Q�[���̏�Ԃ�؂�ւ���
        if (GameData.character1GameCount == (int)Parameters.gameSize || GameData.character2GameCount == (int)Parameters.gameSize) { GameData.gameState = GameState.End; }
        else { GameData.gameState = GameState.Playing; }

        // ���]�̉��o
        for (int i = 0; i < Application.targetFrameRate / 2; i++)
        {
            fadePanel.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - i * (float)(2.0f / Application.targetFrameRate));

            // 1�t���[���ҋ@
            yield return null;
        }

        // ���]��ԂŌŒ�
        fadePanel.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        GameData.replayCancel = false;
    }
}
