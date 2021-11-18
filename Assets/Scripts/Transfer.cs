using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour
{
    // Handle gameobjects collider with a Victory object
	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Player")
		{
            new WaitForSeconds(2.0f);
			// if player then send it to the secret place
			other.gameObject.GetComponent<Transform>().position = new Vector3(-2,-12,0);
		} 
	}
}
