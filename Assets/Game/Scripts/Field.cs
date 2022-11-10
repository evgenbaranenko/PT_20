using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public static Field Create(int size, int emptySquares)
    {
        //Центр поля у центрі сцени
        Vector3 fieldPosition = Vector3.zero;

        //Якщо число клітин парне (якщо залишок від поділу на 2 дорівнює нулю)
        if (size % 2 == 0)
        {
            fieldPosition = new Vector3(0.5f, 0.5f, 0.0f);
        }

        //екземпляр поля фишки 
        var field = Instantiate(Resources.Load("Prefabs/Field") as GameObject, fieldPosition, Quaternion.identity);


        //Встановлюємо масштаб поля
        Vector3 scale = Vector3.one * size;

        scale.z = 1;

        field.transform.localScale = scale;

        //Положення камери
        Vector3 cameraPosition = field.transform.position;

        cameraPosition.z = -10;

        Camera.main.transform.position = cameraPosition;

        //Розмір камери
        Camera.main.orthographicSize = (float)size * 0.7f;

        //Текстура сітки
        field.GetComponent<Renderer>().material.mainTextureScale = Vector2.one * size;

        //Створюємо фішки
        field.gameObject.GetComponent<Field>().CreateTokens(size, emptySquares);

        return field.gameObject.GetComponent<Field>();
    }

    //розставити на полі фішки
    private void CreateTokens(int size, int emptySquares)
    {
        //Положення першої фішки - лівий нижній кут:
        float offset = (size - 1f) / 2f;
        Vector3 startPosition = new Vector3(transform.position.x - offset, transform.position.y -
        offset, transform.position.z - 2);

        //оголошуємо подвійний цикл: зовнішній з
        //параметром i, перебирає стовпці поля, і вкладений з параметром j перебирає клітини в стовпці:
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //якщо всього фішок створено більше або стільки, скільки потрібно - не
                // cтворювати фішку, залишити порожнє місце
                if ((i * size) + j >= (size * size) - emptySquares)
                {
                    //Ведемо підрахунок порожніх місць
                    emptySquares--;
                }

                //Інакше (якщо фішок створено менше, ніж потрібно)
                else
                {
                    //Якщо більше не потрібні порожні клітини,
                    //АБО ймовірність створення нової фішки більше за нуль

                    if (emptySquares == 0 || Random.Range(0, size * size / emptySquares) > 0)
                    {
                        //Створюємо нову фішку
                        Token newToken =
                        Instantiate(Resources.Load("Prefabs/Token"),
                        new Vector3(startPosition.x + i, startPosition.y + j,
                        startPosition.z), Quaternion.identity) as Token;
                    }
                    //Інакше
                    else
                    {
                        //Ведемо підрахунок порожніх місць
                        emptySquares--;
                    }
                }
            }
        }
    }
}
