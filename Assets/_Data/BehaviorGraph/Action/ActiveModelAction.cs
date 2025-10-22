using System;
using Unity.Behavior;
using Unity.InferenceEngine;
using Unity.MLAgents.Policies;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ActiveModel", story: "Active [AgentModel] with [value] sensor and [size] branch size",
    category: "Custom",
    id: "90c97717cda6e6ea0f93d02cd6dcb942")]
public partial class ActiveModelAction : Action
{
    [SerializeReference] public BlackboardVariable<ModelAsset> AgentModel;
    [SerializeReference] public BlackboardVariable<int> Value;
    [SerializeReference] public BlackboardVariable<int> Size;

    private BehaviorParameters yasuoAgentBehaviorParameters;

    protected override Status OnStart()
    {
        this.yasuoAgentBehaviorParameters = GameObject.GetComponent<BehaviorParameters>();
        if (this.yasuoAgentBehaviorParameters == null)
        {
            Debug.LogError("BehaviorParameters component not found on the GameObject.");
            return Status.Failure;
        }

        this.yasuoAgentBehaviorParameters.Model = this.AgentModel.Value;
        this.yasuoAgentBehaviorParameters.BrainParameters.VectorObservationSize = this.Value.Value;
        var spec = this.yasuoAgentBehaviorParameters.BrainParameters.ActionSpec;
        spec.BranchSizes = new int[1] { this.Size.Value };
        this.yasuoAgentBehaviorParameters.BrainParameters.ActionSpec = spec;
        this.yasuoAgentBehaviorParameters.BehaviorType = BehaviorType.InferenceOnly;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}