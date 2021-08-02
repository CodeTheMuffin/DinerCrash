using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GameCamera : MonoBehaviour
{
	public GameObject player;
	public Vector3 offset = Vector3.zero;
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
    }
}
