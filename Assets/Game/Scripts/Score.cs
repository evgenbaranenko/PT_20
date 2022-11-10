using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Score
{
    [SerializeField] private int m_currentScore;

    [SerializeField] private int m_levelScoreBonus;

    [SerializeField] private int m_turnScoreBonus;

    public int CurrentScore
    {
        get { return m_currentScore; }
        set { m_currentScore = value; Hud.Instance.UpdateScoreValue(m_currentScore); }
    }

    // додаватиме з поточного значення очок бонус за проходження рівня

    public void AddLevelBonus()
    {
        CurrentScore += m_levelScoreBonus;
    }

    // додавання бонусу за кожен збережений хід
    public void AddTurnBonus()
    {
        CurrentScore += m_turnScoreBonus; Debug.Log("AddTurnBonus");
    }
}