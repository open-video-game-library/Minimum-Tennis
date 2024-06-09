using UnityEngine;

public class RuleSwitcher : MonoBehaviour
{
    [SerializeField]
    private GameObject normalRuleObject;

    [SerializeField]
    private GameObject taskRuleObject;

    // Start is called before the first frame update
    void Start()
    {
        // Normal���[�h�ACompetition���[�h�̏ꍇ�ɑΏۃI�u�W�F�N�g���A�N�e�B�u��
        normalRuleObject.SetActive(Parameters.playMode == PlayMode.normal || Parameters.playMode == PlayMode.competition);

        // Task���[�h�̏ꍇ�ɑΏۃI�u�W�F�N�g���A�N�e�B�u��
        taskRuleObject.SetActive(Parameters.playMode == PlayMode.task);
    }
}
