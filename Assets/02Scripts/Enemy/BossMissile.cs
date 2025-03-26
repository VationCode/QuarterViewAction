using DUS;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public GameObject target;
    [SerializeField]
    NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        if(navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(target.transform.position);
            Timer();
        }
    }
}
