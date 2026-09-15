using System;
using System.Collections.Generic;
using Godot;

public partial class NodeStateMachine : Node
{
    [Export]
    private NodeState initialNodeState;

    private NodeState activeState;

    private Dictionary<string, NodeState> NodeStates = [];

    public override void _Ready()
    {
        foreach (NodeState nodeState in GetChildren())
        {
            NodeStates[nodeState.Name.ToString()] = nodeState;
            nodeState.Connect(NodeState.SignalName.TransitionState, Callable.From<string>(TransiteState));
        }

        initialNodeState?.OnEnter();
        activeState = initialNodeState;
    }

    public override void _Process(double delta)
    {
        activeState?.UpdateProcess(delta); 
    }

    public override void _PhysicsProcess(double delta)
    {
        activeState?.UpdatePhysicsProcess(delta); 
    }



    public void TransiteState(string state)
    {
        if(state == activeState.Name)
        {
            return;
        }

        activeState?.OnExit();

        activeState = NodeStates[state];
        activeState?.OnEnter();

        GD.Print("Current state ", state);
    }
    

}
