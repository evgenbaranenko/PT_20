using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    private static Hud m_instance;
    public static Hud Instance { get { return m_instance; } }

    [SerializeField] private Text[] m_scoreValue;

    [SerializeField] private Text m_turnsValue;

    [SerializeField] private Slider m_musicSlider;

    [SerializeField] private Slider m_soundSlider;

    [SerializeField] private CanvasGroup m_LevelCompletedWindow;

    #region Raycaster
    /* - Raycaster - це такий компонент, який перевіряє, чи потрапив промінь, запущений з -під
      пойнтера (пальця на сенсорному екрані, або курсора миші), в якийсь інтерактивний об'єкт .
      Є три рейкастери: Graphic Raycaster , Physics Raycaster та Physics2D Raycaster , які
      відповідно працюють з об'єктами канвасу, фізичними колайдерами (3D) та колайдерами 2D
      фізики. На початку розробки цієї гри ми додавали Physics2D Raycaster на камеру, щоб
      отримати можливість рухати фішки, забезпечені Physics2D колайдерами. На канвас
      автоматично додається Graphic Raycaster . Якщо його відключити, всі елементи канвасу
      перестануть реагувати на події інпуту , що нам зараз потрібно.*/
    #endregion
    private GraphicRaycaster m_raycaster;

    private void Awake()
    {
        m_instance = this;

        m_raycaster = gameObject.GetComponent<GraphicRaycaster>();
    }

    //Метод, який показує вікно LevelCompletedWindow
    public void CountScore(int to)
    {
        ShowWindow(m_LevelCompletedWindow);
        /*При запуску корутини можна в метод StartCoroutine передавати
          аргументи як рядок з ім'ям корутини, так і саму корутину.*/
        StartCoroutine(Count(to, 0.3f));
    }
    #region Corutina
    /* - Корутина - це метод, що повертає об'єкт типу IEnumerator,
      який може передавати управління до виконання якоїсь умови та
      продовжувати роботу після виконання умови. Передача керування здійснюється за
      допомогою ключових слів yield return <об'єкт-умова>.
       - Ця корутина запускає цикл, який повторюється стільки разів, скільки гравець заощадив ходів
      (число передається через аргумент to ). На початку кожної ітерації корутина очікує певний
      проміжок часу (визначається аргументом delay ), після чого викликає метод класу Controller ,
      що додає до очок гравця бонус за один хід (і зменшує кількість зекономлених ходів). */
    #endregion
    private IEnumerator Count(int to, float delay)
    {
        m_raycaster.enabled = false;

        for (int i = 1; i <= to; i++)
        {
            yield return new WaitForSeconds(delay);

            Controller.Instance.Score.AddTurnBonus();
        }

        m_raycaster.enabled = true;
    }

    // перехід на наступний рівень
    public void Next()
    {
        Controller.Instance.InitializeLevel();
    }
    public void UpdateOptions()
    {
        m_musicSlider.value = Controller.Instance.Audio.MusicVolume;

        m_soundSlider.value = Controller.Instance.Audio.SfxVolume;
    }

    public void UpdateTurnsValue(int value)
    {
        m_turnsValue.text = value.ToString(); Debug.Log(value);
    }


    public void UpdateScoreValue(int value)
    {
        for (int i = 0; i < m_scoreValue.Length; i++)
        {
            m_scoreValue[i].text = value.ToString();
        }

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
    private void Start()
    {
        //HideWindow();
    }
}