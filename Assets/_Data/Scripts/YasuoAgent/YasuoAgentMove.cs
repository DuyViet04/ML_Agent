using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class YasuoAgentMove : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Animator agentAnimator;
    [SerializeField] private float moveSpeed = 1.725f;
    [SerializeField] private float rotateSpeed = 180f;

    // Khởi tạo agent
    public override void Initialize()
    {
        Debug.Log("YasuoAgent Initialize");
    }

    // Bắt đầu một tập mới
    public override void OnEpisodeBegin()
    {
        Debug.Log("YasuoAgent OnEpisodeBegin");

        this.SpawnObjects();
    }

    // Thu thập các quan sát từ môi trường
    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("YasuoAgent CollectObservations");

        // Chuẩn hóa vị trí của mục tiêu trong phạm vi [-1, 1]
        float normalizedTargetPosX = this.target.localPosition.x / 15f;
        float normalizedTargetPosZ = this.target.localPosition.z / 15f;

        // Chuẩn hóa vị trí của agent trong phạm vi [-1, 1]
        float normalizedAgentPosX = this.transform.localPosition.x / 15f;
        float normalizedAgentPosZ = this.transform.localPosition.z / 15f;

        // Chuẩn hóa hướng của agent trong phạm vi [-1, 1]
        float normalizedAgentRotY = (this.transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        sensor.AddObservation(normalizedTargetPosX);
        sensor.AddObservation(normalizedTargetPosZ);
        sensor.AddObservation(normalizedAgentPosX);
        sensor.AddObservation(normalizedAgentPosZ);
        sensor.AddObservation(normalizedAgentRotY);
    }

    // Xử lý các hành động từ agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("YasuoAgent OnActionReceived");

        // Thực hiện hành động di chuyển
        this.Move(actions.DiscreteActions);

        // Phạt theo thời gian để khuyến khích hoàn thành nhiệm vụ nhanh hơn
        AddReward(-2f / this.MaxStep);
    }

    // Xử lý va chạm với mục tiêu
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(nameof(TagEnum.Target)))
        {
            this.GoalReached();
        }
    }

    // Xử lý va chạm với tường
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(nameof(TagEnum.Wall)))
        {
            // Phạt khi va chạm với tường
            AddReward(-0.05f);
        }
    }

    // Xử lý va chạm liên tục với tường
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag(nameof(TagEnum.Wall)))
        {
            // Phạt khi va chạm với tường
            AddReward(-0.01f * Time.fixedDeltaTime);
        }
    }

    // Cung cấp hành động thủ công cho agent (dùng để kiểm thử)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("YasuoAgent Heuristic");
        
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0; // Mặc định không di chuyển

        if (Keyboard.current.upArrowKey.isPressed)     
        {
            discreteActionsOut[0] = 1; // Di chuyển về phía trước
        }
        else if (Keyboard.current.leftArrowKey.isPressed)
        {
            discreteActionsOut[0] = 2; // Quay trái
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            discreteActionsOut[0] = 3; // Quay phải
        }
        else
        {
            discreteActionsOut[0] = 0; // Không di chuyển
        }
    }

    // Khởi tạo các đối tượng trong môi trường
    void SpawnObjects()
    {
        // Đặt lại vị trí và hướng của agent
        this.transform.localRotation = Quaternion.identity;
        this.transform.localPosition = Vector3.zero;

        // Tạo khoảng cách ngẫu nhiên cho mục tiêu
        float randDis = Random.Range(3f, 10f);

        // Tạo hướng ngẫu nhiên cho mục tiêu
        float randAngle = Random.Range(0f, 360f);
        Vector3 randDir = Quaternion.Euler(0f, randAngle, 0f) * Vector3.forward;

        // Đặt lại vị trí mục tiêu
        Vector3 targetPos = this.transform.localPosition + randDis * randDir;
        this.target.localPosition = new Vector3(targetPos.x, 0f, targetPos.z);
    }

    void Move(ActionSegment<int> act)
    {
        // Lấy hành động di chuyển
        int moveAction = act[0];
        Vector3 moveDir = Vector3.zero;
        float speed;

        switch (moveAction)
        {
            case 1: // Di chuyển về phía trước
                moveDir = this.transform.forward;
                speed = this.moveSpeed;
                break;
            case 2: // Quay trái
                this.transform.Rotate(0f, -this.rotateSpeed * Time.fixedDeltaTime, 0f);
                speed = this.moveSpeed;
                break;
            case 3: // Quay phải
                this.transform.Rotate(0f, this.rotateSpeed * Time.fixedDeltaTime, 0f);
                speed = this.moveSpeed;
                break;
            default: // Không di chuyển
                speed = 0;
                break;
        }

        // Cập nhật vị trí của agent
        this.agentAnimator.SetFloat(nameof(AnimationParamsEnum.MoveSpeed), speed);
        this.transform.localPosition += moveDir * this.moveSpeed * Time.deltaTime;
    }

    void GoalReached()
    {
        Debug.Log("YasuoAgent GoalReached");

        // Thưởng khi đạt được mục tiêu
        AddReward(1.0f);

        // Kết thúc tập hiện tại
        EndEpisode();
    }
}