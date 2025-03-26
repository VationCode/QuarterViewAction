using DUS;
using UnityEngine;

public class StartStage : MonoBehaviour
{
    [SerializeField]
    GameManager m_gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_gameManager.StageStart();
        }
    }
}
