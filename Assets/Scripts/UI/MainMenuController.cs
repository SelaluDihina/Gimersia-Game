
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuButtons;
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Display")]
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "MainScene";

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";

    private void Awake()
    {
        // Tampilan awal
        menuButtons.SetActive(true);
        settingsPanel.SetActive(false);

        // Menghubungkan tombol
        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);
        backButton.onClick.AddListener(CloseSettings);

        // Mengambil volume yang pernah disimpan
        musicVolumeSlider.value =
            PlayerPrefs.GetFloat(MUSIC_KEY, 1f);

        sfxVolumeSlider.value =
            PlayerPrefs.GetFloat(SFX_KEY, 1f);

        // Menghubungkan slider dengan fungsi
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // Mengambil pengaturan fullscreen
        bool fullscreen =
            PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        fullscreenToggle.isOn = fullscreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void Start()
    {
        SetMusicVolume(musicVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);

        Screen.fullScreen = fullscreenToggle.isOn;
    }

    private void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OpenSettings()
    {
        menuButtons.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
        menuButtons.SetActive(true);
    }

    private void ExitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    private void SetMusicVolume(float value)
    {
        SetMixerVolume("MusicVolume", value);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    private void SetSFXVolume(float value)
    {
        SetMixerVolume("SFXVolume", value);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        if (audioMixer == null)
            return;

        float decibels = value <= 0.0001f
            ? -80f
            : Mathf.Log10(value) * 20f;

        if (!audioMixer.SetFloat(parameter, decibels))
        {
            Debug.LogWarning(
                "Audio Mixer parameter belum ditemukan: " + parameter
            );
        }
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt(
            FULLSCREEN_KEY,
            isFullscreen ? 1 : 0
        );
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
