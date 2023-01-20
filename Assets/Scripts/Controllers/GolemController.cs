using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemController : EnemyController
{
    [Header("Skill")]
    public float kickForce;
    public GameObject rockPrefab;
    public Transform handPos;
    public void KickoffandTakeDamage()
    {
        if (atkTarget != null && transform.IsFacingTarget(atkTarget.transform)&&!data.broken)
        {
            transform.LookAt(atkTarget.transform);
            //TODO:测试是否能实现击飞效果（rigidbody设置为kinematic即可避免与NavMeshAgent发生冲突）
            Vector3 direction = (atkTarget.transform.position - transform.position).normalized;
            atkTarget.GetComponent<NavMeshAgent>().isStopped = true;
            atkTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            atkTarget.GetComponent<Animator>().SetTrigger("dizzy");
            var targetData = atkTarget.GetComponent<CharacterData>();
            targetData.takeDamage(data, targetData);
        }
    }
    public void ThrowRock()
    {
        if (atkTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<RockController>().target = atkTarget;
        }
    }
}
