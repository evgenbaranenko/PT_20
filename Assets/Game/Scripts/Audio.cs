using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Audio
{
    /// <summary>
    /// Відтворення звуку з масиву

    /// </summary>
    /// <param name="clipName">Ім'я звуку</param>
    public void PlaySound(string clipName)
    {
        SourceSFX.PlayOneShot(GetSound(clipName), SfxVolume);
    }

    /// <summary>
    /// Відтворення звуку з масиву з випадковою частотою
    /// </summary>
    /// <param name="clipName">Ім'я звуку</param>
    public void PlaySoundRandomPitch(string clipName)
    {
        SourceRandomPitchSFX.pitch = Random.Range(0.7f, 1.3f);
        SourceRandomPitchSFX.PlayOneShot(GetSound(clipName),
        SfxVolume);
    }
    /// <summary>
    /// Відтворення музика
    /// </summary>
    /// <param name="menu">для головного меню?</param>
    public void PlayMusic(bool menu)
    {
        if (menu)
            SourceMusic.clip = menuMusic;
        else
            SourceMusic.clip = gameMusic;

        SourceMusic.volume = MusicVolume;

        SourceMusic.loop = true;

        SourceMusic.Play();
    }


    /// <summary>
    /// Пошук звуку в масиві
    /// </summary>
    /// <param name="clipName">Ім'я звуку</param>
    /// <returns>Звук. Якщо звук не знайдено, повертається значення змінної default
    ///Clip</returns>
    private AudioClip GetSound(string clipName)
    {
        for (var i = 0; i < sounds.Length; i++)
            if (sounds[i].name == clipName) return sounds[i];

        Debug.LogError("Can not find clip " + clipName);

        return defaultClip;
    }

    #region Private_Variables

    //Посилання на джерело звуку для відтворення звуків
    private AudioSource sourceSFX;
    //Посилання на джерело звуку для відтворення музики private
    private AudioSource sourceMusic;
    //Посилання на джерело звуку для відтворення звуків
    //з випадковою частотою
    private AudioSource sourceRandomPitchSFX;

    //гучність музики
    private float musicVolume = 1f;
    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            musicVolume = value;

            SourceMusic.volume = musicVolume;

            DataStore.SaveOptions();
        }
    }

    //гучність звуків
    private float sfxVolume = 1f;

    //Масив звуків
    [SerializeField] private AudioClip[] sounds;

    //Звук за замовчуванням на випадок, якщо в масиві відсутній необхідний
    [SerializeField] private AudioClip defaultClip;

    //Музика для головного меню
    [SerializeField] private AudioClip menuMusic;

    //Музика для гри на рівнях

    [SerializeField] private AudioClip gameMusic;

    #endregion

    #region Public_Properties
    public AudioSource SourceSFX
    {
        get { return sourceSFX; }
        set { sourceSFX = value; }
    }

    public AudioSource SourceMusic
    {
        get { return sourceMusic; }
        set { sourceMusic = value; }
    }

    public AudioSource SourceRandomPitchSFX
    {
        get { return sourceRandomPitchSFX; }
        set { sourceRandomPitchSFX = value; }
    }


    public float SfxVolume
    {
        get { return sfxVolume; }
        set
        {

            sfxVolume = value;

            SourceSFX.volume = sfxVolume;

            SourceRandomPitchSFX.volume = sfxVolume;

            DataStore.SaveOptions();

        }
    }

    #endregion
}