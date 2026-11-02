using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 startPos;
    float timeLeft;
    float duration;
    float magnitude;

    void Awake()
    {
        startPos = transform.localPosition;
    }

    public void Shake(float time, float strength)
    {
        duration = time;
        timeLeft = time;
        magnitude = strength;
    }

    void LateUpdate()
    {
        if (timeLeft <= 0f) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            transform.localPosition = startPos;
            return;
        }
        transform.localPosition = startPos + Random.insideUnitSphere * magnitude * (timeLeft / duration);
    }
}
