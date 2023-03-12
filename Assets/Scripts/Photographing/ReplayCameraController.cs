using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayCameraController : MonoBehaviour
{
    GameObject targetObject = null; // �����������I�u�W�F�N�g��Inspector�������Ă���

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject == null)
        {
            targetObject = GameObject.FindGameObjectWithTag("Ball");
        }
        else if (targetObject.tag == "Ball")
        {
            // �⊮�X�s�[�h�����߂�
            float speed = 0.1f;
            // �^�[�Q�b�g�����̃x�N�g�����擾
            Vector3 relativePos = targetObject.transform.position - this.transform.position;
            // �������A��]���ɕϊ�
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // ���݂̉�]���ƁA�^�[�Q�b�g�����̉�]����⊮����
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);
        }
    }
}
