using UnityEngine;
public class Controller : MonoBehaviour
{

    private static Controller m_instance;

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

