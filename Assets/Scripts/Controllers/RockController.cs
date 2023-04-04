using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RockController : MonoBehaviour
{
    public enum RockState { HitPlayer,HitEnemy,Still}
    private Rigidbody rb;
    private Vector3 direction;
    public RockState state;
    public int damage;
    public GameObject breakEffect;

    [Header("Basic Settings")]
    public float force;
    public GameObject target;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one; //��ֹ��ʼ�ٶ�Ϊ0����ʯͷ���ж�Ϊ����ؾ�ֹ
        state = RockState.HitPlayer;
        Fly2Target();
    }
    //TODO: ����rigidbody��Ҫ����fixedupdate�Ϊʲô����
    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            state = RockState.Still;
        }
    }

    public void Fly2Target()
    {
        //����ʯͷ����ʱ���������Ұ���¶�ʧĿ��
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position+Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (state)
        {
            case RockState.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    //other.gameObject.GetComponent<Animator>().SetTrigger("dizzy");
                    other.gameObject.GetComponent<CharacterData>().TakeDamage(damage, other.gameObject.GetComponent<CharacterData>());
                    state = RockState.Still;
                }
                break;
            case RockState.HitEnemy:
                if (other.gameObject.GetComponent<GolemController>())
                {
                    var otherData = other.gameObject.GetComponent<CharacterData>();
                    otherData.TakeDamage(damage, otherData);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
