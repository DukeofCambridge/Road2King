using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { GUARD, PATROL, ATTACK, DEAD}
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private EnemyState state;
    private float CD;             //攻击冷却时间
    private CharacterData data;
    private Collider collider;
    //动画控制参数
    bool walk;
    bool ready4atk;
    bool follow;
    bool dead;
    
   
    [Header("Basic Settings")]
    public float sightRadius;
    private GameObject atkTarget;
    private float speed;          //怪物移动标准速度
    public int type;          //敌人类型 0-站桩 1-巡逻
    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 waypoint;
    private Vector3 guardPoint;   //起始位置
    public float watchTime;       //标准望风时间
    private float remainedWatchTime;//剩余望风时间
    private Quaternion guardRotation;//起始角度

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
        guardPoint = transform.position;
        guardRotation = transform.rotation;
        //Debug.Log("guardP:" + transform.position);
        remainedWatchTime = watchTime;
        data = GetComponent<CharacterData>();
        collider = GetComponent<Collider>();

    }
    private void Start()
    {
        if (type == 0)
        {
            state = EnemyState.GUARD;
        }else if (type == 1)
        {
            state = EnemyState.PATROL;
            getNewDestination();
        }
    }
    private void Update()
    {
        dead = data.curHealth == 0;
        switchState();
        switchAnimation();
        CD -= Time.deltaTime;
    }
    //动画控制器
    void switchAnimation()
    {
        anim.SetBool("walk", walk);
        anim.SetBool("ready4atk", ready4atk);
        anim.SetBool("follow", follow);
        anim.SetBool("critical", data.isCritical);
        anim.SetBool("dead", dead);
    }
    
    //怪物状态控制器
    void switchState()
    {
        if (dead)
        {
            state = EnemyState.DEAD;
        }
        else if (findPlayer())
        {
            state = EnemyState.ATTACK;
            //Debug.Log("GOTCHA!");
        }
        switch (state)
        {
            case EnemyState.ATTACK:
                chase();
                break;
            case EnemyState.PATROL:
                patrol();
                break;
            case EnemyState.GUARD:
                guard();
                break;
            case EnemyState.DEAD:
                death();
                break;
        }
    }
    bool findPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                atkTarget = target.gameObject;
                return true;
            }
        }
        atkTarget = null;
        return false;
    }
    //追击状态逻辑
    void chase()
    {
        walk = false;
        ready4atk = true;
        agent.speed = speed;
        if (findPlayer())
        {
            remainedWatchTime = watchTime; //为了使玩家脱离敌人视野时，敌人仍能原地驻足片刻（如果巡逻敌人在望风过程中发现敌人，望风时间已经损耗一部分，会减少脱战时的望风时间）
            follow = true;
            agent.isStopped = false;
            agent.destination = atkTarget.transform.position;
        }
        else
        {
            //玩家离开视野后怪物立刻停止行动，而非到达“最后目击位置”，并在原地停留片刻
            follow = false;
            if (remainedWatchTime > 0)
            {
                agent.destination = transform.position;
                remainedWatchTime -= Time.deltaTime;
            }
            else if (type == 0)
            {
                state = EnemyState.GUARD;
            }
            else if (type == 1)
            {
                state = EnemyState.PATROL;
            }
        }
        //玩家进入怪物攻击范围内，执行攻击
        if (withinAtkRange() || withinSkillRange())
        {
            follow = false;
            agent.isStopped = true;
            if (CD < 0)
            {
                CD = data.attackData.cooldown;
                //判断暴击
                data.isCritical = Random.value < data.attackData.criticalRate;
                //执行攻击
                attack();
            }
        }
    }
    //巡逻状态逻辑
    void patrol()
    {
        ready4atk = false;
        agent.speed = speed * 0.5f;
        //判断是否到达下一巡逻点位
        if (Vector3.Distance(waypoint, transform.position) <= agent.stoppingDistance)
        {
            walk = false;
            //望风时间结束后才会去下一巡逻点位
            if (remainedWatchTime > 0)
            {
                remainedWatchTime -= Time.deltaTime;
            }
            else
            {
                remainedWatchTime = watchTime;
                getNewDestination();
            }
                
        }
        else
        {
            walk = true;
            agent.destination = waypoint;
        }
    }
    //守卫状态逻辑
    void guard()
    {
        ready4atk = false;
        if (transform.position != guardPoint)
        {
            walk = true;
            agent.isStopped = false;
            agent.destination = guardPoint;
            if (Vector3.SqrMagnitude(guardPoint-transform.position) <= agent.stoppingDistance)
            {
                walk = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.1f);
            }
        }
    }
    //死亡状态逻辑
    void death()
    {
        collider.enabled = false;
        agent.enabled = false;
        Destroy(gameObject, 2f);
    }
    //选中敌人时绘制其视野范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    //巡逻范围内随机选择一点
    void getNewDestination()
    {
        float dx = Random.Range(-patrolRange, patrolRange);
        float dz = Random.Range(-patrolRange, patrolRange);
        Vector3 newPoint = new Vector3(guardPoint.x + dx,transform.position.y, guardPoint.z + dz);
        //判断新位置是否可达
        NavMeshHit hit;
        waypoint = NavMesh.SamplePosition(newPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    //判断是否处于普通攻击范围
    bool withinAtkRange()
    {
        if (atkTarget != null)
        {
            return Vector3.Distance(atkTarget.transform.position, transform.position) <= data.attackData.attackRange;
        }
        return false;
    }
    //判断是否处于技能攻击范围
    bool withinSkillRange()
    {
        if (atkTarget != null)
        {
            return Vector3.Distance(atkTarget.transform.position, transform.position) <= data.attackData.skillRange;
        }
        return false;
    }
    //执行攻击动作
    void attack()
    {
        transform.LookAt(atkTarget.transform);
        if (withinAtkRange())
        {
            anim.SetTrigger("attack");
        }
        if (withinSkillRange())
        {
            anim.SetTrigger("skill");
        }
    }
    //动画关键帧触发打击效果
    void hit()
    {
        if (atkTarget != null)
        {
            var targetData = atkTarget.GetComponent<CharacterData>();
            targetData.takeDamage(data, targetData);
        }
    }
}
