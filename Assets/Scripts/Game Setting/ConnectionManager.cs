using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    private ControllerSelector player1AvailableController;
    [SerializeField]
    private ControllerSelector player2AvailableController;

    [SerializeField]
    private Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.interactable = false;
    }

    void Update()
    {
        // 1P��2P�̗��҂̃R���g���[�����������I�����ꂽ�ꍇ�A�X�^�[�g�{�^����������悤�ɂ���
        startButton.interactable = player1AvailableController.ManageSelection() && player2AvailableController.ManageSelection();
    }
}
