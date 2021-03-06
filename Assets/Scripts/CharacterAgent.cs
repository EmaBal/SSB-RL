using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CharacterAgent : Agent
{
    private CharacterController2D character;
    private Victory rose;
    private Vector3 localPos;
    private Vector3 rosePos;

    private int jumpNumber = 0;
    private int stepCount = 0;

    private void Awake()
    {
        character = GetComponent<CharacterController2D>();
        rose = GameObject.FindWithTag("Rose").GetComponent<Victory>();
        rosePos = rose.transform.localPosition;
        Academy.Instance.AutomaticSteppingEnabled = false;
    }
    
    private void Start()
    {
        CharacterController2D.getInstance().OnDied += Character_OnDied;
    }

    private void FixedUpdate()
    {
        stepCount++;
        if (stepCount >= MaxStep)
        {
            StartCoroutine(character.KillPlayer());
            EndEpisode();
        }
    }

    void Update()
    {
        Academy.Instance.EnvironmentStep();
    }
    
    private void Character_OnDied(object sender, System.EventArgs e)
    {
        Debug.Log("END EPISODE");
        EndEpisode();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(character.transform.localPosition);
        if (character._isGrounded)
        {
            jumpNumber = 2;
        } else if (character._canDouvbleJump)
        {
            jumpNumber = 1;
        }
        else
        {
            jumpNumber = 0;
        }
        
        sensor.AddObservation(jumpNumber);
        sensor.AddObservation(rosePos);

    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
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
                character.jumping = 1; //jump button down
                break;
            case 2:
                character.jumping = 2; //jump button up
                break;
        }
        AddReward(-1f / MaxStep);
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
            EndEpisode();
        }
        if (c.gameObject.CompareTag("Teleport"))
        {
            AddReward(4f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            AddReward(.1f);
        }
        if (collision.gameObject.CompareTag("Rose"))
        {
            AddReward(5f);
            Debug.Log("END EPISODE WIN");
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}