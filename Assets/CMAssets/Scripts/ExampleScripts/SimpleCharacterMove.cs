using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterMove : MonoBehaviour
{
	public float moveSpeed = 3f;
	Vector3 moveDirection = Vector3.zero;
	Vector2 position;

	Rigidbody2D rb;
    void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		moveDirection.x = Input.GetAxis("Horizontal") * moveSpeed;
		moveDirection.y = Input.GetAxis("Vertical") * moveSpeed;
		rb.velocity = moveDirection;
    }
}
