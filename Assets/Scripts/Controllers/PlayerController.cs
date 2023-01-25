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
    private float CD;                //������ȴʱ��
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

    //��ֹ�л���������
    private void OnDisable()
    {
        if (!MouseManager.isInitialized) return;
        MouseManager.Instance.OnMouseClicked -= Move2Target;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }
    void Update()
    {
        dead = data.curHealth == 0;
        //����������㲥�����е���
        if (dead)
        {
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();
        CD -= Time.deltaTime;
        //TODO:ʵ����սһ��ʱ����Զ���Ѫ
    }
    private void SwitchAnimation()
    {
        animator.SetFloat("speed", agent.velocity.sqrMagnitude);
        animator.SetBool("dead", dead);
    }
    //�ƶ�ָ��
    public void Move2Target(Vector3 target)
    {
        //ȡ�������ж�
        StopAllCoroutines();
        if (dead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    //����ָ��
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
        //���ݹ�������������ֹͣλ��
        agent.stoppingDistance = data.attackData.attackRange;
        //ת�򹥻�Ŀ��
        transform.LookAt(attackObj.transform);
        //�������
        while (attackObj != null && Vector3.Distance(attackObj.transform.position, transform.position) > data.attackData.attackRange)
        {
            agent.destination = attackObj.transform.position;
            yield return null; //��һ֡���Ѹ�Э�̣���while������ִ�г���
        }
        agent.isStopped = true;
        if (CD < 0)
        {
            data.broken = false;
            animator.SetBool("critical", data.isCritical);
            animator.SetTrigger("attack");
            CD = data.attackData.cooldown;
        }
        //�Զ�׷����
        //while(attackObj!=null)
        //{
        //    //����������Χ��׷��
        //    if(Vector3.Distance(attackObj.transform.position, transform.position) > data.attackData.attackRange)
        //    {
        //        agent.destination = attackObj.transform.position;
        //        yield return null; //��һ֡���Ѹ�Э�̣���while������ִ�г���
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
    //�����ؼ�֡�������Ч��
    void hit()
    {
        if (attackObj.CompareTag("Attackable"))
        {
            //ʯͷ����Ժ���Խ�ʯͷ�����ȥ
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
