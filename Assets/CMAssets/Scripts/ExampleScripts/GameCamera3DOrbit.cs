using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera3DOrbit: MonoBehaviour
{
	public GameObject player;
	public Vector3 offset = Vector3.zero;
	float tiltRot = 0;

	// Start is called before the first frame update
	void Start()
	{ 
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (player != null)
		{
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + offset;
		}

		tiltRot = Mathf.Clamp(tiltRot + Input.GetAxisRaw("Mouse Y"), -90, 90);
		transform.localEulerAngles = new Vector3(tiltRot, transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X"), transform.localEulerAngles.z);
	}
} 