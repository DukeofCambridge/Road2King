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
    //�������Ʋ���
    bool walk;
    bool ready4atk;
    bool follow;
    
   
    [Header("Basic Settings")]
    public float sightRadius;
    private GameObject atkTarget;
    private float speed;          //�����ƶ���׼�ٶ�
    public int type = 0;          //�������� 0-վ׮ 1-Ѳ��
    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 waypoint;
    private Vector3 guardPoint;   //��ʼλ��

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
        guardPoint = transform.position;
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
        switchState();
        switchAnimation();
    }

    void switchAnimation()
    {
        anim.SetBool("walk", walk);
        anim.SetBool("ready4atk", ready4atk);
        anim.SetBool("follow", follow);
    }
    bool findPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var target in colliders)
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
    void switchState()
    {
        if (findPlayer())
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
                break;
        }
    }
    //׷��״̬�߼�
    void chase()
    {
        walk = false;
        ready4atk = true;
        agent.speed = speed;
        if (findPlayer())
        {
            follow = true;
            agent.destination = atkTarget.transform.position;
        }
        else
        {
            follow = false;
            //����뿪��Ұ���������ֹͣ�ж������ǵ�����Ŀ��λ�á�
            agent.destination = transform.position;
        }
    }
    //Ѳ��״̬�߼�
    void patrol()
    {
        ready4atk = false;
        agent.speed = speed * 0.5f;
        if (Vector3.Distance(waypoint, transform.position) <= agent.stoppingDistance)
        {
            walk = false;
            getNewDestination();
        }
        else
        {
            walk = true;
            agent.destination = waypoint;
        }
    }
    //ѡ�е���ʱ��������Ұ��Χ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(guardPoint, sightRadius);
    }
    //Ѳ�߷�Χ�����ѡ��һ��
    void getNewDestination()
    {
        float dx = Random.Range(-patrolRange, patrolRange);
        float dz = Random.Range(-patrolRange, patrolRange);
        Vector3 newPoint = new Vector3(guardPoint.x + dx,transform.position.y, guardPoint.z + dz);
        NavMeshHit hit;
        waypoint = NavMesh.SamplePosition(newPoint, out hit, patrolRange, 1) ? newPoint : transform.position;
    }
}
