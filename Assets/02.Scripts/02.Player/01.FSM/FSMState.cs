public abstract class FSMState
{
    protected FSMBase _fsm;

    public FSMState(FSMBase fsm)
    {
        _fsm = fsm;
    }

    public virtual void OnEnter(PlayerController player) { }

    public virtual void OnUpdate(PlayerController player)
    {
        if (InputManager.Instance.ItemSelectPressed)
        {
            GameManager.Instance.ChangeGameState(GameState.ItemSelect);
               
        }

        if (GameManager.Instance.GetCurGameState() == GameState.ItemSelect
            && !InputManager.Instance.ItemSelectPressed)
        {
                GameManager.Instance.ChangeGameState(GameState.Play);
        }
    }

    public virtual void OnFixedUpdate(PlayerController player) { }

    public virtual void OnExit(PlayerController player) { }

}
