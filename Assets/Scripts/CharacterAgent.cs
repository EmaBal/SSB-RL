using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CharacterAgent : Agent
{
    private CharacterController2D character;

    private void Awake() {
        character = GetComponent<CharacterController2D>();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(character.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var dirToGo = Vector3.zero;
        
        var movement = actionBuffers.DiscreteActions[0];
        var jump = actionBuffers.DiscreteActions[1];

        switch (movement)
        {
            case 1:
                dirToGo = transform.right;
                break;
            case 2:
                dirToGo = -transform.right;
                break;
        }

        if (jump == 1)
        {
            character.DoJump();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[1] = 1;
        }
    }
}
