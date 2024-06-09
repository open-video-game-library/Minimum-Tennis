using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAudioManager : MonoBehaviour
{
    private static AudioSource audioSource;

    [SerializeField]
    private AudioClip successSound;
    private static AudioClip success;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        success = successSound;
    }

    public static void PlaySuccessSound()
    {
        // ���_������ۂ̌��ʉ���炷�iTaskBallController�����炱�̊֐����ĂԂ��߁Astatic�ɂ���j
        audioSource.PlayOneShot(success);
    }
}
