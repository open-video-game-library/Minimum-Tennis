using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayBallController : MonoBehaviour
{
    public AnimationCurve positionX = new();
    public AnimationCurve positionY = new();
    public AnimationCurve positionZ = new();

    public AnimationCurve quaternionX = new();
    public AnimationCurve quaternionY = new();
    public AnimationCurve quaternionZ = new();
    public AnimationCurve quaternionW = new();

    public AnimationCurve isExist = new();

    [System.NonSerialized]
    public GameObject replayBall;
    private GameObject ball;

    void Update()
    {
        if (CountObjectAmount("Ball") == 1 && !ball) { ball = GameObject.FindWithTag("Ball"); }
        else if (CountObjectAmount("Ball") == 0) { ball = null; }
    }

    public void RecordTransform(float worldTime)
    {
        // �������g�̈ʒu�Ɖ�]���L�^����
        if (ball)
        {
            Add(ball.transform.position, ball.transform.rotation, worldTime);
            isExist.AddKey(worldTime, 1.0f);
        }
        // �{�[�������݂��Ȃ��ꍇ�́C�����L�^���Ȃ�
        else
        {
            ball = null;
            Add(new Vector3(0.0f, -10.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), worldTime);
            isExist.AddKey(worldTime, 0.0f);
        }
    }

    public void Add(Vector3 pos, Quaternion v, float worldTime)
    {
        positionX.AddKey(worldTime, pos.x);
        positionY.AddKey(worldTime, pos.y);
        positionZ.AddKey(worldTime, pos.z);

        quaternionX.AddKey(worldTime, v.x);
        quaternionY.AddKey(worldTime, v.y);
        quaternionZ.AddKey(worldTime, v.z);
        quaternionW.AddKey(worldTime, v.w);
    }

    public float BackToExistTime(float endTime, float startTime)
    {
        float existTime = startTime;
        float deltaTime = 1.0f / Application.targetFrameRate;

        for (float i = endTime; i > 0.0f; i -= deltaTime)
        {
            if (isExist.Evaluate(i) < 1.0f)
            {
                // �{�[�����Ō�̏o���������Ԃ���A�����]�T����������0.05�b��ɑk��
                existTime = i + Application.targetFrameRate * deltaTime * 0.050f;
                break;
            }
        }

        return existTime;
    }

    public void Replay(float localTime)
    {
        // ���v���C���Ń{�[�������݂��Ȃ��ꍇ
        if (isExist.Evaluate(localTime) < 1.0f)
        {
            replayBall.SetActive(false);
            return;
        }

        replayBall.SetActive(true);

        replayBall.transform.SetPositionAndRotation(
            new Vector3(positionX.Evaluate(localTime), positionY.Evaluate(localTime), positionZ.Evaluate(localTime)), 
            new Quaternion(quaternionX.Evaluate(localTime), quaternionY.Evaluate(localTime), quaternionZ.Evaluate(localTime), quaternionW.Evaluate(localTime)));
    }

    private int CountObjectAmount(string tagName)
    {
        GameObject[] tagObjects = GameObject.FindGameObjectsWithTag(tagName);
        return tagObjects.Length;
    }
}
