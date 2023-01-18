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
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        data = GetComponent<CharacterData>();
    }
    
    void Start()
    {
        //ָ������
        MouseManager.Instance.OnMouseClicked += move2Target;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }
    void Update()
    {
        dead = data.curHealth == 0;
        switchAnimation();
        CD -= Time.deltaTime;
    }
    private void switchAnimation()
    {
        animator.SetFloat("speed", agent.velocity.sqrMagnitude);
        animator.SetBool("dead", dead);
    }
    //�ƶ�ָ��
    public void move2Target(Vector3 target)
    {
        //ȡ�������ж�
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;
    }
    //����ָ��
    public void EventAttack(GameObject obj)
    {
        if (obj != null)
        {
            attackObj = obj;
            data.isCritical = UnityEngine.Random.value < data.attackData.criticalRate;
            StartCoroutine(move2Attack());
        }
        
    }
    IEnumerator move2Attack()
    {
        agent.isStopped = false;
        //ת�򹥻�Ŀ��
        transform.LookAt(attackObj.transform);
        //TODO: ׷�ٹ�����Ҳ��Ҫ�жϹ����Ƿ�Ϊnull���п��ܰ�·����Ķ���������
        while (Vector3.Distance(attackObj.transform.position, transform.position) > data.attackData.attackRange)
        {
            agent.destination = attackObj.transform.position;
            yield return null; //��һ֡���Ѹ�Э�̣���while������ִ�г���
        }
        agent.isStopped = true;
        if (CD < 0)
        {
            animator.SetBool("critical", data.isCritical);
            animator.SetTrigger("attack");
            CD = data.attackData.cooldown;
        }
    }
    //�����ؼ�֡�������Ч��
    void hit()
    {
        var targetData = attackObj.GetComponent<CharacterData>();
        targetData.takeDamage(data, targetData);
    }
}
