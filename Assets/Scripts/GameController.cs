using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    public int MatchPerGame;
    public int TimeLimit;
    public float EnergyBar;
    public float BallSpeed;

    [Space(10)]
    [Header("Attacker Settings")]
    public float EnergyRegenerationAttacker;
    public float EnergyCostAttacker;
    public float SpawnTimeAttacker;
    public float ReactiveTimeAttacker;
    public float NormalSpeedAttacker;
    public float CarryingSpeedAttacker;
    [Space(10)]
    [Header("Defender Settings")]
    public float EnergyRegenerationDefender;
    public float EnergyCostDefender;
    public float SpawnTimeDefender;
    public float ReactiveTimeDefender;
    public float NormalSpeedDefender;
    public float ReturnSpeedDefender;
    [Range(0, 100)]
    public float DetectionRangeDefender;

    [Space(10)]
    [Header("Game Objects")]
    public GameObject AttackerPrefab;
    public GameObject DefenderPrefab;

    public Text GameTimeText;
    public GameObject AttackerParrentNode;
    public GameObject DefenderParrentNode;
    public GameObject BallPrefab;
    public GameObject Ball;
    public GameObject GameplayUI;

    public GameObject dialog;

    private float m_current_Attacker_Energy;
    private float m_current_Defender_Energy;
    public static GameController Instance { get; private set; }

    private GameObject m_attacker_have_ball = null;

    public enum Role{Attacker,Defender};

    public enum MatchResult{WIN, LOSE, DRAW, NONE};
    private MatchResult m_currentGameResult = MatchResult.NONE ;
    public Role PlayerRole;
    public Role EnemyRole;
    private GameObject playerField;
    private GameObject enemyField;
    private int round = 0;

    List<MatchResult> m_MatchScore = new List<MatchResult>();
    void Awake()
    {
        Instance = this;
        
        
    }
    private static GameController m_instance;
    // Start is called before the first frame update
    private float m_timer = 0;
    
    void Start()
    {
        playerField = GameObject.Find("PlayerField");
        enemyField = GameObject.Find("EnemyField");
        resetGame();
        switchField();
        m_MatchScore.Clear();
    }
    void Update()
    {
        m_timer -= Time.deltaTime;
        UpdateEnergyBar();
        UpdateGameTime();
        UpdateTouch();

    }
    void UpdateEnergyBar()
    {
        if(m_current_Attacker_Energy < EnergyBar)
        {
            m_current_Attacker_Energy  += Time.deltaTime * EnergyRegenerationAttacker;
        }
        else 
        {
            m_current_Attacker_Energy = EnergyBar;
        }
        if(m_current_Defender_Energy < EnergyBar)
        {
            m_current_Defender_Energy += Time.deltaTime * EnergyRegenerationDefender;
        }
        else
        {
            m_current_Defender_Energy = EnergyBar;
        }
    }
    public float getCurrentAttackerEnergy()
    {
        return m_current_Attacker_Energy;
    }

    public float getCurrentDefenderEnergy()
    {
        return m_current_Defender_Energy;
    }
    public void resetGame()
    {
        m_MatchScore.Add(m_currentGameResult);
        m_timer = TimeLimit;
        m_current_Attacker_Energy = 0.0f;
        m_current_Defender_Energy = 0.0f;
        m_currentGameResult = MatchResult.NONE;
        setAttackerHoldBall(null);
        //delete attacker List.
        foreach(Transform t in AttackerParrentNode.transform)
        {
            Destroy(t.gameObject);
        }
        //delete defender List
        foreach(Transform t in DefenderParrentNode.transform)
        {
            Destroy(t.gameObject);
        }
        Destroy(Ball);
    }

    private void switchField()
    {
        if(round >= 5)
        {
            GameOver();
            return;
        }
        round++;
        Debug.Log(" round " + round);
        if(round % 2 == 0)
        {
            PlayerRole = Role.Defender;
            EnemyRole = Role.Attacker;
            playerField.layer = LayerMask.NameToLayer("DefenderField");
            enemyField.layer = LayerMask.NameToLayer("AttackerrField");
        }
        else 
        {
            PlayerRole = Role.Attacker;
            EnemyRole = Role.Defender;
            playerField.layer = LayerMask.NameToLayer("AttackerrField");
            enemyField.layer = LayerMask.NameToLayer("DefenderField");
        }
        ShowDialog("New Match",StartMatchText() , "OK",  delegate () { this.ClickStartGame(); });
    }

    string StartMatchText()
    {
        string text = "<size=60>Current round: " + "<color=blue>" + (round)+"</color></size>"+ 
                        "\n\n" +
                        "<color=fuchsia>Enemy: "+ EnemyRole + "</color>" +
                        "\n \n" +
                        "<color=cyan>Player: "+ PlayerRole + "</color>";
        return text;
    }

    void PutBallRadomInField()
    {
        GameObject Field = playerField;
        if(EnemyRole == Role.Attacker)
        {
            Field = enemyField;
        }

        // Debug.Log(" PutBallInRadom ");
        // Generate random position of Ball on field.
        BoxCollider enemyBoxCollider = Field.GetComponent<BoxCollider>();
        float X = enemyBoxCollider.size.x / 2;
        float Y = enemyBoxCollider.size.z /2;
        float randomX = Random.Range( -X, X);
        float randomZ = Random.Range( -Y, Y);
        Vector3 newPos = Field.transform.position;
        newPos.x += randomX;
        newPos.z += randomZ;
        Ball.transform.position = newPos;
    }

    void UpdateGameTime()
    {
        if(Time.timeScale == 0)
            return;
        GameTimeText.text = ((int) m_timer) + " s";
        if(m_timer <=0 )
        {
            m_currentGameResult = MatchResult.DRAW;
            ShowDialog("Draw", " Prepare for Next Match " , "OK",  delegate () { this.EndGame(); });
        }
    }

    public void setAttackerHoldBall(GameObject attacker)
    {
        //Remove current attacker out of List.
        if(attacker != null)
        {
            m_attacker_have_ball = attacker;
            m_attacker_have_ball.transform.parent = null;
        }
        else
        {
            if(m_attacker_have_ball != null)
            {
                m_attacker_have_ball.transform.parent = AttackerParrentNode.transform;
            }
            m_attacker_have_ball = null;
        }
    }
    public GameObject getAttackerHoldBall()
    {
        return m_attacker_have_ball;
    }
    
    void UpdateTouch()
    {
        if(Time.timeScale == 0f)
            return;
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask fieldMask = (1 << LayerMask.NameToLayer("AttackerrField") | 1 << LayerMask.NameToLayer("DefenderField"));
            if (Physics.Raycast( ray, out hit, 1000, fieldMask))
            {
                // Debug.Log("hit " + hit.collider.name);
                if( LayerMask.LayerToName(hit.collider.gameObject.layer) == "AttackerrField")
                {
                    // Debug.Log("Attacker field Hit " + hit.point);
                    generateAttacker(hit.point);
                
                }
                else if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "DefenderField")
                {
                    // Debug.Log("Defender Field Hit " + hit.point);
                    generateDefender(hit.point);
                }
            } 
            
        }
    }

    void generateAttacker(Vector3 position)
    {
        // Debug.Log("generateAttacker PlayerRole " + PlayerRole);
        if(IsAttackerEnoughEnergy())
        {
            GameObject new_Attacker =  Instantiate(AttackerPrefab);
            Vector3 pos= position;
            pos.y = 0.3f;
            new_Attacker.transform.position = pos;
            new_Attacker.transform.parent = AttackerParrentNode.transform;
            if(PlayerRole == Role.Attacker)
            {
                new_Attacker.GetComponent<AttackerController>().setGoal(GameObject.Find("UpperGoal"));
            }
            else if(PlayerRole == Role.Defender)
            {
                new_Attacker.GetComponent<AttackerController>().setGoal(GameObject.Find("UnderGoal"));
            }
        }
        else
        {
            Debug.Log("generateAttacker Not Enought Energy");
        }
    }

    void generateDefender(Vector3 position)
    {
        // Debug.Log("generateDefender PlayerRole ");
        if(IsDefenderEnoughEnergy())
        {
            GameObject new_Defender =  Instantiate(DefenderPrefab);
            Vector3 pos= position;
            pos.y = 0.3f;
            new_Defender.transform.position = pos;
            new_Defender.transform.parent = DefenderParrentNode.transform;
        }
        else
        {
            // Debug.Log("generateAgenerateDefenderttacker Not Enought Energy");
        }
    }

    public Role getPlayerRole()
    {
        return PlayerRole;
    }

    public Role getEnemyRole()
    {
        return EnemyRole;
    }

    public bool isPlayerAttacker()
    {
        return  PlayerRole == Role.Attacker;
    }

    public bool isEnemyAttacker()
    {
        return  EnemyRole == Role.Attacker;
    }

    public GameObject FindNearestAttacker()
    {
        GameObject found = null;
        float minDistance = 1000f;
        // Debug.Log("Child Cound= " + AttackerParrentNode.transform.childCount);
        
        int count = 0;
        foreach(Transform t in AttackerParrentNode.transform)
        {
            count ++;
            t.name = ""+ count;
            float dist = Vector3.Distance(getAttackerHoldBall().transform.position, t.position);
            // Debug.Log("i = " + count + " dist = " + dist +" minDistance " + minDistance);
            if(dist < minDistance)
            {
                found = t.gameObject;
                minDistance = dist;
            }
        }
        // Debug.Log("Attacker naem " + );
        return found;
    }

    public void SetAllFreeAttackerChaseBall()
    {
        foreach(Transform t in AttackerParrentNode.transform)
        {
            if(t.gameObject.GetComponent<AttackerController>().isFreedomState())
            {
                t.gameObject.GetComponent<AttackerController>().setStateChaseBall();
            }
        }
    }

    public void SetOthersDefenderReturnToHome()
    {
        foreach(Transform t in DefenderParrentNode.transform)
        {
            if(t.gameObject.GetComponent<DefenderController>().isChaseAttacker())
            {
                t.gameObject.GetComponent<DefenderController>().setStateRunBackToHome();
            }
        }
    }

    bool IsAttackerEnoughEnergy()
    {
        if(m_current_Attacker_Energy >= GameController.Instance.EnergyCostAttacker)
        {
            m_current_Attacker_Energy -= GameController.Instance.EnergyCostAttacker;
            return true;
        }
        else
            return false;
    }
    bool IsDefenderEnoughEnergy()
    {
        if(m_current_Defender_Energy >= GameController.Instance.EnergyCostDefender)
        {
            m_current_Defender_Energy -= GameController.Instance.EnergyCostDefender;
            return true;
        }
        else
            return false;
    }

    public void ClickStartGame()
    {
        Debug.Log("Click start game");
        //Put Ball on field
        Ball = Instantiate(BallPrefab);
        PutBallRadomInField();

        //Enable Gameplay UI
        GameplayUI.SetActive(true);
    }

    public void EndGame()
    {
        GameplayUI.SetActive(false);
        resetGame();
        switchField();
    }

    public void PlayerWin()
    {
         m_currentGameResult = MatchResult.WIN;
        ShowDialog("<color=green>Player WIN</color>", "Congratulation!\nBe prepare for Next Match " , "OK",  delegate () { this.EndGame(); });
    }
    public void PlayerLose()
    {
        m_currentGameResult = MatchResult.LOSE;
        ShowDialog("<color=red>Player LOSE</color>", "Good luck next time!" , "OK",  delegate () { this.EndGame(); });
    }

    public void GameOver()
    {
        int playerWin = 0;
        int playerLose = 0;
        foreach(MatchResult m in m_MatchScore)
        {
            if(m == MatchResult.WIN)
            {
                playerWin++;
            }
            else if(m == MatchResult.LOSE)
            {
                playerLose++;
            }
        }
        if(playerWin > playerLose)
        {
            ShowDialog("<color=green>Victory</color>", "Congratulation!\nYou are number 1! " , "OK",  delegate () { this.QuitGame(); });
        }
        else if(playerWin < playerLose)
        {
           ShowDialog("<color=red>GameOver</color>", "LOSERRRRRRRRRR! " , "OK",  delegate () { this.QuitGame(); });
        }
        else
        {
            ShowDialog("<color=yellow>Penalty</color>", "You have challange! " , "OK",  delegate () { this.Penalty(); });
        }
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Penalty()
    {
        ShowDialog("<color=yellow>So Sorry</color>", "You can't enter now." , "OK",  delegate () { this.QuitGame(); });
    }
     public void ShowDialog(string title, string message, string buttonText, UnityAction call)
    {
        dialog.SetActive(true);
        DialogController.Instance.ShowDialog(title, message , buttonText, call);
    }
}
