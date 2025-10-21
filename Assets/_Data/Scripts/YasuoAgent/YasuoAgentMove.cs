using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class YasuoAgentMove : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Animator agentAnimator;
    [SerializeField] private float moveSpeed = 1.725f;
    [SerializeField] private float rotateSpeed = 180f;

    [Header("Environment")] [SerializeField]
    private GameObject ground;

    [SerializeField] private List<GameObject> walls;
    [SerializeField] private float randomValue;

    private float lastDistanceToTarget;

    // Khởi tạo agent
    public override void Initialize()
    {
        Debug.Log("YasuoAgentMove Initialize");
    }

    // Bắt đầu một tập mới
    public override void OnEpisodeBegin()
    {
        Debug.Log("YasuoAgentMove OnEpisodeBegin");

        this.lastDistanceToTarget = Vector3.Distance(this.transform.localPosition, this.target.localPosition);
        this.SpawnObjects();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("YasuoAgentMove CollectObservations");

        // Chuẩn hóa vị trí của mục tiêu trong phạm vi [-1, 1]
        float normalizedTargetPosX = this.target.localPosition.x / 15f;
        float normalizedTargetPosZ = this.target.localPosition.z / 15f;
        
        // Chuẩn hóa vị trí của agent trong phạm vi [-1, 1]
        float normalizedAgentPosX = this.transform.localPosition.x / 15f;
        float normalizedAgentPosZ = this.transform.localPosition.z / 15f;
        
        // Tính khoảng cách giữa agent và mục tiêu
        float dis = Vector3.Distance(this.transform.localPosition, this.target.localPosition);
        float normalizedDis = dis / (5f * this.randomValue);
        
        // Chuẩn hóa hướng của agent trong phạm vi [-1, 1]
        float normalizedAgentRotY = (this.transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        // Tính hướng từ agent đến mục tiêu
        Vector3 targetDir = (this.target.localPosition - this.transform.localPosition).normalized;

        // Thêm các quan sát vào sensor
        sensor.AddObservation(normalizedTargetPosX);
        sensor.AddObservation(normalizedTargetPosZ);
        sensor.AddObservation(normalizedAgentPosX);
        sensor.AddObservation(normalizedAgentPosZ);
        sensor.AddObservation(normalizedDis);
        sensor.AddObservation(normalizedAgentRotY);
        sensor.AddObservation(targetDir.x);
        sensor.AddObservation(targetDir.z);
    }

    // Xử lý các hành động từ agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("YasuoAgentMove OnActionReceived");

        // Thực hiện hành động di chuyển
        this.Move(actions.DiscreteActions);

        // Phạt nhẹ mỗi bước để khuyến khích hoàn thành nhiệm vụ nhanh hơn
        AddReward(-1f / MaxStep);

        // Thưởng dựa trên sự tiến gần hơn đến mục tiêu
        // float currentDistance = Vector3.Distance(this.transform.localPosition, this.target.localPosition);
        // float delta = this.lastDistanceToTarget - currentDistance;
        // if (delta > 0)
        // {
        //     AddReward(delta * 1f / MaxStep * 100f);
        // }
        // else
        // { 
        //     AddReward( delta * -1f / MaxStep);
        // }
        //
        // this.lastDistanceToTarget = currentDistance;
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
            AddReward(-1f);
            EndEpisode();
        }
    }

    // Khởi tạo các đối tượng trong môi trường
    void SpawnObjects()
    {
        // Lấy ngẫu nhiên giá trị để thay đổi cấu trúc môi trường
        //his.randomValue = Random.Range(1f, 3f);

        // Thay đổi kích thước mặt đất
        this.ground.transform.localScale = new Vector3(1, 1, 1) * this.randomValue;

        // Thay đổi kích thước và vị trí các bức tường
        this.walls[0].transform.localScale = new Vector3(1, 1, 10 * this.randomValue);
        this.walls[0].transform.localPosition = new Vector3(5, 0, 0) * this.randomValue + new Vector3(0, 0.5f, 0);
        this.walls[1].transform.localScale = new Vector3(1, 1, 10 * this.randomValue);
        this.walls[1].transform.localPosition = new Vector3(-5, 0, 0) * this.randomValue + new Vector3(0, 0.5f, 0);
        this.walls[2].transform.localScale = new Vector3(10 * this.randomValue + 1, 1, 1);
        this.walls[2].transform.localPosition = new Vector3(0, 0, 5) * this.randomValue + new Vector3(0, 0.5f, 0);
        this.walls[3].transform.localScale = new Vector3(10 * this.randomValue + 1, 1, 1);
        this.walls[3].transform.localPosition = new Vector3(0, 0, -5) * this.randomValue + new Vector3(0, 0.5f, 0);

        // Đặt lại vị trí và hướng của agent
        this.transform.localRotation = Quaternion.identity;
        this.transform.localPosition = Vector3.zero;

        // Tạo khoảng cách ngẫu nhiên cho mục tiêu
        float randDis = Random.Range(1.5f * this.randomValue, 4f * this.randomValue);

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
                this.transform.Rotate(Vector3.up, -this.rotateSpeed * Time.fixedDeltaTime);
                speed = this.moveSpeed;
                break;
            case 3: // Quay phải
                this.transform.Rotate(Vector3.up, this.rotateSpeed * Time.fixedDeltaTime);
                speed = this.moveSpeed;
                break;
            default: // Không di chuyển
                speed = 0;
                break;
        }

        // Cập nhật vị trí của agent
        this.agentAnimator.SetFloat(nameof(AnimationParamsEnum.MoveSpeed), speed);
        this.transform.localPosition += moveDir * this.moveSpeed * Time.fixedDeltaTime;
    }

    void GoalReached()
    {
        Debug.Log("YasuoAgent GoalReached");

        // Thưởng khi đạt được mục tiêu
        AddReward(2f);

        // Kết thúc tập hiện tại
        Debug.Log($"YasuoAgent Reward: {GetCumulativeReward()}");
        EndEpisode();
    }
}