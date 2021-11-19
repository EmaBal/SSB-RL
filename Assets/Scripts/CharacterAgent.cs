using System;
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

    public override void OnEpisodeBegin()
    {
        
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        var dirToGo = Vector2.zero;
        
        var movement = actionBuffers.DiscreteActions[0];
        var jump = actionBuffers.DiscreteActions[1];

        switch (movement)
        {
            case 0:
                character.direction = 0; //character fermo
                break;
            case 1:
                character.direction = 1; //movimento a destra
                break;
            case 2:
                character.direction = -1; //movimento a sinistra
                break;
        }

        switch (jump)
        {
            case 0:
                character.jumping = 0; //non sta saltando
                break;
            case 1:
                character.jumping = 1; //jump down
                break;
            case 2:
                character.jumping = 2; //jump up
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetAxisRaw("Horizontal") == 1f)
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetAxisRaw("Horizontal") == -1f)
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetButtonDown("Jump"))
        {
            discreteActionsOut[1] = 1;
        }

        if (Input.GetButtonUp("Jump"))
        {
            discreteActionsOut[1] = 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("DeathZone"))
        {
            AddReward(-1f);
            Debug.Log("DEATHZONE COLLISION");
            EndEpisode();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            AddReward(.1f);
            Debug.Log("COIN COLLISION");
        }
        if (collision.gameObject.CompareTag("Rose"))
        {
            AddReward(5f);
            Debug.Log("ROSE COLLISION");
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AddReward(-1f);
            Debug.Log("ENEMY COLLISION");
            EndEpisode();
        }
    }
}