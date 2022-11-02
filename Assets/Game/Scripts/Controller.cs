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
                Instantiate(Resources.Load("Game/Resurses/Prefabs/Controller")) as GameObject;

                m_instance = controller.GetComponent<Controller>();
            }
            return m_instance;
        }
    }

    [SerializeField] private int m_fieldSize;

    [SerializeField] private int m_emptySquares;

    [SerializeField] private int m_tokenTypes;

    [SerializeField] private Color[] m_tokenColors;

    public int FieldSize { get { return m_fieldSize; } set { m_fieldSize = value; } }
    public int TokenTypes { get { return m_tokenTypes; } set { m_tokenTypes = value; } }
    public Color[] TokenColors { get { return m_tokenColors; } set { m_tokenColors = value; } }

    private void Awake()
    {
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
    }
}
    





