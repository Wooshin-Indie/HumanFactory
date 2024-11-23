

namespace HumanFactory.StateMachine
{
    public class HumanStateMachine
    {
        private StateBase curState;
        public StateBase CurState { get { return curState; } }

        public void Init(StateBase startState)
        {
            curState = startState;
            startState.Enter();
        }

        public void ChangeState(StateBase newState)
        {
            curState.Exit();

            curState = newState;
            newState.Enter();
        }
    }
}