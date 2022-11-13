using System;
using System.Collections.Generic;
using UnityEngine;
public class Controller : MonoBehaviour
{
    private static Controller m_instance; // m_ - означает что переменная це private member (приватный член классу)
    public static Controller Instance
    {
        get
        {
            if (m_instance == null)
            {

                var controller =
                Instantiate(Resources.Load("Prefabs/Controller")) as GameObject;
                Debug.Log(controller);
                m_instance = controller.GetComponent<Controller>();
            }
            return m_instance;
        }
    }
    [SerializeField] private Score m_score;
    public Score Score { get { return m_score; } set { m_score = value; } }

    [SerializeField] private LevelParameters m_level;

    // [SerializeField] private int m_fieldSize;

    // [SerializeField] private int m_emptySquares;

    // [SerializeField] private int m_tokenTypes;

    [SerializeField] private Color[] m_tokenColors;

    [SerializeField] private Audio m_audio = new Audio();

    public Audio Audio { get { return m_audio; } set { m_audio = value; } }

    private Field m_field;

    private int m_currentLevel;
    public int CurrentLevel { set { m_currentLevel = value; } get { return m_currentLevel; } }

    public LevelParameters Level { get { return m_level; } set { m_level = value; } }
    public Field Field { get { return m_field; } set { m_field = value; } }
    public int FieldSize { get { return m_level.FieldSize; } }
    public int TokenTypes { get { return m_level.TokenTypes; } }
    public Color[] TokenColors { get { return m_tokenColors; } set { m_tokenColors = value; } }

    private List<List<Token>> m_tokensByTypes; // масив масивів для зберігання фішок, відсортованих за типами
    public List<List<Token>> TokensByTypes { get { return m_tokensByTypes; } set { m_tokensByTypes = value; } }
    private Color[] MakeColors(int count)
    {
        //ініціалізація масиву
        Color[] result = new Color[count];
        //зсув кольору - крок значення
        float colorStep = 1f / (count + 1);

        //задаємо вихідний колір
        float hue = 0f;

        float saturation = 0.5f;

        float value = 1f;

        //Задаємо значення кольору зі зміщенням для кожного елемента масиву
        for (int i = 0; i < count; i++)
        {
            //зміщення кольору:
            float newHue = hue + (colorStep * i);

            result[i] = Color.HSVToRGB(newHue, saturation, value);
        }
        return result;
    }

    // Метод, який визначає, чи знаходяться дві фішки поряд.
    private bool IsTokensNear(Token first, Token second)
    {
        if ((int)first.transform.position.x == (int)second.transform.position.x + 1 ||
        (int)first.transform.position.x == (int)second.transform.position.x - 1)
        {
            if ((int)first.transform.position.y == (int)second.transform.position.y)
            {
                return true;
            }
        }

        if ((int)first.transform.position.y == (int)second.transform.position.y + 1 ||
        (int)first.transform.position.y == (int)second.transform.position.y - 1)

        {
            if ((int)first.transform.position.x == (int)second.transform.position.x)
            {
                return true;
            }
        }
        return false;
    }

    // Метод, за допомогою якого перебираємо всі групи і перевіряємо, чи з'єднані всі фішки в цій групі
    // він повинен повертати булеве значення
    // і приймати один аргумент - масив фішок одного типу
    // Додаємо перевірку, на випадок, якщо якогось із типів фішок взагалі немає на полі 
    // при випадковій генерації таке може бути
    private bool IsTokensConnected(List<Token> tokens)
    {
        if (tokens.Count == 0)
        {
            return true;
        }

        //Массив для тих фішок, які знаходяться поруч один з одним
        List<Token> connectedTokens = new List<Token>(); /// непонянтно как заполняется массив 
        connectedTokens.Add(tokens[0]); //фішка, яка буде точкою відліку
        bool moved = true; //означає, чи була пересунута в масив connectedTokens якась нова фішка

        while (moved)
        {
            moved = false;

            for (int i = 0; i < connectedTokens.Count; i++)
            {
                for (int j = 0; j < tokens.Count; j++)
                {
                    if (IsTokensNear(tokens[j], connectedTokens[i]) == true)
                    {                                                     // тобто ми порівнюємо шось з чимось за допомогою Contains та повертаемо true/fals
                        if (connectedTokens.Contains(tokens[j]) == false) // Проверяем содержится ли элемент переданный в метод в коллекции
                        {                                                 // Contains повертае усі єлементи массива 
                            /*До того часу, поки ми будемо додавати в масив
                              connectedTokens хоча б одну фішку, цикл буде повторюватися. 
                              Як тільки пройде хоча б одна ітерація циклу,
                              в ході якої жодна фішка не буде перенесена в масив connectedTokens,
                              програма виходить з циклу*/
                            connectedTokens.Add(tokens[j]);
                            moved = true;
                        }
                    }
                }
            }
        }
        if (tokens.Count == connectedTokens.Count)
        {
            return true;
        }
        return false;
    }
    // перевіряе усі группи фішок 
    public bool IsAllTokensConnected()
    {
        //TODO:
        //Оптимізувати: перевіряти лише той тип, фішка якого була переміщена

        //Перебираємо типи
        for (var i = 0; i < TokensByTypes.Count; i++)
        {
            //Чи з'єднані всі фішки поточного типу
            if (IsTokensConnected(TokensByTypes[i]) == false)
            {
                return false;
            }
        }
        return true;
    }

    // Після кожного ходу гравця перевіряемо, чи виконано умову перемоги
    // Цей метод за допомогою функції IsAllTokensConnected() перевіряє умову
    // перемоги і виводить відповідне повідомлення в консоль.
    public void TurnDone()
    {
        Audio.PlaySound("Drop");

        if (IsAllTokensConnected())
        {
            Debug.Log("Win!");

            Score.AddLevelBonus();

            m_currentLevel++;

            Destroy(m_field.gameObject);

            //InitializeLevel();

            Hud.Instance.CountScore(m_level.Turns);

            Audio.PlaySound("Victory");
        }
        else
        {
            Debug.Log("Continue...");

            if (m_level.Turns > 0)
            {
                m_level.Turns--;
            }
        }
    }



    public void InitializeLevel()
    {
        m_level = new LevelParameters(m_currentLevel);

        TokenColors = MakeColors(m_level.TokenTypes);

        //код який проініціалізує масиви так, щоб у них можна було зберігати фішки
        TokensByTypes = new List<List<Token>>();

        for (int i = 0; i < m_level.TokenTypes; i++)
        {
            TokensByTypes.Add(new List<Token>());
        }

        //створює ігрове поле 
        m_field = Field.Create(m_level.FieldSize, m_level.FreeSpace);
    }
    private void Awake()
    {
        InitializeLevel();

        MakeColors(m_level.FieldSize);

        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (m_instance != this) Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        Audio.SourceMusic = gameObject.AddComponent<AudioSource>();
        Audio.SourceRandomPitchSFX = gameObject.AddComponent<AudioSource>();
        Audio.SourceSFX = gameObject.AddComponent<AudioSource>();
    }

    //скидае прогрес гри
    public void Reset()
    {
        CurrentLevel = 1;

        Score.CurrentScore = 0;

        Destroy(m_field.gameObject);

        InitializeLevel();
    }

    private void Start()
    {
        Audio.PlayMusic(true);
    }
}