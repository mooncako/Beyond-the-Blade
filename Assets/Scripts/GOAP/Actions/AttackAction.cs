using System;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace CrashKonijn.Goap.GenTest
{
    [GoapId("Attack-ac8b7a8d-2943-4f78-9b06-2d3085eedf1b")]
    public class AttackAction : GoapActionBase<AttackAction.Data>, IInjectable
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
            data.Timer = _attackSensorConfig.AttackDelay;
            
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
            if (data.Controller.IsSkillPlaying())
                data.Timer -= context.DeltaTime;
            if (data.Controller.CanAttack)
            {
                data.Controller.SetTargetPos(data.Target.Position);
                data.Controller.ActivateSkill();
            }
            data.Controller.Stop();
            return data.Timer > 0 ? ActionRunState.Continue : ActionRunState.Completed;
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

            public float Timer { get; set; }

            [GetComponent]
            public EnemyController Controller { get; set; }
        }
    }
}