using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataStore
{
    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private const string SOUND_VOLUME_KEY = "SoundVolume";

    private const string SAVED_LEVEL_KEY = "Level";

    private const string SAVED_SCORE_KEY = "Score";

    private const float DEFAULT_VOLUME = 0.75f;

    static public void SaveOptions()
    {
        /*Перший рядок методу зберігає ключ типу float, з назвою, вказаною в константі
          MUSIC_VOLUME_KEY , і значенням, що дорівнює змінній MusicVolume нашого об'єкта Audio .*/
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, Controller.Instance.Audio.MusicVolume);

        /*Другий рядок робить те саме для музики.*/
        PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, Controller.Instance.Audio.SfxVolume);

        /*Третій рядок примусово здійснює запис PlayerPrefs , на той випадок, якщо раптом роботу
          програми буде аварійно завершено.*/
        PlayerPrefs.Save();
    }

    //Метод, який завантажуватиме налаштування:
    static public void LoadOptions()
    {
        Controller.Instance.Audio.MusicVolume =
        PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);

        Controller.Instance.Audio.SfxVolume =
        PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, DEFAULT_VOLUME);
    }

    // Методи для збереження та завантаження ігрового прогресу:

    static public void LoadGame()
    {
        Controller.Instance.CurrentLevel =
        PlayerPrefs.GetInt(SAVED_LEVEL_KEY, 1);
        Controller.Instance.Score.CurrentScore =
        PlayerPrefs.GetInt(SAVED_SCORE_KEY, 0);
    }

    static public void SaveGame()
    {
        PlayerPrefs.SetInt(SAVED_LEVEL_KEY,
        Controller.Instance.CurrentLevel);
        PlayerPrefs.SetInt(SAVED_SCORE_KEY,
        Controller.Instance.Score.CurrentScore);
        PlayerPrefs.Save();
    }
}

