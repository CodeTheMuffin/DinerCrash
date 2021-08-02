using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)] //We want to run this before inputs are calculated by the player.
public class Sign : MonoBehaviour
{
	[Tooltip("Player will automatically be filled on load with the first object having the \"Player\" tag assigned.")]
	public GameObject player;
	[TextArea(3,50)]
	public string message = "";
	public bool useColor = false;
	public Color color;
	public float speed = .1f;
	public float distance = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (!Game.pauseTime && player != null)
		{
			if (Vector2.Distance(transform.position, player.transform.position) <= distance && Input.GetButtonDown(Game.interactButton) && Time.unscaledTime - MessageBoxController.lastMessage > 1f)
			{
				if (useColor)
				{
					MessageBoxController.SayMessage(message, color, speed);
				}
				else
				{
					MessageBoxController.SayMessage(message, MessageBoxController.defaultColor, speed);
				}
			}
		}
    }
}
