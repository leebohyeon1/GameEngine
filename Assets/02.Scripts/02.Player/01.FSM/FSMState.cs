public abstract class FSMState
{
    protected FSMBase _fsm;

    public FSMState(FSMBase fsm)
    {
        _fsm = fsm;
    }

    public virtual void OnEnter(PlayerController player) { }
    public virtual void OnExit(PlayerController player) { }
    public virtual void OnUpdate(PlayerController player) { }
}
