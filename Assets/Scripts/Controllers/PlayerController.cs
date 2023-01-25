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
    private float stopDistance;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        data = GetComponent<CharacterData>();
        stopDistance = agent.stoppingDistance;
    }
    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += Move2Target;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }
    void Start()
    {
        GameManager.Instance.RegisterPlayer(data);
    }

    //防止切换场景报错
    private void OnDisable()
    {
        if (!MouseManager.isInitialized) return;
        MouseManager.Instance.OnMouseClicked -= Move2Target;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
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
        //TODO:实现脱战一段时间后自动回血
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
        agent.stoppingDistance = stopDistance;
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
        //根据攻击距离调整玩家停止位置
        agent.stoppingDistance = data.attackData.attackRange;
        //转向攻击目标
        transform.LookAt(attackObj.transform);
        //初版代码
        while (attackObj != null && Vector3.Distance(attackObj.transform.position, transform.position) > data.attackData.attackRange)
        {
            agent.destination = attackObj.transform.position;
            yield return null; //下一帧唤醒该协程，从while处继续执行程序
        }
        agent.isStopped = true;
        if (CD < 0)
        {
            data.broken = false;
            animator.SetBool("critical", data.isCritical);
            animator.SetTrigger("attack");
            CD = data.attackData.cooldown;
        }
        //自动追击版
        //while(attackObj!=null)
        //{
        //    //超出攻击范围，追踪
        //    if(Vector3.Distance(attackObj.transform.position, transform.position) > data.attackData.attackRange)
        //    {
        //        agent.destination = attackObj.transform.position;
        //        yield return null; //下一帧唤醒该协程，从while处继续执行程序
        //    }else if (CD < 0)
        //    {
        //        agent.isStopped = true;
        //        animator.SetBool("critical", data.isCritical);
        //        animator.SetTrigger("attack");
        //        CD = data.attackData.cooldown;
        //        yield return null;
        //    }

        //}
    }
    //动画关键帧触发打击效果
    void hit()
    {
        if (attackObj.CompareTag("Attackable"))
        {
            //石头落地以后可以将石头击打回去
            if (attackObj.GetComponent<RockController>()&& attackObj.GetComponent<RockController>().state==RockController.RockState.Still)
            {
                attackObj.GetComponent<RockController>().state = RockController.RockState.HitEnemy;
                attackObj.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackObj.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.Impulse);
            }
        }
        else if(attackObj != null && !data.broken)
        {
            var targetData = attackObj.GetComponent<CharacterData>();
            targetData.TakeDamage(data, targetData);
        }

    }
}
