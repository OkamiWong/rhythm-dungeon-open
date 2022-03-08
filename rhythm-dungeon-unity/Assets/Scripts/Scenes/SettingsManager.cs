using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    static public SettingsManager Instance;

    public Scrollbar music, effect;

    private void Start()
    {
        Instance = this;

        music.value = AudioManager.Instance.background.volume;
        effect.value = AudioManager.Instance.soundEffect.volume;
    }

    public void Update()
    {
        AudioManager.Instance.background.volume = music.value;
        AudioManager.Instance.soundEffect.volume = effect.value;
    }

    public void Back()
    {
        SceneManager.UnloadSceneAsync("Settings");
    }
}