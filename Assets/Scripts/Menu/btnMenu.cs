using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class btnMenu : MonoBehaviour
{
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button endButton;
    [SerializeField]
    private TextMeshProUGUI displayText;
    [SerializeField]
    private GameObject menuMain;
    [SerializeField]
    private GameObject menuLevel;

    // Start is called before the first frame update
    void Start()
    {
        HideScreen(menuLevel);
        DisplayScreen(menuMain);
        AddListener(); // Call the AddListener method in Start
    }

    // Update is called once per frame
    void Update()
    {

    }
    //--------------------------------------------------------------------------------------------------
    void HideScreen(GameObject go)
    {
        if (go != null)
        {
            go.SetActive(false);
        }
    }
    void DisplayScreen(GameObject go)
    {
        if (go != null)
        {
            go.SetActive(true);
        }
    }
    void AddListener()
    {
        startButton.onClick.AddListener(ClickStart);
        endButton.onClick.AddListener(ClickEnd);
    }

    void ClickStart()
    {
        HideScreen(menuMain);
        DisplayScreen(menuLevel);
    }

    void ClickEnd()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
