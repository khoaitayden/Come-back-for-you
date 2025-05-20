using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private VisualElement root;
    private Button backButton;
    private Button mainMenuButton;
    private Slider musicSlider;
    private Slider sfxSlider;
    private Label musicText;
    private Label sfxText;

    [SerializeField] private InputAction pauseAction;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource buttonSoundSource; // Reference to the external AudioSource

    private void Awake()
    {
        pauseAction.performed += ctx => TogglePause();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }

    void OnEnable()
    {
        pauseAction.Enable();
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        backButton = root.Q<Button>("BackButton");
        backButton.clicked += Resume;

        mainMenuButton = root.Q<Button>("MainButton");
        mainMenuButton.clicked += GoToMainScene;

        musicSlider = root.Q<Slider>("MusicSlider");
        sfxSlider = root.Q<Slider>("SFXSlider");
        musicText = root.Q<Label>("MusicText");
        sfxText = root.Q<Label>("SFXText");

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

        root.style.display = DisplayStyle.None;
    }

    public void Resume()
    {
        if (buttonSoundSource != null)
        {
            buttonSoundSource.Play();
        }
        Time.timeScale = 1f;
        root.style.display = DisplayStyle.None;
        isPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        root.style.display = DisplayStyle.Flex;
        isPaused = true;
    }

    private void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void GoToMainScene()
    {
        if (buttonSoundSource != null)
        {
            buttonSoundSource.Play();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}