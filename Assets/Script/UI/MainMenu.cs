using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    private UIDocument mainmenuDocument;
    [SerializeField] private GameObject storyPanel;
    private Button startButton;
    private Button storyButton;
    private Slider musicSlider;
    private Slider sfxSlider;
    private Label musicText;
    private Label sfxText;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource buttonSoundSource;

    void OnEnable()
    {
        if (mainmenuDocument == null)
        {
            mainmenuDocument = GetComponent<UIDocument>();
        }
        var root = mainmenuDocument.rootVisualElement;

        startButton = root.Q<Button>("StartButton");
        storyButton = root.Q<Button>("StoryButton");
        musicSlider = root.Q<Slider>("MusicSlider");
        sfxSlider = root.Q<Slider>("SFXSlider");
        musicText = root.Q<Label>("MusicText");
        sfxText = root.Q<Label>("SFXText");

        startButton.RegisterCallback<ClickEvent>(ev => OnStartButtonClick());
        storyButton.RegisterCallback<ClickEvent>(ev => OnStoryButtonClick());

        float musicVolume, sfxVolume;
        audioMixer.GetFloat("MusicVolume", out musicVolume);
        audioMixer.GetFloat("SFXVolume", out sfxVolume);

        musicSlider.value = Mathf.Pow(10, musicVolume / 20);
        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20);

        musicText.text = "Music: " + Mathf.RoundToInt(Mathf.Clamp(musicSlider.value * 100, 0, 100)) + "%";
        sfxText.text = "SFX: " + Mathf.RoundToInt(Mathf.Clamp(sfxSlider.value * 100, 0, 100)) + "%";

        musicSlider.RegisterValueChangedCallback(evt =>
        {
            float volume = evt.newValue;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
            musicText.text = "Music: " + Mathf.RoundToInt(Mathf.Clamp(volume * 100, 0, 100)) + "%";
        });

        sfxSlider.RegisterValueChangedCallback(evt =>
        {
            float volume = evt.newValue;
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
            sfxText.text = "SFX: " + Mathf.RoundToInt(Mathf.Clamp(volume * 100, 0, 100)) + "%";
        });
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
        if (buttonSoundSource != null)
        {
            buttonSoundSource.Play();
        }
        SceneManager.LoadScene("MainScene");
    }

    private void OnStoryButtonClick()
    {
        if (buttonSoundSource != null)
        {
            buttonSoundSource.Play();
        }
        gameObject.SetActive(false);
        storyPanel.SetActive(true);
    }
}