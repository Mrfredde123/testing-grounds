using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class camera : MonoBehaviour
{
    public Transform cam_copy;
    public int camera_mode = 0;
    float smoothness = 5f;
    Vector3 izo_pos = new Vector3(4.53000021f, 5.05000019f, -4.01999998f);
    Vector3 side_pos = new Vector3(0, 0, -5);
    Vector3 current_pos;
    Quaternion izo_rot = new Quaternion(0.239117652f, -0.369643807f, 0.0990457684f, 0.892399132f);
    Quaternion side_rot = Quaternion.identity;

    Quaternion current_rot;
    void Start()
    {
        current_pos = izo_pos;
        current_rot = izo_rot;
    }


    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                if (camera_mode == 1)
                {
                    camera_mode = 0;
                    current_pos = izo_pos;
                    current_rot=izo_rot;
                }
                else if (camera_mode == 0)
                {
                    camera_mode = 1;
                    current_pos = side_pos;
                    current_rot = side_rot;
                }

            }




            transform.rotation = Quaternion.Lerp(transform.rotation, current_rot, smoothness * Time.deltaTime);
            Vector3 targetPosition = cam_copy.position + current_pos;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);
        }
    }
}
