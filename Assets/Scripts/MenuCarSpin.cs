using UnityEngine;

public class MenuCarSpin : MonoBehaviour
{
    [SerializeField] float degreesPerSecond = 20f;

    void Update()
    {
        transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.World);
    }
}
