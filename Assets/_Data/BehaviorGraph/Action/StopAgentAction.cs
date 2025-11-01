using System;
using Unity.Behavior;
using Unity.InferenceEngine;
using Unity.MLAgents.Policies;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Stop Agent", story: "Stop [ModelAgent]", category: "Custom", id: "3e1e4a97c6114fb02398e3fd6afa32ad")]
public partial class StopAgentAction : Action
{
    [SerializeReference] public BlackboardVariable<ModelAsset> ModelAgent;
    private YasuoAgentMove yasuoAgentMove;

    protected override Status OnStart()
    {
        this.yasuoAgentMove = GameObject.GetComponent<YasuoAgentMove>();
        this.yasuoAgentMove.enabled = false;
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

