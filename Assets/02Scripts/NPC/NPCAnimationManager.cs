using UnityEngine;

public class NPCAnimationManager : MonoBehaviour
{
    Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    public void Hello( )
    {
        m_animator.SetTrigger("doHello");
    }
}
