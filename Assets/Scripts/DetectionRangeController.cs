using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRangeController : MonoBehaviour
{
    public Transform WestWall;
    public Transform EastWall;
   
    private bool attackerInRange = false;

    // float m_rangeDetect;
    // Start is called before the first frame update
    void Start()
    {
        WestWall = GameObject.Find("West").transform;
        EastWall = GameObject.Find("East").transform;
        float distance = Vector3.Distance(WestWall.transform.position, EastWall.transform.position);
        // Debug.Log(" Distance " + distance);

        float scaleFactor = distance * GameController.Instance.DetectionRangeDefender / 100;
        Vector3 newScale = new Vector3(scaleFactor * 2.5f, 0.0003f, scaleFactor * 2.5f);
        transform.localScale = newScale;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerStay(Collider other)
    {
        // Debug.Log("DetectionRangeController hit player: " + other.name);
        if(GameController.Instance.getAttackerHoldBall() != null && other.gameObject == GameController.Instance.getAttackerHoldBall().gameObject)
        {
            // Debug.Log("Attacker hold ball in detection range.");
            attackerInRange = true;
        }
    }
    public bool isAnyAttackerInRange()
    {
        return attackerInRange;
    }

    void OnDisable()
    {
        // Debug.Log("DetectionRangeController: script was disabled");
    }

    void OnEnable()
    {
        attackerInRange = false;
    }

    
}
