using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoliderController : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public GameObject Arrow;
    protected Animator anim;
    public enum SoliderState {Spawn, 
                        ChaseBall, 
                        HoldBall, 
                        PassBall,
                        Freedom,
                        Inactive,
                        Standby,
                        ChaseAttacker,
                        RunBackToHome,
                        };

    [SerializeField]
    protected SoliderState m_currentState;
    protected GameObject m_target;
    protected float speed;
    protected float m_inactive_time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchState(SoliderState nextState )
    {
        m_currentState = nextState;
    }
    public void setTarget(GameObject target)
    {
        m_target = target;
    }
    public GameObject getTarget()
    {
        return m_target;
    }
    protected void rotateToTarget(Vector3 target)
    {
        Vector3 target_pos = target;
        target_pos.y = transform.position.y;
        Vector3 targetDirection = target_pos - transform.position;
        float singleStep = speed * 10 * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards( transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
