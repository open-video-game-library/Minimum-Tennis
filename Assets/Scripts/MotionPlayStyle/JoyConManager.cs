using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JoyConManager : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private readonly int defaultTime = 10;
    [NonSerialized] public int swingCoolTime = 60;
    private int testTime;
    private bool tested = false;
    [NonSerialized] public string swing = null;

    private float maxAccel = 0.0f;
    private float minAccel = 0.0f;

    [NonSerialized] public bool fore = false;
    [NonSerialized] public bool back = false;
    [NonSerialized] public bool tos = false;

    public static float threhold = 4.0f;

    // Change the way you hit the button.
    [NonSerialized] public bool isPressedA;
    [NonSerialized] public bool isPressedB;
    [NonSerialized] public bool isPressedX;
    [NonSerialized] public bool isPressedY;

    void Start()
    {
        testTime = defaultTime;

        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    void Update()
    {
        if (m_joyconR != null || m_joyconL != null)
        {
            if (m_joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT)) { tos = true; }
            else { tos = false; }

            isPressedA = m_joyconR.GetButton(Joycon.Button.DPAD_RIGHT);
            isPressedB = m_joyconR.GetButton(Joycon.Button.DPAD_DOWN);
            isPressedX = m_joyconR.GetButton(Joycon.Button.DPAD_UP);
            isPressedY = m_joyconR.GetButton(Joycon.Button.DPAD_LEFT);
        }
    }

    void FixedUpdate()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count < 1) return;

        foreach (var button in m_buttons)
        {
            if (m_joyconL != null)
            {
                if (m_joyconL.GetButton(button)) { m_pressedButtonL = button; }
            }

            if (m_joyconR != null)
            {
                if (m_joyconR.GetButton(button)) { m_pressedButtonR = button; }
            }
        }

        if (m_joyconR.GetAccel().y > 1.0f && m_joyconR.GetAccel().x < -2.5f && swing == null)
        {
            testTime = defaultTime;
            swing = "back";
        }
        else if (m_joyconR.GetAccel().y < -1.8f && swing == null)
        {
            testTime = defaultTime;
            swing = "fore";
        }

        if (swing != null)
        {
            if (swing == "fore")
            {
                if (testTime > 0)
                {
                    if (m_joyconR.GetAccel().y > maxAccel) { maxAccel = m_joyconR.GetAccel().y; }
                    tested = false;
                    testTime--;
                }
                else if (testTime <= 0 && !tested)
                {
                    if (maxAccel > 6.0f) { Debug.Log("�X�}�b�V�� "); fore = true; }
                    else if (maxAccel > 3.0f) { Debug.Log("�����œ_ "); fore = true; }
                    else if (maxAccel > 1.8f) { Debug.Log("���ʂ̑œ_ "); fore = true; }
                    else if (maxAccel > 0) { Debug.Log("�Ⴂ�œ_ "); fore = true; }
                    else { Debug.Log("���ʂ̑œ_ "); fore = true; }

                    tested = true;
                }
            }
            else if (swing == "back")
            {
                if (testTime > 0)
                {
                    if (m_joyconR.GetAccel().y < minAccel) { minAccel = m_joyconR.GetAccel().y; }
                    tested = false;
                    testTime--;
                }
                else if (testTime <= 0 && !tested)
                {
                    if (minAccel < -6.0f) { Debug.Log("�X�}�b�V�� "); back = true; }
                    else if (minAccel < -3.0f) { Debug.Log("�����œ_ "); back = true; }
                    else if (minAccel < -2.0f) { Debug.Log("���ʂ̑œ_ "); back = true; }
                    else if (minAccel < 0) { Debug.Log("�Ⴂ�œ_ "); back = true; }
                    else { Debug.Log("���ʂ̑œ_ "); back = true; }

                    tested = true;
                }
            }

            swingCoolTime--;

            if (swingCoolTime < 0)
            {
                swing = null;
                fore = false;
                back = false;
                maxAccel = 0.0f;
                minAccel = 0.0f;
                testTime = defaultTime;
                swingCoolTime = 120;
            }
        }
    }

    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con ���ڑ�����Ă��܂���");
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) ���ڑ�����Ă��܂���");
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) ���ڑ�����Ă��܂���");
        }

        GUILayout.BeginHorizontal(GUILayout.Width(960));

        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key = isLeft ? "Z �L�[" : "X �L�[";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick = joycon.GetStick();
            var gyro = joycon.GetGyro();
            var accel = joycon.GetAccel();
            var orientation = joycon.GetVector();

            GUILayout.BeginVertical(GUILayout.Width(480));
            GUILayout.Label(name);
            GUILayout.Label("������Ă���{�^���F" + button);
            GUILayout.Label(string.Format("�X�e�B�b�N�F({0}, {1})", stick[0], stick[1]));
            GUILayout.Label("�W���C���F" + gyro);
            GUILayout.Label("�����x�F" + accel);
            GUILayout.Label("�X���F" + orientation);
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
    }
}