using UnityEngine;

public class ButtonToggle : MonoBehaviour
{
    private Animator animator;
    private bool isOn = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }
    public void ToggleAnimation()
    {
        animator.enabled = true;
        isOn = !isOn;
        animator.SetBool("Start", isOn);
    }
}