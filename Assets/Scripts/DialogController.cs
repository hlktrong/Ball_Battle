using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text Title;
    public Text TextMessage;
    public Text ButtonText;
    public Button Button;
    UnityAction call;
    public static DialogController Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowDialog(string title, string message, string buttonText, UnityAction call)
    {
        Debug.Log("Call Show Dialog");
        this.Title.text = title;
        RectTransform rect = GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0, 0,0);
        TextMessage.text = message;

        ButtonText.text = buttonText;
        Button.onClick.AddListener(call);
        Time.timeScale = 0f;
    }
    public void CloseDialog()
    {
        Debug.Log("Call close Dialog");
        Time.timeScale = 1f;
        RectTransform rect = GetComponent<RectTransform>();
        rect.localPosition = new Vector3(-5000, 0,0);
        Button.onClick.RemoveAllListeners();
        this.gameObject.SetActive(false);
    }
}
