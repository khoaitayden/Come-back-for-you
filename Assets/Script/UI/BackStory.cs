using UnityEngine;
using UnityEngine.UIElements;
public class BackStory : MonoBehaviour
{
     private UIDocument backstoryDocument;
    [SerializeField] GameObject mainMenuPanel;
    private Button backButton;
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
        gameObject.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
