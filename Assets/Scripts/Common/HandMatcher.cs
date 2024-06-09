using UnityEngine;

public class HandMatcher : MonoBehaviour
{
    [SerializeField]
    private DominantHand dominantHand;

    // IK������s���̂̕���
    private AvatarIKGoal ikGoal;

    [SerializeField]
    private Transform handPoint;

    private Animator animator;

    private float ikStrength;

    // IK����L�����t���O
    public bool isEnableIK = true;

    // Start is called before the first frame update
    void Start()
    {
        if (dominantHand == DominantHand.right) { ikGoal = AvatarIKGoal.LeftHand; }
        else if (dominantHand == DominantHand.left) { ikGoal = AvatarIKGoal.RightHand; }
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK()
    {
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("ReceiverMovingBlendTree")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("MovingBlendTree"))
            && animator.GetCurrentAnimatorStateInfo(1).IsName("humanoid_idle"))
        {
            // OnAnimatorIK��IK��������X�V����ۂɌĂ΂��R�[���o�b�N
            if (!isEnableIK) { return; }

            ikStrength += Time.deltaTime;
            if (ikStrength > 1.0f) { ikStrength = 1.0f; }

            // ����ɍ쐬����LeftHandPoint�ɁA������ړ�������
            animator.SetIKPositionWeight(ikGoal, ikStrength);
            animator.SetIKRotationWeight(ikGoal, ikStrength);
            animator.SetIKPosition(ikGoal, handPoint.position);
            animator.SetIKRotation(ikGoal, handPoint.rotation);
        }
        else { ikStrength = 0.0f; }
    }
}
