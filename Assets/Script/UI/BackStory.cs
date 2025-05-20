using UnityEngine;
using UnityEngine.UIElements;

public class BackStory : MonoBehaviour
{
    private UIDocument backstoryDocument;
    [SerializeField] private GameObject mainMenuPanel;
    private Button backButton;
    [SerializeField] private AudioSource buttonSoundSource; // Reference to the external AudioSource

    private void OnEnable()
    {
        if (backstoryDocument == null)
        {
            backstoryDocument = GetComponent<UIDocument>();
        }
        backButton = backstoryDocument.rootVisualElement.Q<Button>("BackButton");
        backButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClick());
    }

    private void OnDisable()
    {
        if (backButton != null)
        {
            backButton.UnregisterCallback<ClickEvent>(ev => OnBackButtonClick());
        }
    }

    private void OnBackButtonClick()
    {
        if (buttonSoundSource != null)
        {
            buttonSoundSource.Play();
        }
        gameObject.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}