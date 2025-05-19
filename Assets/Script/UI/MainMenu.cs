using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    private UIDocument mainmenuDocument;
    [SerializeField] GameObject storyPanel;
    private Button startButton;
    private Button storyButton;
    void OnEnable()
    {
        if (mainmenuDocument == null)
        {
            mainmenuDocument = GetComponent<UIDocument>();
        }
        startButton = mainmenuDocument.rootVisualElement.Q<Button>("StartButton");
        storyButton = mainmenuDocument.rootVisualElement.Q<Button>("StoryButton");

        startButton.RegisterCallback<ClickEvent>(ev => OnStartButtonClick());
        storyButton.RegisterCallback<ClickEvent>(ev => OnStoryButtonClick());
    }
    void OnDisable()
    {
        if (startButton != null)
        {
            startButton.UnregisterCallback<ClickEvent>(ev => OnStartButtonClick());
        }
        if (storyButton != null)
        {
            storyButton.UnregisterCallback<ClickEvent>(ev => OnStoryButtonClick());
        }
    }
    private void OnStartButtonClick()
    {
        SceneManager.LoadScene("MainScene");
    }
    private void OnStoryButtonClick()
    {
        gameObject.SetActive(false);
        storyPanel.SetActive(true);
    }
}
