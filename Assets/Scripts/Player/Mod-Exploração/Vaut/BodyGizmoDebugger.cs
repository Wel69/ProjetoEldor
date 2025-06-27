using UnityEngine;

public class BodyGizmoDebugger : MonoBehaviour
{
    public Color handsColor = Color.red;
    public Color feetColor = Color.blue;
    public Color headColor = Color.green;
    public float sphereSize = 0.05f;

    private Animator animator;

    void OnDrawGizmos()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null) return;

        // Mãos
        Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

        // Pés
        Transform leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        Transform rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

        // Cabeça
        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);

        // 🟥 Mãos
        Gizmos.color = handsColor;
        if (leftHand != null) Gizmos.DrawSphere(leftHand.position, sphereSize);
        if (rightHand != null) Gizmos.DrawSphere(rightHand.position, sphereSize);

        // 🟦 Pés
        Gizmos.color = feetColor;
        if (leftFoot != null) Gizmos.DrawSphere(leftFoot.position, sphereSize);
        if (rightFoot != null) Gizmos.DrawSphere(rightFoot.position, sphereSize);

        // 🟩 Cabeça
        Gizmos.color = headColor;
        if (head != null) Gizmos.DrawSphere(head.position, sphereSize);

        // 🔥 Extra (linha da mão pra frente)
        if (leftHand != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftHand.position, leftHand.position + transform.forward * 0.5f);
        }
        if (rightHand != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rightHand.position, rightHand.position + transform.forward * 0.5f);
        }
    }
}
