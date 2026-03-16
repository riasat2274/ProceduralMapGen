using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Button generateButton;
    public WFCBuilder builder;
    void Start()
    {
        builder.onGenerationComplete.AddListener(EnableButton);
        generateButton.onClick.AddListener(() => { DisableButton(); builder.GenerateNew(); });
    }
    public void EnableButton()
    {
        generateButton.interactable = true;
    }
    void DisableButton()
    {
        generateButton.interactable = false;
    }

}
