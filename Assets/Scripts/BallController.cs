using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // Start is called before the first frame update
    float speed;
    Vector3 m_target = Vector3.zero;

    void Start()
    {
        speed = GameController.Instance.BallSpeed;
        //Generate random on field.
    }

    // Update is called once per frame
    void Update()
    {
        if(m_target != Vector3.zero)
        {
            Vector3 newPos = Vector3.MoveTowards(this.transform.position, m_target, Time.deltaTime * speed);
            newPos.y = this.transform.position.y;
            this.transform.position = newPos;
        }
    }

    public void setTarget(Vector3 target)
    {
        // Debug.Log(" Ball set target ");
        m_target = target;
    }

}
