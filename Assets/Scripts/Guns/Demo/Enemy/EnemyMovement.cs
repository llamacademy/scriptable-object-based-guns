using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Guns.Demo.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public class EnemyMovement : MonoBehaviour, ISlowable, IKnockbackable
    {
        private Animator Animator;
        [SerializeField]
        private float StillDelay = 1f;

        [field: SerializeField] public float StillThreshold { get; set; } = 0.01f;
        private LookAtIK LookAt;
        private NavMeshAgent Agent;
        private Rigidbody Rigidbody;

        private Coroutine SlowCoroutine;
        private Coroutine MoveCoroutine;
        private float BaseSpeed;
        private const string IsWalking = "IsWalking";

        private static NavMeshTriangulation Triangulation;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            Agent = GetComponent<NavMeshAgent>();
            Rigidbody = GetComponent<Rigidbody>();
            LookAt = GetComponent<LookAtIK>();
            if (Triangulation.vertices == null || Triangulation.vertices.Length == 0)
            {
                Triangulation = NavMesh.CalculateTriangulation();
            }
            
            BaseSpeed = Agent.speed;
        }

        private void Start()
        {
            MoveCoroutine = StartCoroutine(Roam());
            BaseSpeed = Agent.speed;
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

        public void Slow(AnimationCurve SlowCurve)
        {
            if (SlowCoroutine != null)
            {
                StopCoroutine(SlowCoroutine);
            }
            SlowCoroutine = StartCoroutine(SlowDown(SlowCurve));
        }

        private IEnumerator SlowDown(AnimationCurve SlowCurve)
        {
            float time = 0;

            while (time < SlowCurve.keys[^1].time)
            {
                Agent.speed = BaseSpeed * SlowCurve.Evaluate(time);
                time += Time.deltaTime;
                yield return null;
            }

            Agent.speed = BaseSpeed;
        }

        public void GetKnockedBack(Vector3 Force, float MaxMoveTime)
        {
            StopCoroutine(MoveCoroutine);
            MoveCoroutine = StartCoroutine(ApplyKnockback(Force, MaxMoveTime));
        }

        private IEnumerator ApplyKnockback(Vector3 Force, float MaxMoveTime)
        {
            yield return null;
            Agent.enabled = false;
            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = false;
            Rigidbody.AddForce(Force);
            
            yield return new WaitForFixedUpdate();
            float knockbackTime = Time.time;
            yield return new WaitUntil(
                () => Rigidbody.velocity.magnitude < StillThreshold || Time.time > knockbackTime + MaxMoveTime 
            );
            yield return new WaitForSeconds(0.25f);

            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;
            Agent.Warp(transform.position);
            Agent.enabled = true;

            yield return null;

            MoveCoroutine = StartCoroutine(Roam());
        }
    }
}