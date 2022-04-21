using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private int damage = 15;
    [SerializeField] private float attackRange = 0.75f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float angleOffset = 90;
    [SerializeField] private float minDistance = 1f;
    private PlayerInventory _player;
    private Rigidbody2D _rigidbody;
    private NavMeshAgent _agent;
    private Animator animator;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.updatePosition = false;
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(_player.transform.position, _rigidbody.position) > minDistance)
        {
            Go();
        }

        else
        {
            animator.SetBool("inDistance", true);
            _agent.nextPosition = _rigidbody.position;
            _agent.isStopped = true;
        }

    }
    private void Attack()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, ~LayerMask.NameToLayer("Player"));
        if (hitPlayer != null)
        {
            _player.TakeDamage(damage);
        }
    }
    private void Go()
    {
        animator.SetBool("inDistance", false);
        _agent.isStopped = false;

        _agent.SetDestination(_player.transform.position);

        Vector2 agentNextPosition = _agent.nextPosition;
        Vector2 dir = agentNextPosition - _rigidbody.position;
        float nextAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rigidbody.MoveRotation(nextAngle + angleOffset);

        _rigidbody.position = _agent.nextPosition;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}
