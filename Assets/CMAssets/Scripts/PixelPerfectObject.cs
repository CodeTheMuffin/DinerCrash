using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PixelPerfectObject : MonoBehaviour
{
	public bool useGlobalPixelsPerUnit = true;
	[Tooltip("Since this was made for the low rez jam, it's default is 8 pixels per unit. This should be the same as the import settings you used on all your sprites.")]
	public int pixelsPerUnit = 8;
	
	[Tooltip("With this checked, it will apply the y value to z. This makes things automatically change their layer position to be infront of or behind things based on the y position. This is useful for top down environments like RPGs and such.")]
	public bool useYForSortOrderAssist = true;

    // Start is called before the first frame update
    void Start()
    {
        if(Game.instance != null)
		{
			pixelsPerUnit = Game.pixelsPerUnit;
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (useYForSortOrderAssist)
		{
			transform.position = new Vector3(GetSnapped(transform.position.x), GetSnapped(transform.position.y), GetSnapped(transform.position.y));
		}
		else
		{ 
			transform.position = new Vector3(GetSnapped(transform.position.x), GetSnapped(transform.position.y), GetSnapped(transform.position.z));
		}
    }

	public float GetSnapped(float input)
	{
		float output = 0;

		output = input * pixelsPerUnit;
		output = Mathf.Round(output);
		output = output / pixelsPerUnit;

		return output;
	}
}

class PixelPerfect
{
	public static float GetSnapped(float input, float pixelsPerUnit)//Suggest Game.pixelsPerUnit
	{
		float output = 0;

		output = input * pixelsPerUnit;
		output = Mathf.Round(output);
		output = output / pixelsPerUnit;

		return output;
	}
}
