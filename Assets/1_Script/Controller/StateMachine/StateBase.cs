using HumanFactory.Controller;

namespace HumanFactory.StateMachine
{
    public class StateBase
    {

        protected HumanController controller;
        protected HumanStateMachine stateMachine;

        public StateBase(HumanController controller, HumanStateMachine stateMachine)
        {
            this.controller = controller;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void HandleInput() { }
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Exit() { }
    }
}