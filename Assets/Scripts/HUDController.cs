using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Image FillBar;
    public Image FullBar;

    public Text Info;
    public enum HUDType{Enemy,Player};
    public HUDType TypeOfHUD = HUDType.Enemy;
    // Start is called before the first frame update
    void Start()
    {
        // resetBar();
    }

    void resetBar()
    {
        if(GameController.Instance == null )
            return;
        FillBar.fillAmount = 0.0f;
        FullBar.fillAmount = 0.0f;
        
        if(TypeOfHUD == HUDType.Player)
        {
            // Debug.Log(" HUDType.Player " + GameController.Instance.getPlayerRole().ToString());
            Info.text = TypeOfHUD.ToString() + " (" + GameController.Instance.getPlayerRole().ToString() +")";
        }
        else if(TypeOfHUD == HUDType.Enemy)
        {
            // Debug.Log(" HUDType.Enemy " + GameController.Instance.getEnemyRole().ToString());
            Info.text = TypeOfHUD.ToString() + " (" + GameController.Instance.getEnemyRole().ToString() +")";
        }
    }

    // Update is called once per frame
    void Update()
    {
        float energy = GameController.Instance.getCurrentDefenderEnergy();
        if(TypeOfHUD == HUDType.Enemy && GameController.Instance.isEnemyAttacker() || TypeOfHUD == HUDType.Player && GameController.Instance.isPlayerAttacker())
        {
            energy = GameController.Instance.getCurrentAttackerEnergy();
        }
        FillBar.fillAmount = energy / GameController.Instance.EnergyBar;
        if(FillBar.fillAmount >= FullBar.fillAmount + 1 / GameController.Instance.EnergyBar - 0.001f)
        {
            FullBar.fillAmount = FillBar.fillAmount;
            // barCount = (int) (energy / 6.0f + 1);
        }
        else if(FullBar.fillAmount > FillBar.fillAmount)
        {
            FullBar.fillAmount = (int) ( FillBar.fillAmount / (1 / GameController.Instance.EnergyBar)) / GameController.Instance.EnergyBar;
        }

    }
    void OnEnable()
    {
        resetBar();
    }
}
