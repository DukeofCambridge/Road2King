using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject attackObj;
    private CharacterData data;
    private float CD;                //攻击冷却时间
    private bool dead;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        data = GetComponent<CharacterData>();
    }
    
    void Start()
    {
        //指令类型
        MouseManager.Instance.OnMouseClicked += Move2Target;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RegisterPlayer(data);
    }
    void Update()
    {
        dead = data.curHealth == 0;
        //玩家死亡，广播给所有敌人
        if (dead)
        {
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();
        CD -= Time.deltaTime;
    }
    private void SwitchAnimation()
    {
        animator.SetFloat("speed", agent.velocity.sqrMagnitude);
        animator.SetBool("dead", dead);
    }
    //移动指令
    public void Move2Target(Vector3 target)
    {
        //取消其他行动
        StopAllCoroutines();
        if (dead) return;
        agent.isStopped = false;
        agent.destination = target;
    }
    //攻击指令
    public void EventAttack(GameObject obj)
    {
        if (dead) return;
        if (obj != null)
        {
            attackObj = obj;
            data.isCritical = UnityEngine.Random.value < data.attackData.criticalRate;
            StartCoroutine(Move2Attack());
        }
        
    }
    IEnumerator Move2Attack()
    {
        agent.isStopped = false;
        //转向攻击目标
        transform.LookAt(attackObj.transform);
        //TODO: 追踪过程中也需要判断怪物是否为null，有可能半路被别的东西搞死了
        while (Vector3.Distance(attackObj.transform.position, transform.position) > data.attackData.attackRange)
        {
            agent.destination = attackObj.transform.position;
            yield return null; //下一帧唤醒该协程，从while处继续执行程序
        }
        agent.isStopped = true;
        if (CD < 0)
        {
            animator.SetBool("critical", data.isCritical);
            animator.SetTrigger("attack");
            CD = data.attackData.cooldown;
        }
    }
    //动画关键帧触发打击效果
    void hit()
    {
        var targetData = attackObj.GetComponent<CharacterData>();
        targetData.takeDamage(data, targetData);
    }
}
