using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IState
{
    public void Enter();
    public void Execute();
    public void Exit();
}

public enum MoveState 
{
    Standard, GroundPounding, Flying, Grappling, Suspended
}
public class StandardState : IState
{
    CharacterMovement CharacterMovement;
    public StandardState (CharacterMovement Character)
    {
        Debug.Log("entering GroundedState state");
        CharacterMovement = Character;
    }
    public void Enter()
    {
        PlayerInput.Move += CharacterMovement.RegularMovement;
    }

    public void Execute()
    {
        Debug.Log("updating Grounded state");
    }

    public void Exit()
    {
        Debug.Log("exiting GroundedState state");
        PlayerInput.Move -= CharacterMovement.RegularMovement;
        
    }
}
public class GroundingState : IState
{
    CharacterMovement CharacterMovement;
    public GroundingState(CharacterMovement Character)
    {
        CharacterMovement = Character;
    }
    public void Enter()
    {
        PlayerInput.Move += CharacterMovement.GroundPoundMovement;
    }

    public void Execute()
    {
        Debug.Log("updating Grounded state");
    }

    public void Exit()
    {
        PlayerInput.Move -= CharacterMovement.GroundPoundMovement;

    }
}

public class FlyingState : IState
{
    CharacterMovement CharacterMovement;
    public FlyingState(CharacterMovement Character)
    {
        CharacterMovement = Character;
    }
    public void Enter()
    {
        Debug.Log("entering FlyingState state");
        PlayerInput.Move += CharacterMovement.FlyMovement;
    }

    public void Execute()
    {
        Debug.Log("updating Flying state");
    }

    public void Exit()
    {
        Debug.Log("exiting FlyingState state");
        PlayerInput.Move -= CharacterMovement.FlyMovement;
    }
}
public class GrapplingState : IState
{
    public void Enter()
    {
        Debug.Log("entering Grappling state");
    }

    public void Execute()
    {
        Debug.Log("updating Grappling state");
    }

    public void Exit()
    {
        Debug.Log("exiting Grappling state");
    }
}
public class SuspendedState : IState
{
    public void Enter()
    {
        Debug.Log("entering Suspended state");
    }

    public void Execute()
    {
        Debug.Log("updating Suspended state");
    }

    public void Exit()
    {
        Debug.Log("exiting Suspended state");
    }
}
public class MovementState 
{
    IState currentState;
    CharacterMovement Character;
    public MovementState(CharacterMovement character)
    {
        Character = character;
    }
    public void ChangeState(MoveState State)
    {
        if (currentState != null)
            currentState.Exit();

        switch (State)
        {
            case MoveState.Standard:
                currentState = new StandardState(Character);
                break;
            case MoveState.Flying:
                currentState = new FlyingState(Character);
                break;
            case MoveState.Grappling:
                currentState = new GrapplingState();
                break;
            case MoveState.Suspended:
                currentState = new SuspendedState();
                break;
            case MoveState.GroundPounding:
                currentState = new GroundingState(Character);
                break;
            default:
                currentState = new StandardState(Character);
                break;
        }
     
        currentState.Enter();
    } 
}
