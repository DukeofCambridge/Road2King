using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GruntController : EnemyController
{
    [Header("Skill")]
    public float kickForce = 15f;
    public void Kickoff()
    {
        if (atkTarget != null)
        {
            transform.LookAt(atkTarget.transform);
            Vector3 direction = atkTarget.transform.position - transform.position;
            direction.Normalize();
            atkTarget.GetComponent<NavMeshAgent>().isStopped = true;
            atkTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            atkTarget.GetComponent<Animator>().SetTrigger("dizzy");
        }
    }
}
