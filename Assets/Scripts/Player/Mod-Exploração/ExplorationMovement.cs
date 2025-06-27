using UnityEngine;

public class ExplorationMovement : MonoBehaviour
{
    [Header("Referência ao Animator")]
    public Animator animator;


    public void UpdateAnimator(Vector2 moveInput, bool isJumping, bool isSliding, bool isCrouching)
    {
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSliding", isSliding);
        animator.SetBool("IsCrouching", isCrouching);
    }

    public void SetVaulting(bool isVaulting)
    {
        animator.SetBool("IsVaulting", isVaulting);
    }

    public void SetClimbing(bool isClimbing)
    {
        animator.SetBool("IsClimbingStairs", isClimbing);
    }

    public void TriggerInteraction(string type)
    {
        switch (type)
        {
            case "Chest":
                animator.SetTrigger("TriggerOpenChest");
                break;
            case "Box":
                animator.SetTrigger("TriggerPushBox");
                break;
        }
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetJumping(bool isJumping)
    {
        animator.SetBool("IsJumping", isJumping);
    }

    public void SetSliding(bool isSliding)
    {
        animator.SetBool("IsSliding", isSliding);
    }

    public void SetCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    public void TriggerVaultType(int vaultType)
    {
        Debug.Log("VaultType recebido: " + vaultType);
        animator.SetInteger("VaultType", vaultType);
        animator.SetTrigger("DoVault");
    }

public void TriggerVault(string triggerName)
{
    if (animator != null)
    {
        animator.SetTrigger(triggerName);
    }
}



}
