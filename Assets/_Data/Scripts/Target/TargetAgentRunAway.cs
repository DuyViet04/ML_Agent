using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class TargetAgentRunAway : Agent
{
    [SerializeField] private Transform yasuoAgent;
    [SerializeField] private float moveSpeed = 1.875f;

    private float lastDistance;

    public override void OnEpisodeBegin()
    {
        Debug.Log("TargetRunAway OnEpisodeBegin");

        this.lastDistance = Vector3.Distance(this.transform.localPosition, this.yasuoAgent.localPosition);
    }

    // Thu thập các quan sát từ môi trường
    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("TargetAgentRunAway CollectObservations");

        // Chuẩn hóa vị trí của YasuoAgent trong phạm vi [-1, 1]
        float normalizedYasuoPosX = this.yasuoAgent.localPosition.x / 15f;
        float normalizedYasuoPosZ = this.yasuoAgent.localPosition.z / 15f;

        // Chuẩn hóa vị trí của agent trong phạm vi [-1, 1]
        float normalizedAgentPosX = this.transform.localPosition.x / 15f;
        float normalizedAgentPosZ = this.transform.localPosition.z / 15f;

        // Chuẩn hoá khoảng cách giữa agent và YasuoAgent trong phạm vi [0, 1]
        float dis = Vector3.Distance(this.yasuoAgent.localPosition, this.transform.localPosition);
        float normalizedDis = dis / 15f;

        // Chuẩn hoá hướng của yasuoAgent trong phạm vi [-1, 1]
        float normalizedYasuoRotY = (this.yasuoAgent.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        // Chuẩn hoá hướng của agent trong phạm vi [-1, 1]
        float normalizedAgentRotY = (this.transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        // Thêm các quan sát vào sensor
        sensor.AddObservation(normalizedYasuoPosX);
        sensor.AddObservation(normalizedYasuoPosZ);
        sensor.AddObservation(normalizedAgentPosX);
        sensor.AddObservation(normalizedAgentPosZ);
        sensor.AddObservation(normalizedDis);
        sensor.AddObservation(normalizedYasuoRotY);
        sensor.AddObservation(normalizedAgentRotY);
    }

    // Xử lý các hành động của agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("TargetAgentRunAway OnActionReceived");

        this.Move(actions.DiscreteActions);

        AddReward(2f / MaxStep); // Thưởng nhẹ mỗi bước để khuyến khích chạy xa hơn

        // Tính khoảng cách hiện tại giữa agent và YasuoAgent
        float currentDistance = Vector3.Distance(this.transform.localPosition, this.yasuoAgent.localPosition);

        if (currentDistance > this.lastDistance)
        {
            AddReward(1f / MaxStep * 50); // Thưởng nếu khoảng cách tăng lên
        }
        else
        {
            AddReward(-1f / MaxStep); // Phạt nếu khoảng cách giảm xuống
        }

        this.lastDistance = currentDistance;
    }

    // Thực hiện hành động di chuyển
    private void Move(ActionSegment<int> act)
    {
        Vector3 moveDir = Vector3.zero;
        int moveAction = act[0];

        switch (moveAction)
        {
            case 1: // Di chuyển về phía trước
                moveDir = this.transform.forward;
                break;
            case 2: // Quay trái
                this.transform.Rotate(Vector3.up, -180f * Time.fixedDeltaTime);
                break;
            case 3: // Quay phải
                this.transform.Rotate(Vector3.up, 180f * Time.fixedDeltaTime);
                break;
        }

        this.transform.position += moveDir * this.moveSpeed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(nameof(TagEnum.Wall)))
        {
            // Phạt khi chạm vào tường
            AddReward(-1f);
            Debug.Log($"Target Reward: {GetCumulativeReward()}");
            EndEpisode();
        }
        else if (other.gameObject.CompareTag(nameof(TagEnum.Wall)))
        {
            AddReward(-1f);
            Debug.Log($"Target Reward: {GetCumulativeReward()}");
            EndEpisode();
        }
    }
}