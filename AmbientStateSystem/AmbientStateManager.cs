using UnityEngine;

public class AmbientStateManager : MonoBehaviour

{
    IAmbientState _currentState;

    public void Set(IAmbientState state)
    {
        if (_currentState != null) {
            _currentState.OnAmbientStateExit();
        }

        _currentState = state;
        _currentState.OnAmbientStateEnter();
    }
}