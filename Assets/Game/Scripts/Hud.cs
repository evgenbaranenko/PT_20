using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField] private Text m_scoreValue;

    [SerializeField] private Text m_turnsValue;

    private static Hud m_instance;
    public static Hud Instance { get { return m_instance; } }

    [SerializeField] private Slider m_musicSlider;

    [SerializeField] private Slider m_soundSlider;

    private void Awake()
    {
        m_instance = this;
    }
    public void UpdateOptions()
    {
        m_musicSlider.value = Controller.Instance.Audio.MusicVolume;

        m_soundSlider.value = Controller.Instance.Audio.SfxVolume;
    }

    public void UpdateTurnsValue(int value)
    {
        m_turnsValue.text = value.ToString();
    }
    public void UpdateScoreValue(int value)
    {
        m_scoreValue.text = value.ToString();
    }

    public void ShowWindow(CanvasGroup window)
    {
        window.alpha = 1f;

        window.blocksRaycasts = true;

        window.interactable = true;
    }
    public void HideWindow(CanvasGroup window)
    {
        window.alpha = 0f;

        window.blocksRaycasts = false;

        window.interactable = false;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Reset()
    {
        Controller.Instance.Reset();
    }
    //методи, які будуть задавати гучність звуків та музики:
    public void SetMusicVolume(float volume)
    {
        Controller.Instance.Audio.MusicVolume = volume;
    }
    public void SetSoundVolume(float volume)
    {
        Controller.Instance.Audio.SfxVolume = volume;
    }
}