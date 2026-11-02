using UnityEngine;

public class MovingCar : Obstacle
{
    [SerializeField] float ownSpeed = 5f;
    [SerializeField] float laneChangeChance = 0.4f;
    [SerializeField] float laneChangeSpeed = 4f;
    [SerializeField] float laneChangeZ = 70f;

    float targetX;
    bool changed;

    public override void OnSpawn()
    {
        base.OnSpawn();
        targetX = transform.position.x;
        changed = Random.value > laneChangeChance;
    }

    protected override void Update()
    {
        base.Update();
        if (!Playing || !gameObject.activeSelf) return;
        var pos = transform.position;
        pos.z += ownSpeed * Time.deltaTime;
        if (!changed && pos.z < laneChangeZ)
        {
            changed = true;
            int lane = Mathf.RoundToInt(pos.x / GameManager.LaneWidth) + (Random.value < 0.5f ? -1 : 1);
            targetX = Mathf.Clamp(lane, -1, 1) * GameManager.LaneWidth;
        }
        pos.x = Mathf.MoveTowards(pos.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = pos;
    }
}
