using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private Animator Animator;
    [SerializeField]
    private float StillDelay = 1f;
    private LookAtIK LookAt;
    private NavMeshAgent Agent;

    private const string IsWalking = "IsWalking";

    private static NavMeshTriangulation Triangulation;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        LookAt = GetComponent<LookAtIK>();
        if (Triangulation.vertices == null || Triangulation.vertices.Length == 0)
        {
            Triangulation = NavMesh.CalculateTriangulation();
        }
    }

    private void Start()
    {
        StartCoroutine(Roam());
    }

    private void Update()
    {
        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
        if (LookAt != null)
        {
            LookAt.lookAtTargetPosition = Agent.steeringTarget + transform.forward;
        }
    }

    private IEnumerator Roam()
    {
        WaitForSeconds wait = new WaitForSeconds(StillDelay);

        while (enabled)
        {
            int index = Random.Range(1, Triangulation.vertices.Length);
            Agent.SetDestination(
                Vector3.Lerp(
                    Triangulation.vertices[index - 1],
                    Triangulation.vertices[index],
                    Random.value
                )
            );
            yield return new WaitUntil(() => Agent.remainingDistance <= Agent.stoppingDistance);
            yield return wait;
        }
    }

    public void StopMoving()
    {
        StopAllCoroutines();
        Agent.isStopped = true;
        Agent.enabled = false;
    }
}