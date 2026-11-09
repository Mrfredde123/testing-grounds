using UnityEngine;
using UnityEngine.UI;          // Чтобы скрипт знал, что такое компонент Image
using System.Collections;      // Чтобы работали корутины (анимация по времени)

public class UI_Sword : MonoBehaviour
{
    private RawImage swordImage;      // Ссылка на саму картинку
    private RectTransform rect;    // Ссылка на Transform UI-элемента (для движения)
    private Vector2 targetPos;     // Где картинка должна остановиться (финиш)

    public float duration = 0.5f;  // Сколько времени длится анимация (в секундах)
    public float dropHeight = 150f; // С какого расстояния сверху она падает
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 1. Получаем ссылки на компоненты, висящие на этом же объекте
        swordImage = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();

        // 2. Запоминаем финальную позицию (ту, которую ты выставил в редакторе Unity)
        targetPos = rect.anchoredPosition;

        // 3. Делаем картинку полностью невидимой при старте игры (Альфа = 0)
        swordImage.color = new Color(swordImage.color.r, swordImage.color.g, swordImage.color.b, 0f);
    }
    public void ShowSwordUI()
    {
        // Запускаем нашу анимацию (корутину)
        StartCoroutine(AnimateSword());
    }
    IEnumerator AnimateSword()
    {
        float timeElapsed = 0f;

        // Вычисляем стартовую позицию: берем финальную и поднимаем её вверх на dropHeight
        Vector2 startPos = targetPos + new Vector2(0f, dropHeight);

        // Ставим картинку в стартовую позицию (высоко)
        rect.anchoredPosition = startPos;

        // Крутимся, пока не пройдет время анимации
        while (timeElapsed < duration)
        {
            // Прибавляем время, прошедшее с прошлого кадра
            timeElapsed += Time.deltaTime;

            // Вычисляем прогресс от 0 (начало) до 1 (конец)
            float t = timeElapsed / duration;

            // 1. Двигаем вниз: Lerp плавно смешивает стартовую и финишную позицию
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            // 2. Проявляем: Lerp плавно меняет прозрачность от 0 до 1
            float alpha = Mathf.Lerp(0f, 1f, t);
            swordImage.color = new Color(swordImage.color.r, swordImage.color.g, swordImage.color.b, alpha);

            // Ждем следующий кадр, чтобы анимация не выполнилась за 1 миг
            yield return null;
        }

        // В самом конце ставим идеальные значения, чтобы ничего не сбилось
        rect.anchoredPosition = targetPos;
        swordImage.color = new Color(swordImage.color.r, swordImage.color.g, swordImage.color.b, 1f);
    }
}
