using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class StopMove : Action
{
    private TutorialEnemy _enemy;

    public override void OnStart()
    {
        _enemy = GetComponent<TutorialEnemy>();
        _enemy.NavMeshAgentCompo.enabled = false;
        _enemy.AnimatorCompo.SetBool("isMove", false);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}