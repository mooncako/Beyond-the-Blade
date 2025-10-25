using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace CrashKonijn.Goap.GenTest
{
    [GoapId("AttackCautious-456f7e7d-2bb8-4f55-b312-814f20114b69")]
    public class AttackCautiousAction : GoapActionBase<AttackCautiousAction.Data>, IInjectable
    {
        private AttackSensorConfigSO _attackSensorConfig;


        // This method is called when the action is created
        // This method is optional and can be removed
        public override void Created()
        {
        }

        // This method is called every frame before the action is performed
        // If this method returns false, the action will be stopped
        // This method is optional and can be removed
        public override bool IsValid(IActionReceiver agent, Data data)
        {
            return data.Controller.CanAttack;
        }

        // This method is called when the action is started
        // This method is optional and can be removed
        public override void Start(IMonoAgent agent, Data data)
        {
        }

        // This method is called once before the action is performed
        // This method is optional and can be removed
        public override void BeforePerform(IMonoAgent agent, Data data)
        {
        }

        // This method is called every frame while the action is running
        // This method is required
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            if (data.Controller.CanAttack)
            {
                data.Controller.CanAttack = false;
                data.Controller.SetTargetPos(data.Target.Position);
                data.Controller.ActivateSkill();
                data.AnimationStateMachine.SetAction(data.Controller.CurrentWeapon.GetAnimationClip(data.Controller.GetCurrentSkillAnimationID()), AnimationStateType.Attack, data.Controller.GetCurrentSkill());
                data.AnimationStateMachine.SwitchState(AnimationStateType.Attack);

            }
            

            data.Controller.Stop();
            return data.AnimationStateMachine.IsInAttackActionState() ? ActionRunState.Continue : ActionRunState.Completed;
        }

        // This method is called when the action is completed
        // This method is optional and can be removed
        public override void Complete(IMonoAgent agent, Data data)
        {
        }

        // This method is called when the action is stopped
        // This method is optional and can be removed
        public override void Stop(IMonoAgent agent, Data data)
        {
        }

        // This method is called when the action is completed or stopped
        // This method is optional and can be removed
        public override void End(IMonoAgent agent, Data data)
        {
        }

        public void Inject(DependencyInjector injector)
        {
            _attackSensorConfig = injector.AttackSensorConfig;
        }

        // The action class itself must be stateless!
        // All data should be stored in the data class
        public class Data : IActionData
        {
            public ITarget Target { get; set; }

            [GetComponent]
            public EnemyController Controller { get; set; }

            [GetComponent]
            public AnimationStateMachine AnimationStateMachine { get; set; }
        }
    }
}