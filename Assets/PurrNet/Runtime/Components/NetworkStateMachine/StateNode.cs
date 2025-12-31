namespace PurrNet.StateMachine
{
    public abstract class StateNode : NetworkBehaviour
    {
        protected StateMachine machine { get; private set; }
        public bool isCurrentState => machine && machine.currentStateNode == this;

        public void Setup(StateMachine stateMachine)
        {
            machine = stateMachine;
        }

        /// <summary>
        /// This is called when the state is entered.
        /// </summary>
        public virtual void Enter()
        {
        }

        /// <summary>
        /// This is called when the state is entered.
        /// </summary>
        /// <param name="asServer">Whether you are acting as server or client</param>
        public virtual void Enter(bool asServer)
        {
        }

        /// <summary>
        /// This is like the update loop, which only runs when the state is active.
        /// </summary>
        public virtual void StateUpdate()
        {
        }

        /// <summary>
        /// This is like the update loop, which only runs when the state is active.
        /// </summary>
        /// <param name="asServer">Whether you are acting as server or client</param>
        public virtual void StateUpdate(bool asServer)
        {
        }

        /// <summary>
        /// This is called when the state is exited
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// This is called when the state is exited
        /// </summary>
        /// <param name="asServer">Whether you are acting as server or client</param>
        public virtual void Exit(bool asServer)
        {
        }

        /// <summary>
        /// Override this to control whether the state can be entered
        /// </summary>
        public virtual bool CanEnter() => true;
        
        /// <summary>
        /// Override this to control whether the state can be exited
        /// </summary>
        public virtual bool CanExit() => true;
    }

    public abstract class StateNode<T> : StateNode
    {
        /// <summary>
        /// This is called when the state is entered.
        /// </summary>
        /// <param name="data">The data which the state is entered with</param>
        public virtual void Enter(T data)
        {
        }
        
        /// <summary>
        /// This is called when the state is entered.
        /// </summary>
        /// <param name="asServer">Whether you are acting as server or client</param>
        /// <param name="data">The data which the state is entered with</param>
        public virtual void Enter(T data, bool asServer)
        {
        }
        
        /// <summary>
        /// Override this to control whether the state can be entered
        /// </summary>
        /// <param name="data">The data which the state is attempted to be entered with</param>
        public virtual bool CanEnter(T data) => true;
    }
}