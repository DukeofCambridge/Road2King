using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { GUARD, PATROL, ATTACK, DEAD}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterData))]
[RequireComponent(typeof(BoxCollider))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private NavMeshAgent agent;
    private Animator anim;
    private EnemyState state;
    private float CD;             //������ȴʱ��
    private CharacterData data;
    private Collider collider;
    //�������Ʋ���
    bool walk;
    bool ready4atk;
    bool follow;
    bool dead;
    bool win;       //�������
    
   
    [Header("Basic Settings")]
    public float sightRadius;
    protected GameObject atkTarget;
    private float speed;          //�����ƶ���׼�ٶ�
    public int type;          //�������� 0-վ׮ 1-Ѳ��
    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 waypoint;
    private Vector3 guardPoint;   //��ʼλ��
    public float watchTime;       //��׼����ʱ��
    private float remainedWatchTime;//ʣ������ʱ��
    private Quaternion guardRotation;//��ʼ�Ƕ�

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
        GameManager.Instance.AddObserver(this);
    }
    private void Update()
    {
        dead = data.curHealth == 0;
        if (!win)
        {
            SwitchState();
            SwitchAnimation();
            CD -= Time.deltaTime;
        }
    }
    //private void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    private void OnDisable()
    {
        if (!GameManager.isInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    //����������
    void SwitchAnimation()
    {
        anim.SetBool("walk", walk);
        anim.SetBool("ready4atk", ready4atk);
        anim.SetBool("follow", follow);
        anim.SetBool("critical", data.isCritical);
        anim.SetBool("dead", dead);
    }
    
    //����״̬������
    void SwitchState()
    {
        if (dead)
        {
            state = EnemyState.DEAD;
        }
        else if (FindPlayer())
        {
            state = EnemyState.ATTACK;
            //Debug.Log("GOTCHA!");
        }
        switch (state)
        {
            case EnemyState.ATTACK:
                Chase();
                break;
            case EnemyState.PATROL:
                Patrol();
                break;
            case EnemyState.GUARD:
                Guard();
                break;
            case EnemyState.DEAD:
                Death();
                break;
        }
    }
    bool FindPlayer()
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
    //׷��״̬�߼�
    void Chase()
    {
        walk = false;
        ready4atk = true;
        agent.speed = speed;
        //������ȴʱ���ڲ�����׷��
        if (FindPlayer()&&CD<=0)
        {
            remainedWatchTime = watchTime; //Ϊ��ʹ������������Ұʱ����������ԭ��פ��Ƭ�̣����Ѳ�ߵ�������������з��ֵ��ˣ�����ʱ���Ѿ����һ���֣��������սʱ������ʱ�䣩
            follow = true;
            agent.isStopped = false;
            agent.destination = atkTarget.transform.position;
        }
        else
        {
            //����뿪��Ұ���������ֹͣ�ж������ǵ�����Ŀ��λ�á�������ԭ��ͣ��Ƭ��
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
        //��ҽ�����﹥����Χ�ڣ�ִ�й���
        if (withinAtkRange() || withinSkillRange())
        {
            follow = false;
            agent.isStopped = true;
            if (CD < 0)
            {
                CD = data.attackData.cooldown;
                //�жϱ���
                data.isCritical = Random.value < data.attackData.criticalRate;
                //ִ�й���
                attack();
            }
        }
    }
    //Ѳ��״̬�߼�
    void Patrol()
    {
        ready4atk = false;
        agent.speed = speed * 0.5f;
        //�ж��Ƿ񵽴���һѲ�ߵ�λ
        if (Vector3.Distance(waypoint, transform.position) <= agent.stoppingDistance)
        {
            walk = false;
            //����ʱ�������Ż�ȥ��һѲ�ߵ�λ
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
    //����״̬�߼�
    void Guard()
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
    //����״̬�߼�
    void Death()
    {
        collider.enabled = false;
        //agent.enabled = false;
        agent.radius = 0;
        Destroy(gameObject, 2f);
    }
    //ѡ�е���ʱ��������Ұ��Χ��Ѳ�߷�Χ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
    //Ѳ�߷�Χ�����ѡ��һ��
    void getNewDestination()
    {
        float dx = Random.Range(-patrolRange, patrolRange);
        float dz = Random.Range(-patrolRange, patrolRange);
        Vector3 newPoint = new Vector3(guardPoint.x + dx,transform.position.y, guardPoint.z + dz);
        //�ж���λ���Ƿ�ɴ�
        NavMeshHit hit;
        waypoint = NavMesh.SamplePosition(newPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    //�ж��Ƿ�����ͨ������Χ
    bool withinAtkRange()
    {
        if (atkTarget != null)
        {
            return Vector3.Distance(atkTarget.transform.position, transform.position) <= data.attackData.attackRange;
        }
        return false;
    }
    //�ж��Ƿ��ڼ��ܹ�����Χ
    bool withinSkillRange()
    {
        if (atkTarget != null)
        {
            return Vector3.Distance(atkTarget.transform.position, transform.position) <= data.attackData.skillRange;
        }
        return false;
    }
    //ִ�й�������
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
    //�����ؼ�֡�������Ч��
    void hit()
    {
        //�жϹ���Ŀ���Ƿ��ڹ��������ڣ���ǰ��
        if (atkTarget != null && transform.IsFacingTarget(atkTarget.transform))
        {
            var targetData = atkTarget.GetComponent<CharacterData>();
            targetData.takeDamage(data, targetData);
        }
    }

    public void GetNotified()
    {
        anim.SetBool("win", true);
        walk = false;
        ready4atk = false;
        follow = false;
        atkTarget = null;
        //agent.isStopped = true;
    }
}
