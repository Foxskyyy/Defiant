using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }
}
