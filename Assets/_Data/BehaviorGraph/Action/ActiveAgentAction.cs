using System;
using Unity.Behavior;
using Unity.InferenceEngine;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Active Agent", story: "Active [ModeAgent]", category: "Custom", id: "950566ce53a3ff399dbdea8af6068e20")]
public partial class ActiveAgentAction : Action
{
    [SerializeReference] public BlackboardVariable<ModelAsset> ModeAgent;
    private YasuoAgentMove yasuoAgentMove;

    protected override Status OnStart()
    {
        this.yasuoAgentMove = GameObject.GetComponent<YasuoAgentMove>();
        this.yasuoAgentMove.enabled = true;
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

