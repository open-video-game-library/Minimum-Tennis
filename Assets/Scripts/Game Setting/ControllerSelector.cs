using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Controller
{
    public GameObject[] controllerPrefabs;
    public GameObject[] controllers;
    public bool[] connections;
    public Toggle[] toggles;

    public Controller(GameObject[] prefabs)
    {
        controllerPrefabs = prefabs;
        controllers = new GameObject[prefabs.Length];
        connections = new bool[prefabs.Length];
        toggles = new Toggle[prefabs.Length];
    }
}

public class ControllerSelector : MonoBehaviour
{
    [SerializeField]
    private Players players;

    [SerializeField]
    private GameObject[] normalControllersPrefabs;
    private Controller normalController;

    [SerializeField]
    private GameObject[] competitionControllersPrefabs;
    private Controller competitionController;

    [SerializeField]
    private GameObject[] settingControllersPrefabs;
    private Controller settingController;

    [SerializeField]
    private GameObject[] taskControllersPrefabs;
    private Controller taskController;

    private ToggleGroup toggleGroup;

    private Controller currentController;

    // Start is called before the first frame update
    void Awake()
    {
        toggleGroup = GetComponent<ToggleGroup>();

        normalController = new Controller(normalControllersPrefabs);
        competitionController = new Controller(competitionControllersPrefabs);
        settingController = new Controller(settingControllersPrefabs);
        taskController = new Controller(taskControllersPrefabs);

        switch (Parameters.playMode)
        {
            case PlayMode.normal:
                currentController = normalController;
                break;
            case PlayMode.competition:
                currentController = competitionController;
                break;
            case PlayMode.setting:
                currentController = settingController;
                break;
            case PlayMode.task:
                currentController = taskController;
                break;
        }

        GenerateUI(currentController);

        for (int i = 0; i < currentController.controllerPrefabs.Length; i++)
        {
            currentController.toggles[i] = currentController.controllers[i].GetComponent<Toggle>();
            currentController.toggles[i].isOn = i == 0;
            currentController.toggles[i].group = toggleGroup;
        }

        StartCoroutine(CheckStatus(currentController));
    }

    private void GenerateUI(Controller controller)
    {
        for (int i = 0; i < controller.controllerPrefabs.Length; i++)
        {
            controller.controllers[i] = Instantiate(controller.controllerPrefabs[i]);
            controller.controllers[i].transform.SetParent(transform);
            controller.controllers[i].transform.localPosition = new Vector3(-350.0f + 220.0f * (i % 3), -170.0f - 170.0f * (i / 3), 0.0f);
            controller.controllers[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    private IEnumerator CheckStatus(Controller controller)
    {
        while (true)
        {
            for (int i = 0; i < controller.controllers.Length; i++)
            {
                // Normal���[�h���̗��p�\�R���g���[���̐ڑ���Ԃ��`�F�b�N����
                controller.connections[i] = controller.controllers[i].GetComponent<IConnectionManager>().ManageConnection();
            }

            // 1�b���Ƀ`�F�b�N���s��
            yield return new WaitForSeconds(1f);
        }
    }

    public bool ManageSelection()
    {
        bool returnValue = false;

        // �I������Ă����Ԃɕs��������ꍇ�ɒ������鏈��
        CorrectSelection(currentController.connections, currentController.toggles);

        // �e�R���g���[���̑I����Ԃ��S�ēK�؂����Ǘ�����ϐ�
        bool[] isNormalSelectCorrectly = new bool[currentController.controllerPrefabs.Length];

        for (int i = 0; i < currentController.connections.Length; i++)
        {
            isNormalSelectCorrectly[i] = currentController.connections[i] && currentController.toggles[i].isOn;
            if (isNormalSelectCorrectly[i])
            {
                // Parameters�����ϐ��ɁA�I�𒆂̑����@��o�^����
                Parameters.inputMethod[(int)players] = currentController.controllers[i].GetComponent<IConnectionManager>().ReturnInputMethod();
                returnValue = true;
                break;
            }
        }

        return returnValue;
    }

    private void CorrectSelection(bool[] connections, Toggle[] toggles)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i] && toggles[i].isOn)
            {
                for (int j = 0; j < connections.Length; j++)
                {
                    if (connections[j])
                    {
                        toggles[j].isOn = true;
                        break;
                    }
                }
            }
        }
    }
}