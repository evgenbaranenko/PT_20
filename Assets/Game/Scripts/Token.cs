using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Token : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera m_camera;

    private Vector3 m_pointerPositionBeforeDrag;

    private Vector3 m_positionBeforeDrag;

    private int[] m_dragSpace;

    private int m_tokenType;

    private AudioSource m_audioSource;

    // викликається тоді, коли користувач клікнув на об'єкт
    // і з натиснутою кнопкою потягнув курсор кубік

    // об'єкт PointerEventData містить багато корисної
    // інформації про те, що відбувається,
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetDragSpace();

        //Фіксуємо початкове положення фішки та пойнтера
        //(курсор чи палець)
        m_pointerPositionBeforeDrag = m_camera.ScreenToWorldPoint(eventData.position);
        m_positionBeforeDrag = transform.position;

        m_audioSource.Play();
    }

    #region OnDrag(Explanation)
    // У метод OnDrag(), який викликається щоразу, коли оновлюється положення пойнтера 
    // (кожен раз, коли користувач переміщає палець по екрану), додамо код, який рухатиме фішку за пальцем:
    #endregion
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 previousPosition = transform.position;

        Vector3 mouseWorldPosition = m_camera.ScreenToWorldPoint(eventData.position);

        //Загальне зміщення курсору (пальця) щодо точки, звідки почався дрег:
        Vector3 totalDrag = mouseWorldPosition - m_pointerPositionBeforeDrag;

        //Визначаємо, тягнемо фішку по горизонталі або по вертикалі:
        if (Mathf.Abs(totalDrag.x) > Mathf.Abs(totalDrag.y))
        {
            //Обмежуємо переміщення лише порожніми клітинами всередині поля
            float posX = Mathf.Clamp(mouseWorldPosition.x, m_positionBeforeDrag.x -
            m_dragSpace[1], m_positionBeforeDrag.x + m_dragSpace[0]);

            //переміщуємо фішку
            transform.position = new Vector3(posX, m_positionBeforeDrag.y,
            transform.position.z);
        }
        else
        {
            //Обмежуємо переміщення лише порожніми клітинами всередині поля
            float posY = Mathf.Clamp(mouseWorldPosition.y, m_positionBeforeDrag.y -
            m_dragSpace[3], m_positionBeforeDrag.y + m_dragSpace[2]);

            //переміщуємо фішку
            transform.position = new Vector3(m_positionBeforeDrag.x, posY,
            transform.position.z);
        }
        // Нагадаю, що Mathf.Clamp обмежує значення в заданих межах:
        // Mathf.Clamp(змінна, мінімальне значення, максимальне значення).Тепер ми можемо рухати
        // фішки полем, їх неможливо вивести за межі поля, і вони блокують один одного.

        float currentFrameTokenDrag = Vector3.Distance(previousPosition, transform.position);
        #region Note:
        /*Примітка: якби ми просто рухали фішку по порожньому полю, без обмежень,
          визначення зміщення можна було б використовувати значення об'єкта
          PointerEventData, що передається в цей метод інтерфейсу автоматично -
          delta. У цій змінній зберігається переміщення пойнтер в останньому кадрі. Або
          можна було б використовувати клас Input:

          Input.GetTouch(0).deltaPosition
          Ця змінна дає нам зміну позиції тача на пристроях із сенсорним введенням. Але оскільки
          у нас є обмежувачі руху фішки, ці методи нам не підходять.*/
        #endregion

        ///Код, який регулює звучання в залежності від швидкості переміщення:
        
        //Розраховуємо зміну частоти залежно від швидкості переміщення
        //в обмеженому діапазоні (Clamp)
        float clampedPitchDrag = Mathf.Clamp(currentFrameTokenDrag * 10, 0.9f, 1.05f);

        //задаємо зміну частоти із застосуванням інтерполяції для пом'якшення переходів
        m_audioSource.pitch = Mathf.Lerp(m_audioSource.pitch, clampedPitchDrag, 0.5f);

        //Розраховуємо зміну гучності залежно від швидкості переміщення
        //в обмеженому діапазоні (Clamp)
        float clampedVolumeDrag = Mathf.Clamp(currentFrameTokenDrag * 10, 0.2f, 1.2f);

        // застосовуємо інтерполяцію
        float interpolatedDrag = Mathf.Lerp(m_audioSource.volume, clampedVolumeDrag - 0.2f, 0.7f);

        //множимо на загальний рівень звуку, щоб регулятор гучності
        //впливав на цей звук
        m_audioSource.volume = interpolatedDrag * Controller.Instance.Audio.SfxVolume;
    }







    // метод, який вирівнюватиме фішку по сітці:
    private void AlignOnGrid()
    {
        Vector3 alignedPosition = transform.position;
        alignedPosition.x = Mathf.Round(transform.position.x);
        alignedPosition.y = Mathf.Round(transform.position.y);
        transform.position = alignedPosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        AlignOnGrid();

        Controller.Instance.TurnDone();

        m_audioSource.Stop();
    }

    #region GetDragSpace(Explanation)
    // Ми обмежуватимемо переміщення фішки, зберігаючи інформацію про те, скільки клітин від
    // неї вільних у кожному з чотирьох напрямків.Інформація зберігатиметься в цілочисловому
    // масиві з чотирьох елементів: [0] – праворуч, [1] – зліва, [2] – зверху, [3] – знизу.Іншими
    // словами, 0 і 2 – позитивні значення X та Y, 1 та 3 – негативні значення.
    // метод, який заповнюватиме масив private int[] m_dragSpace залежно від положення фішок на полі
    // Тут ми ініціалізуємо масив, після чого знаходимо половину вільного простору навколо фішки
    // по одній осі за умови відсутності інших фішок.Тобто ми беремо ширину поля, віднімаємо одну
    // клітинку (саму фішку), а решту ділимо навпіл.
    // Таким чином, з одного боку фішки не може бути більше вільних клітин, ніж половина field х 2.
    #endregion
    private void GetDragSpace()
    {
        #region перевірка  гравого поля з парною/непарною кількостью клітин 

        int OddEven = 1; // парне поле 

        if (Controller.Instance.Level.FieldSize % 2 != 0)
        {
            OddEven = 0; // непарне поле 
        }
        #endregion

        m_dragSpace = new int[] { 0, 0, 0, 0 };
        int halfField = (Controller.Instance.FieldSize - 1) / 2;

        // код, який знайде число вільних клітин
        // праворуч від фішки (перше значення масиву)
        // для розрахунку кількості вільних клітин зправа від фішки
        m_dragSpace[0] = CheckSpace(Vector2.right);

        if (m_dragSpace[0] == -1)
        {
            m_dragSpace[0] = halfField - (int)transform.position.x + OddEven;
        }

        // для розрахунку кількості вільних клітин зліва від фішки
        m_dragSpace[1] = CheckSpace(Vector2.left);

        if (m_dragSpace[1] == -1)
        {
            m_dragSpace[1] = halfField + (int)transform.position.x;
        }

        // код для координати Y
        m_dragSpace[2] = CheckSpace(Vector2.up);

        if (m_dragSpace[2] == -1)
        {
            m_dragSpace[2] = halfField - (int)transform.position.y + OddEven;
        }

        m_dragSpace[3] = CheckSpace(Vector2.down);

        if (m_dragSpace[3] == -1)
        {
            m_dragSpace[3] = halfField + (int)transform.position.y;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        AlignOnGrid();

        m_tokenType = UnityEngine.Random.Range(0, Controller.Instance.TokenTypes);

        #region myMaterial
        // Тут ми знаходимо на об'єкті рендерер, через рендерер отримуємо доступ до матеріалу
        // об'єкта, і задаємо колір у полі "_Color" матеріалу рівний одному з масиву кольорів контролера.

        Material myMaterial = gameObject.GetComponent<Renderer>().material;
        myMaterial.SetColor("_Color", Controller.Instance.TokenColors[m_tokenType]);
        #endregion

        //Для того, щоб фішки при створенні додавали себе в масив
        Controller.Instance.TokensByTypes[m_tokenType].Add(this);

        m_audioSource = gameObject.GetComponent<AudioSource>();

        //Щоб фішки при знищенні поля також знищувалися,
        //вони повинні бути дочірніми об'єктами.
        //рядок “удочеріння” фішок
        transform.SetParent(Controller.Instance.Field.transform); /// тут непонятно отчего "наследуется" первый transform
    }

    #region CheckSpace(Explanation)
    // Цей метод запускає промінь із центру клітини у заданому напрямку 
    // (напрямок отримуємо у вигляді аргументу).
    // Нам потрібен саме RaycastAll, оскільки звичайний Raycast завжди
    // повертатиме у вигляді результату колайдера самої фішки. Ми перебираємо всі зустріті
    // променем колайдери, доки не знайдемо перший, що не належить самій фішці. Якщо такий
    // знайдений, метод повертає округлене в меншу сторону(щоб прибрати підлогу клітини від
    // центру фішки до краю) значення відстані до колайдера.
    // Тобто ми стріляємо промінь, наприклад, праворуч. Першим промінь зустрічає колайдер самої
    // фішки, але він відсівається умовним оператором. Якщо промінь зустрічає будь-який наступний
    // колайдер, наприклад, через одну клітинку від фішки, що тестується, то відстань від центру
    // фішки до цього колайдера буде дорівнює 1,5. Округлюємо в меншу сторону(Mathf.Floor),
    // виходить 1 – одна клітина до наступної, зайнятої.
    // Якщо промінь не зустрів більше перешкод, метод повертає -1.
    #endregion
    private int CheckSpace(Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject != gameObject)
            {
                return Mathf.FloorToInt(hits[i].distance);
            }
        }
        return -1;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
