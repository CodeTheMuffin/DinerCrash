using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageOnLoad : MonoBehaviour
{
	[TextArea(3,500)]
	public string message = "";
    // Start is called before the first frame update
    void Start()
    {
		MessageBoxController.SayMessage(message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
