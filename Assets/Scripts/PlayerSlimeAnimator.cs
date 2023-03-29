using UnityEngine;

public class PlayerSlimeAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int JumpParamHash = Animator.StringToHash("Jump");
    private static readonly int SpeedParamHash = Animator.StringToHash("Speed");

    private bool isWalking;

    
    
    public void StartWalk()
    {
        isWalking = true;
        animator.SetTrigger(JumpParamHash);

    }

    public void StopWalk() => isWalking = false;

    // Used by animator event
    public void OnJumpEnd()
    {
        if(isWalking) animator.Play(JumpParamHash);
        else StopWalk();
    }
}
