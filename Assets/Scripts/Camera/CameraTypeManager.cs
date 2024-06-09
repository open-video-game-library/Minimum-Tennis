using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraTypeManager : MonoBehaviour
{
    // �ʏ�v���C���Ɏg�p����J�����֘A�̗v�f
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject mainMixingCamera;
    [SerializeField]
    private CinemachineVirtualCamera mainVirtualCamera;
    private CinemachineTransposer mainTransposer;

    // �ΐ�v���C���Ɏg�p����J�����֘A�̗v�f
    [SerializeField]
    private GameObject[] multipleCamera;
    [SerializeField]
    private GameObject[] multipleMixingCamera;
    [SerializeField]
    private CinemachineVirtualCamera[] multipleVirtualCamera;
    private CinemachineTransposer[] multipleTransposer;
    [SerializeField]
    private GameObject boundaryLine;

    // Start is called before the first frame update
    void Start()
    {
        SwitchCameraType(Parameters.playMode);

        multipleTransposer = new CinemachineTransposer[multipleVirtualCamera.Length];

        mainTransposer = mainVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        for (int i = 0; i < multipleVirtualCamera.Length; i++) { multipleTransposer[i] = multipleVirtualCamera[i].GetCinemachineComponent<CinemachineTransposer>(); }
    }

    public void SwitchCameraType(PlayMode playMode)
    {
        mainCamera.SetActive(playMode != PlayMode.competition);
        mainMixingCamera.SetActive(playMode != PlayMode.competition);

        for (int i = 0; i < multipleCamera.Length; i++) { multipleCamera[i].SetActive(playMode == PlayMode.competition); }
        for (int i = 0; i < multipleMixingCamera.Length; i++) { multipleMixingCamera[i].SetActive(playMode == PlayMode.competition); }
        boundaryLine.SetActive(playMode == PlayMode.competition);
    }

    public void TeleportCamera(PlayMode playMode)
    {
        switch (playMode)
        {
            case PlayMode.competition:
                for (int i = 0; i < multipleTransposer.Length; i++) { Teleport(multipleTransposer[i]); }
                break;
            default:
                Teleport(mainTransposer);
                break;
        }
    }

    private void Teleport(CinemachineTransposer transposer)
    {
        transposer.m_XDamping = 0.0f;
        transposer.m_YDamping = 0.0f;
        transposer.m_ZDamping = 0.0f;

        // ���̃t���[���Ō��ɖ߂�
        StartCoroutine(ResetDamping(transposer));
    }

    private IEnumerator ResetDamping(CinemachineTransposer transposer)
    {
        // 1�t���[���҂�
        yield return null;

        // ���ɖ߂�
        transposer.m_XDamping = 1.0f;
        transposer.m_YDamping = 1.0f;
        transposer.m_ZDamping = 1.0f;
    }
}
