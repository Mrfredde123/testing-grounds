using UnityEngine;
using UnityEngine.InputSystem; // ВАЖНО: подключаем новую систему

public class Player : MonoBehaviour
{
    public Transform player_pos;
    private CharacterController characterController;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Считываем ввод через НОВУЮ систему
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
        }

        // 2. Создаем вектор движения
        Vector3 move = new Vector3(input.x, 0, input.y);

        // 3. Логика поворота
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 4. Движение
        characterController.Move(move * Time.deltaTime * speed);
    }
}