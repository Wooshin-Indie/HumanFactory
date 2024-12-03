
using HumanFactory.Controller;
using UnityEngine;

namespace HumanFactory.StateMachine
{
    public class IdleState : StateBase
    {
        public IdleState(HumanController controller, HumanStateMachine stateMachine) 
            : base(controller, stateMachine)
        {
        }

        public override void Enter() { }
        public override void HandleInput() { }
        public override void LogicUpdate() { }
        public override void PhysicsUpdate() { }
        public override void Exit() { }
    }
}