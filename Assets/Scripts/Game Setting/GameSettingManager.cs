using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSettingManager : MonoBehaviour
{
    public static InputMethod inputMethod;

    void Start()
    {
        Application.targetFrameRate = SystemParameters.fps;
    }

    public void LoadCharacterSelectScene()
    {
        // �L�����N�^�I���V�[���ɖ߂�
        StartCoroutine(LoadAfterWaiting("CharacterSelectScene"));
    }

    public void StartGame()
    {
        switch (Parameters.playMode)
        {
            case PlayMode.normal:
                StartCoroutine(LoadAfterWaiting("TennisScene"));
                break;
            case PlayMode.competition:
                StartCoroutine(LoadAfterWaiting("TennisScene"));
                break;
            case PlayMode.setting:
                StartCoroutine(LoadAfterWaiting("ParameterSettingScene"));
                break;
            case PlayMode.task:
                StartCoroutine(LoadAfterWaiting("TaskScene"));
                break;
        }
    }

    private IEnumerator LoadAfterWaiting(string sceneName)
    {
        // ���͂𖳌���
        DisableInput();

        // 1�b�ҋ@
        yield return new WaitForSeconds(1);

        // ���͂�L����
        EnableInput();
        SceneManager.LoadScene(sceneName);
    }

    private void DisableInput()
    {
        foreach (var input in FindObjectsOfType<InputField>())
        {
            input.interactable = false;
        }
        foreach (var button in FindObjectsOfType<Button>())
        {
            button.interactable = false;
        }
        foreach (var toggle in FindObjectsOfType<Toggle>())
        {
            if (toggle.gameObject.TryGetComponent(out IConnectionManager connectionManager))
            {
                // Interface���o�R���āA�R���g���[���̐ڑ���Ԃ��m�F���鏈�����~������
                connectionManager.Active = false;
            }

            toggle.interactable = false;
        }
    }

    private void EnableInput()
    {
        foreach (var input in FindObjectsOfType<InputField>())
        {
            input.interactable = true;
        }
        foreach (var button in FindObjectsOfType<Button>())
        {
            button.interactable = true;
        }
        foreach (var toggle in FindObjectsOfType<Toggle>())
        {
            if (toggle.gameObject.TryGetComponent(out IConnectionManager connectionManager))
            {
                // Interface���o�R���āA�R���g���[���̐ڑ���Ԃ��m�F���鏈�����ĊJ������
                connectionManager.Active = true;
            }

            toggle.interactable = true;
        }
    }
}
