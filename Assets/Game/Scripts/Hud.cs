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

    private void Awake()
    {
        m_instance = this;
    }
    public void UpdateTurnsValue(int value)
    {
        m_turnsValue.text = value.ToString();
    }

    public void UpdateScoreValue(int value)
    {
        m_scoreValue.text = value.ToString();
    }
}
