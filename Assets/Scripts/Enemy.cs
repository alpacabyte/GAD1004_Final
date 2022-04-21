using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int moneyDropped = 5;
    [SerializeField] private bool canCountAsZombie = true;
    [SerializeField] private int _health = 100;
    private EnemyPathfinding pathfinding;
    private Animator animator;
    private void Start()
    {
        pathfinding = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health < 0 && !animator.GetBool("isDead"))
        {
            StartDyingAnimation();
        }
    }

    private void StartDyingAnimation()
    {
        Destroy(pathfinding);
        animator.WriteDefaultValues();
        animator.SetBool("isDead", true);

        GameController.Instance.ZombieKilled(canCountAsZombie, moneyDropped);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
