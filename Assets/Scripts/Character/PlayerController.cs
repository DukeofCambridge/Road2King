using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject attackObj;
    private float CD;                //攻击冷却时间
    private float atkDistance = 1;   //攻击距离
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += move2Target;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }
    void Update()
    {
        switchAnimation();
        CD -= Time.deltaTime;
    }
    private void switchAnimation()
    {
        animator.SetFloat("speed", agent.velocity.sqrMagnitude);
    }
    public void move2Target(Vector3 target)
    {
        //取消其他行动
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;
    }
    public void EventAttack(GameObject obj)
    {
        if (obj != null)
        {
            attackObj = obj;
            StartCoroutine(move2Attack());
        }
        
    }

    IEnumerator move2Attack()
    {
        agent.isStopped = false;
        //转向攻击目标
        transform.LookAt(attackObj.transform);
        //TODO: 追踪过程中也需要判断怪物是否为null，有可能半路被别的东西搞死了
        while (Vector3.Distance(attackObj.transform.position, transform.position) > atkDistance)
        {
            agent.destination = attackObj.transform.position;
            yield return null; //下一帧唤醒该协程，从while处继续执行程序
        }
        agent.isStopped = true;
        if (CD < 0)
        {
            animator.SetTrigger("attack");
            CD = 0.5f;
        }
    }
}
