using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    public static Character[] selectedCharacterData = new Character[2];

    public static Vector3[] playerDefaultPosition = { new Vector3(8.0f, 0.0f, -49.0f), new Vector3(-8.0f, 0.0f, 49.0f) };
    public static Vector3[] playerDefaultRotation = { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 180.0f, 0.0f) };

    [SerializeField]
    private GameObject[] defaultCharacters = new GameObject[2];

    [SerializeField]
    private GameObject allCharactersObjectRight;
    private AllCharacters allCharactersRight;

    [SerializeField]
    private GameObject allCharactersObjectLeft;
    private AllCharacters allCharactersLeft;

    // �E�����̃A�o�^�Q
    private GameObject[] playersRight;

    // �������̃A�o�^�Q
    private GameObject[] playersLeft;

    private GameObject[] characters = new GameObject[2];
    private static GameObject[] returnCharacters = new GameObject[2];

    // Start is called before the first frame update
    void Awake()
    {
        allCharactersRight = allCharactersObjectRight.GetComponent<AllCharacters>();
        allCharactersLeft = allCharactersObjectLeft.GetComponent<AllCharacters>();

        // �E�����̃A�o�^�Q�̓ǂݍ���
        playersRight = allCharactersRight.characterObjects.characters;

        // �������̃A�o�^�Q�̓ǂݍ���
        playersLeft = allCharactersLeft.characterObjects.characters;

        for (int i = 0; i < characters.Length; i++)
        {
            if (selectedCharacterData[i] != null) { characters[i] = SearchCharacter(selectedCharacterData[i], i, Parameters.charactersDominantHand[i]); }
            else
            {
                Debug.Log("Character data is not set!");
                characters[i] = defaultCharacters[i];
            }
            returnCharacters[i] = Instantiate(characters[i]);

            // returnCharacters[i]�ɁA�L�����N�^�[����ɕK�v�ȃR���|�[�l���g���A�^�b�`����
            AddPlayerComponent(i);

            // �L�����N�^�[�̏����ʒu�Ə�����]��ݒ肷��
            returnCharacters[i].transform.position = playerDefaultPosition[i];
            returnCharacters[i].transform.eulerAngles = playerDefaultRotation[i];

            returnCharacters[i].name = characters[i].name;
        }

        // ���O�̏d�����Ȃ��悤�ɁA�������O�̏ꍇ�̓i���o�����O����
        if (returnCharacters[0].name == returnCharacters[1].name)
        {
            returnCharacters[0].name = returnCharacters[0].name + " 1";
            returnCharacters[1].name = returnCharacters[1].name + " 2";
        }

        returnCharacters[0].GetComponent<CharacterData>().players = Players.p1;
        returnCharacters[1].GetComponent<CharacterData>().players = Players.p2;
    }

    public static GameObject GetCharacter(int characterNum)
    {
        return returnCharacters[characterNum];
    }

    private GameObject SearchCharacter(Character characterData, int characterNum, DominantHand dominantHand)
    {
        GameObject targetCharacter = defaultCharacters[characterNum];

        int targetNumber = characterData.characterNumber;

        GameObject[] players;

        if (dominantHand == DominantHand.left) { players = playersLeft; }
        else { players = playersRight; }

        for (int i = 0; i < players.Length; i++)
        {
            Character comparisonData = players[i].GetComponent<CharacterData>().character;
            if (targetNumber == comparisonData.characterNumber) { targetCharacter = players[i]; }
        }

        return targetCharacter;
    }

    private void AddPlayerComponent(int playerNum)
    {
        // �v���C���̈ړ��𐧌䂷��R���|�[�l���g
        returnCharacters[playerNum].GetComponent<Move>().player = (Players)playerNum;
        // �v���C���̃X�C���O�𐧌䂷��R���|�[�l���g
        returnCharacters[playerNum].GetComponent<Shot>().player = (Players)playerNum;

        if (Parameters.playMode == PlayMode.normal || Parameters.playMode == PlayMode.competition)
        {
            // �����`���̃��[�h�ŁA�v���C���̓����𐧌䂷��R���|�[�l���g
            returnCharacters[playerNum].AddComponent<PlayerNormalController>().player = (Players)playerNum;

            // �v���C���������ő��삷��R���|�[�l���g
            returnCharacters[playerNum].AddComponent<PlayerNormalAI>().player = (Players)playerNum;
        }
        else if (Parameters.playMode == PlayMode.setting || Parameters.playMode == PlayMode.task)
        {
            // �^�X�N�`���̃��[�h�ŁA�v���C���̓����𐧌䂷��R���|�[�l���g
            returnCharacters[playerNum].AddComponent<PlayerTaskController>().player = (Players)playerNum;

            // �v���C���������ő��삷��R���|�[�l���g
            returnCharacters[playerNum].AddComponent<PlayerTaskAI>().player = (Players)playerNum;
        }

        switch (Parameters.playMode)
        {
            case PlayMode.normal:

                PlayerNormalAI normalAI = returnCharacters[playerNum].GetComponent<PlayerNormalAI>();

                switch (Parameters.inputMethod[playerNum])
                {
                    case InputMethod.keyboard:
                        returnCharacters[playerNum].AddComponent<KeyboardInputManager>();
                        normalAI.autoMove = false;
                        normalAI.autoShot = false;
                        break;
                    case InputMethod.gamepad:
                        returnCharacters[playerNum].AddComponent<GamepadInputManager>();
                        normalAI.autoMove = false;
                        normalAI.autoShot = false;
                        break;
                    case InputMethod.motion:
                        returnCharacters[playerNum].AddComponent<JoyconInputManager>();
                        returnCharacters[playerNum].AddComponent<JoyconManager>();
                        normalAI.autoMove = true;
                        normalAI.autoShot = false;
                        break;
                    case InputMethod.none:
                        normalAI.autoMove = true;
                        normalAI.autoShot = true;
                        break;
                }
                break;
            case PlayMode.competition:

                PlayerNormalAI competitionAI = returnCharacters[playerNum].GetComponent<PlayerNormalAI>();

                switch (Parameters.inputMethod[playerNum])
                {
                    case InputMethod.keyboard:
                        // Setting��ʂőI��s��
                        break;
                    case InputMethod.gamepad:
                        returnCharacters[playerNum].AddComponent<GamepadInputManager>();
                        competitionAI.autoMove = false;
                        competitionAI.autoShot = false;
                        break;
                    case InputMethod.motion:
                        // Setting��ʂőI��s��
                        break;
                    case InputMethod.none:
                        // Setting��ʂőI��s��
                        break;
                }
                break;
            case PlayMode.setting:

                PlayerTaskAI settingAI = returnCharacters[playerNum].GetComponent<PlayerTaskAI>();

                switch (Parameters.inputMethod[playerNum])
                {
                    case InputMethod.keyboard:
                        returnCharacters[playerNum].AddComponent<KeyboardInputManager>();
                        settingAI.autoMove = false;
                        settingAI.autoShot = false;
                        break;
                    case InputMethod.gamepad:
                        returnCharacters[playerNum].AddComponent<GamepadInputManager>();
                        settingAI.autoMove = false;
                        settingAI.autoShot = false;
                        break;
                    case InputMethod.motion:
                        returnCharacters[playerNum].AddComponent<JoyconInputManager>();
                        returnCharacters[playerNum].AddComponent<JoyconManager>();
                        settingAI.autoMove = true;
                        settingAI.autoShot = false;
                        break;
                    case InputMethod.none:
                        // 1P��Setting��ʂőI��s��
                        settingAI.autoMove = true;
                        settingAI.autoShot = true;
                        break;
                }
                break;
            case PlayMode.task:

                PlayerTaskAI taskAI = returnCharacters[playerNum].GetComponent<PlayerTaskAI>();

                switch (Parameters.inputMethod[playerNum])
                {
                    case InputMethod.keyboard:
                        returnCharacters[playerNum].AddComponent<KeyboardInputManager>();
                        taskAI.autoMove = false;
                        taskAI.autoShot = false;
                        break;
                    case InputMethod.gamepad:
                        returnCharacters[playerNum].AddComponent<GamepadInputManager>();
                        taskAI.autoMove = false;
                        taskAI.autoShot = false;
                        break;
                    case InputMethod.motion:
                        returnCharacters[playerNum].AddComponent<JoyconInputManager>();
                        returnCharacters[playerNum].AddComponent<JoyconManager>();
                        taskAI.autoMove = true;
                        taskAI.autoShot = false;
                        break;
                    case InputMethod.none:
                        // 1P��Setting��ʂőI��s��
                        taskAI.autoMove = true;
                        taskAI.autoShot = true;
                        break;
                }
                break;
        }
    }
}
