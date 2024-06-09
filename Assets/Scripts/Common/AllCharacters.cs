using UnityEngine;

[System.Serializable]
public class CharacterObjects
{
    public GameObject[] characters;

    public CharacterObjects(GameObject[] _characters)
    {
        characters = _characters;
    }
}

public class AllCharacters : MonoBehaviour
{
    // �ȉ���CharacterObjects�N���X�̕ϐ��ɁAUnity�G�f�B�^����l��ݒ肷��B
    public CharacterObjects characterObjects;

    void Awake()
    {
        // CharacterNumber�����Ȃ��悤�ɁA�����Ŏ����I�ɔԍ���U���Ă���鏈��
        characterObjects.characters = SetData(characterObjects.characters);
    }

    private GameObject[] SetData(GameObject[] characterObjects)
    {
        GameObject[] returnObjects = characterObjects;

        for (int i = 0; i < returnObjects.Length; i++)
        {
            CharacterData characterData = returnObjects[i].GetComponent<CharacterData>();
            Character character = characterData.character;

            character.characterNumber = i;
        }

        return returnObjects;
    }
}
