using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Transform player_pos;
    private CharacterController characterController;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    // Переменные для гравитации
    public float gravity = -9.81f;
    private float verticalVelocity;

    // Переменные для настраиваемого прыжка
    public float jumpForce = 7f;
    public float maxJumpTime = 0.2f;
    private float jumpTimeCounter;
    private bool isJumping;

    public camera cam;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (cam.camera_mode == 0)
            {
                if (Keyboard.current.wKey.isPressed) input.y += 1;
                if (Keyboard.current.sKey.isPressed) input.y -= 1;
            }

            if (Keyboard.current.dKey.isPressed) input.x += 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
        }

        Vector3 move = new Vector3(input.x, 0, input.y);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        move *= speed;

        // Логика гравитации и прыжка
        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;
            isJumping = false;

            // НАЧАЛО ПРЫЖКА: нажали пробел на земле
            if (cam.camera_mode == 1 && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                isJumping = true;
                jumpTimeCounter = maxJumpTime;
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            // УДЕРЖАНИЕ ПРЫЖКА: держим пробел в воздухе
            if (cam.camera_mode == 1 && Keyboard.current.spaceKey.isPressed && isJumping)
            {
                if (jumpTimeCounter > 0)
                {
                    verticalVelocity = jumpForce;
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }

            // Если отпустили пробел раньше времени — прекращаем активный подъем
            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                isJumping = false;
            }

            // Гравитация работает всегда, когда мы не «взлетаем» активно за счет удержания
            if (!isJumping || jumpTimeCounter <= 0)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }

        // Записываем вертикальную скорость в общую переменную движения
        move.y = verticalVelocity;

        // Финальное движение (сохраняем результат столкновений)
        CollisionFlags collisionFlags = characterController.Move(move * Time.deltaTime);

        // ИСПРАВЛЕНИЕ ПРИЛИПАНИЯ К ПОТОЛКУ:
        // Проверяем, было ли столкновение сверху (головой) и летел ли персонаж при этом вверх
        if ((collisionFlags & CollisionFlags.Above) != 0 && verticalVelocity > 0)
        {
            verticalVelocity = 0f; // Гасим скорость подъема
            isJumping = false;     // Прерываем фазу удержания прыжка
        }
    }
}
