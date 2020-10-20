using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : SoliderController
{
    public GameObject Goal;
    public GameObject BallHolderHighlight;


    
    void Start()
    {
        anim = GetComponent<Animator>();
        reset();
    }

    // Update is called once per frame
    void Update()
    {

        switch(m_currentState)
        {
            case SoliderState.Spawn:
                UpdateSpawn();
            break;
            case SoliderState.ChaseBall:
            case SoliderState.HoldBall:
            case SoliderState.Freedom:
                UpdateRunOnScreen();
            break;

            case SoliderState.Inactive:
                UpdateInactive();
            break;

        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("On Trigger Enter: " + other.tag);
        if(other.tag == "Ball")
        {
            if(m_currentState == SoliderState.ChaseBall)
            {
                setStateHoldBall();

                GameController.Instance.setAttackerHoldBall(this.gameObject);
            }
        }
        else if(other.tag == "Goal")
        {
            if(m_currentState == SoliderState.Freedom)
            {
                Destroy(this.gameObject);
            }
            else if(m_currentState == SoliderState.HoldBall)
            {
                if(GameController.Instance.isPlayerAttacker())
                    GameController.Instance.PlayerWin();
                else
                    GameController.Instance.PlayerLose();
            }
        }
    }

    void UpdateSpawn()
    {
        if(m_inactive_time > 0)
            m_inactive_time -= Time.deltaTime;
        else
        {
            if(GameController.Instance.getAttackerHoldBall() == null)
                setStateChaseBall();
            else
                setStateFreedom();
        }
    }
    void UpdateRunOnScreen()
    {
        if(GameController.Instance.getAttackerHoldBall() != null)
        {
            if(m_currentState == SoliderState.ChaseBall)
            {
                setStateFreedom();
            }
        }
        Vector3 newPos = Vector3.MoveTowards(this.transform.position, getTarget().transform.position, Time.deltaTime * speed);
        newPos.y = this.transform.position.y;
        this.transform.position = newPos;
        rotateToTarget(getTarget().transform.position);
    }

    void UpdateInactive()
    {
        if(m_inactive_time > 0)
        {
            m_inactive_time -= Time.deltaTime;
        }
        else
        {
            if(GameController.Instance.getAttackerHoldBall() == null)
                setStateChaseBall();
            else 
                setStateFreedom();
        }
    }

    public void setStateSpawn()
    {
        Arrow.SetActive(false);
        BallHolderHighlight.SetActive(false);
        m_inactive_time = GameController.Instance.SpawnTimeAttacker;
        switchState(SoliderState.Spawn);
        meshRenderer.material.color = Color.gray;
        anim.SetTrigger("Spaw");
    }

    public void setStateChaseBall()
    {
        Arrow.SetActive(true);
        speed = GameController.Instance.NormalSpeedAttacker;
        meshRenderer.material.color = getActiveColor();
        switchState(SoliderState.ChaseBall);
        setTarget(GameController.Instance.Ball.gameObject);
    }
    public void setStateHoldBall()
    {
        GameController.Instance.Ball.GetComponent<BallController>().setTarget(Vector3.zero);
        GameController.Instance.Ball.transform.parent = transform;
        GameController.Instance.Ball.SetActive(false);

        BallHolderHighlight.SetActive(true);
        speed = GameController.Instance.CarryingSpeedAttacker;
        switchState(SoliderState.HoldBall);
        setTarget(Goal);
    }

    public void setStateFreedom()
    {
        Arrow.SetActive(true);
        meshRenderer.material.color = getActiveColor();
        GameObject tempObj = new GameObject();
        Vector3 pos = Goal.transform.position;
        pos.x = transform.position.x;
        tempObj.transform.position = pos;
        setTarget(tempObj);
        switchState(SoliderState.Freedom);
    }

    public void setStateInactive()
    {
        anim.SetTrigger("Hit");
        Arrow.SetActive(false);
        PassBallToOtherAttacker();
        GameController.Instance.SetAllFreeAttackerChaseBall();
        //Remove Ball Holder
        GameController.Instance.setAttackerHoldBall(null);

        BallHolderHighlight.SetActive(false);
        m_inactive_time = GameController.Instance.ReactiveTimeAttacker;
        meshRenderer.material.color = Color.gray;
        switchState(SoliderState.Inactive);
    }

    public void reset()
    {
        speed = GameController.Instance.NormalSpeedAttacker;
        setStateSpawn();
    }

    public void setGoal(GameObject goal)
    {
        Goal = goal;
    }

    void PassBallToOtherAttacker()
    {
        GameObject nearestAttacker = GameController.Instance.FindNearestAttacker();
        if(nearestAttacker != null)
        {
            GameController.Instance.Ball.SetActive(true);
            GameController.Instance.Ball.GetComponent<BallController>().setTarget(nearestAttacker.transform.position);
            GameController.Instance.Ball.transform.parent = null;
        }
        else
        {
            if(GameController.Instance.isPlayerAttacker())
                GameController.Instance.PlayerLose();
            else
                GameController.Instance.PlayerWin();
        }
    }

    public bool isFreedomState()
    {
        return m_currentState == SoliderState.Freedom;
    }

    public Color getActiveColor()
    {
        if(GameController.Instance.PlayerRole == GameController.Role.Attacker)
        {
            return Color.cyan;
        }
        else
        {
            return Color.red;
        }
    }
}
