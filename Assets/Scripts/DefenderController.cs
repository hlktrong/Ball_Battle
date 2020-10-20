using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderController : SoliderController
{
    public GameObject Detection_Range;
    private Vector3 m_originPosition;
    // Start is called before the first frame update
    void Start()
    {
        speed = GameController.Instance.NormalSpeedDefender;
        anim = GetComponent<Animator>();
        m_originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_currentState)
        {
            case SoliderState.Spawn:
                UpdateSpawn();
            break;
            case SoliderState.Standby:
                UpdateStandby();
            break;
            case SoliderState.ChaseAttacker:
                UpdateChaseAttacker();
            break;
            case SoliderState.Inactive:
                UpdateInactive();
            break;
            case SoliderState.RunBackToHome:
                UpdateRunBackToHome();
            break;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(m_currentState == SoliderState.ChaseAttacker)
        {
            if(other.gameObject == GameController.Instance.getAttackerHoldBall())
            {
                GameController.Instance.getAttackerHoldBall().GetComponent<AttackerController>().setStateInactive();
                setStateInactive();
                anim.SetTrigger("Catch");
                GameController.Instance.SetOthersDefenderReturnToHome();
            }
        }
    }
    void UpdateSpawn()
    {
        if(m_inactive_time > 0)
            m_inactive_time -= Time.deltaTime;
        else
        {
            setStateStandby();
        }
    }
    void UpdateStandby()
    {
        if(Detection_Range.GetComponent<DetectionRangeController>().isAnyAttackerInRange())
        {
            Detection_Range.SetActive(false);
            setStateChaseAttacker();
        }
    }
    void UpdateChaseAttacker()
    {
        MoveToTarget(getTarget().transform.position);
    }
    void UpdateRunBackToHome()
    {
        
        if(Vector3.Distance(this.transform.position, m_originPosition) > 0.01)
        {
            MoveToTarget(m_originPosition);
        }
        else
        {
            setStateStandby();
        }
    }
    void UpdateInactive()
    {
        if(m_inactive_time > 0)
        {
            m_inactive_time -= Time.deltaTime;
        }
        else
        {
            setStateStandby();
        }
        
        if(Vector3.Distance(this.transform.position, m_originPosition) > 0 )
        {
            MoveToTarget(m_originPosition);
        }
    }
    void MoveToTarget(Vector3 target)
    {
        Vector3 newPos = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * speed);
        newPos.y = this.transform.position.y;
        this.transform.position = newPos;
        rotateToTarget(target);
    }


    public void setSpawnState()
    {
        Arrow.SetActive(false);
        m_inactive_time = GameController.Instance.SpawnTimeDefender;
        meshRenderer.material.color = Color.gray;
        switchState(SoliderState.Spawn);
    }
    public void setStateStandby()
    {
        Arrow.SetActive(false);
        meshRenderer.material.color = getActiveColor();
        Detection_Range.SetActive(true);
        switchState(SoliderState.Standby);
    }
    public void setStateChaseAttacker()
    {
        Arrow.SetActive(true);
        speed = GameController.Instance.NormalSpeedDefender;
        setTarget(GameController.Instance.getAttackerHoldBall());
        switchState(SoliderState.ChaseAttacker);
    }
    public void setStateInactive()
    {
        Arrow.SetActive(true);
        m_inactive_time = GameController.Instance.ReactiveTimeDefender;
        meshRenderer.material.color = Color.gray;
        speed = GameController.Instance.ReturnSpeedDefender;
        switchState(SoliderState.Inactive);
    }
    public void setStateRunBackToHome()
    {
        Arrow.SetActive(true);
        speed = GameController.Instance.ReturnSpeedDefender;
        switchState(SoliderState.RunBackToHome);
    }
    public Color getActiveColor()
    {
        if(GameController.Instance.EnemyRole == GameController.Role.Attacker)
        {
            return Color.cyan;
        }
        else
        {
            return Color.red;
        }
    }
    public bool isChaseAttacker()
    {
        return m_currentState == SoliderState.ChaseAttacker;
    }
}
