using UnityEngine;

public class TriggerExample : MonoBehaviour
{
    // Перетащите сюда объект "sword" из иерархии
    public GameObject swordObject;
    public GameObject shoot;
    public UI_Sword swordUI;

    void Update()
    {
            
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Игрок вошел в зону: " + other.name);
        if (swordUI != null) swordUI.ShowSwordUI();

        // Проверяем ТЕГ ТОГО ОБЪЕКТА, в который мы вошли.
        // Например, если вы создали зону с тегом "SwordZone"
        if (other.CompareTag("SwordZone"))
        {
            if (swordObject != null)
            {
                
                swordObject.SetActive(true); // Включаем меч
                Debug.Log("Меч получен!");

                // (Опционально) Уничтожаем зону, чтобы меч нельзя было взять дважды
                Destroy(other.gameObject); 

            }
        }
        

    }
}