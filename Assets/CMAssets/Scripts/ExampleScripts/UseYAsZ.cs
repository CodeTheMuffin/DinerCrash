using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseYAsZ : MonoBehaviour
{
	[TextArea(5, 100)]
	public string notes = "This is a simple script to set the Z postion equal to the Y position for 2d objects.\n\nThis is super useful when using sort order as layers but you still want to have things move in front of or behind things. Just remember this uses the pivot position to determine if things are in front or behind.";
    // Update is called once per frame
    void Update()
    {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
}
