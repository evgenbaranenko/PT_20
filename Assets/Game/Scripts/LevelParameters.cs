using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelParameters
{
    [SerializeField] private int m_fieldSize;

    [SerializeField] private int m_freeSpace;

    [SerializeField] private int m_TokenTypes;

    [SerializeField] private int m_turns;
    public int FieldSize { get { return m_fieldSize; } }
    public int FreeSpace { get { return m_freeSpace; } }
    public int TokenTypes { get { return m_TokenTypes; } }
    public int Turns
    {
        get { return m_turns; }
        set { m_turns = value; Hud.Instance.UpdateTurnsValue(m_turns); Debug.Log(value); }
    }
    public LevelParameters(int currentLevel)
    {
        //Збільшується на 1 кожні 4 рівні
        int fieldIncreaseStep = currentLevel / 4;

        //Збільшується від 0 до 1 протягом 4-х рівнів, доки розмір поля не змінюється
        float subStep = (currentLevel / 4f) - fieldIncreaseStep;

        //Початковий розмір поля – 3х3

        //Розмір збільшується на 1 кожні 4 рівні
        m_fieldSize = 3 + fieldIncreaseStep;

        //розраховуємо вільний простір залежно від рівня складності
        m_freeSpace = (int)(m_fieldSize * (1f - subStep));

        if (m_freeSpace < 1)
        {
            //мінімальна кількість порожніх клітин
            m_freeSpace = 1;
        }

        //Початкова кількість кольорів - 2

        // Збільшується на 1 кожні 2 рівні, збільшення починається з 4-го рівня

        m_TokenTypes = 2 + (currentLevel / 3);

        if (m_TokenTypes > 10)
        {
            //максимальна кількість кольорів
            m_TokenTypes = 10;
        }

        //Кількість ходів, за які треба встигнути закінчити рівень,
        //щоб отримати бонус, залежить від інших параметрів:

        m_turns = (((m_fieldSize * m_fieldSize / 2) - m_freeSpace) * m_TokenTypes) + m_fieldSize;
    }
}


