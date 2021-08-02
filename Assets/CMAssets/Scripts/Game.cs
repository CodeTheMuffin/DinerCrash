using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using System.Reflection;
using System;

[ExecuteAlways]
public class Game : MonoBehaviour
{
	[TextArea(5, 10)]
	public string Notes = "The Game script is a simple container for static values that are easier to grab. For instance if the game is paused. You can also use it for storing settings.";
	public bool _enforcePixelPerfectOnAllSprites = true;
	public bool _enforcePixelPerfectOnMessageBoxLetters = false;
	public bool _pixelPerfectUseYForSortOrderAssist = true;
	public int _pixelsPerUnit = 8;

	public static bool pauseTime = false; //Change this state to pause things. An if statement checking this value should happen on all scripts where you want stuff paused, like enemies and the player.
										  //This allows objects to be paused while animations (like the message box, or water tiles) are able to play still.
	public static Color defaultMessageBoxColor = Color.yellow;//Change this to set the default color of the message boxes.
	public static bool enforcePixelPerfectOnAllSprites = true;//Disable this on low framerate in editor.
	public static bool enforcePixelPerfectOnMessageBoxLetters = false;
	public static bool pixelPerfectUseYForSortOrderAssist = true;
	public static int pixelsPerUnit = 8;

	public static Game instance;

	public static string interactButton = "Fire1";

	public static float warningTime = 0;

	// Start is called before the first frame update
	void Start()
    {
		#if UNITY_EDITOR
		LayerManagement.AddLayer("RenderTexture");
		#endif

		if (instance == null)
		{
			instance = this;
		}
		else
		{
			if (Time.time > Game.warningTime + 10)
			{
				Debug.LogError("A game object (" + instance.gameObject.name + ") with the game script (probably a camera object) is already in the scene. \nIf you'd like to use this prefab, please remove the game object with this instance from the scene and try adding the prefab again.");
				warningTime = Time.time;
			}
			DestroyImmediate(gameObject);
		}
    }

    // Update is called once per frame
    void Update()
    {
		#if UNITY_EDITOR
			enforcePixelPerfectOnAllSprites = _enforcePixelPerfectOnAllSprites;
			enforcePixelPerfectOnMessageBoxLetters = _enforcePixelPerfectOnMessageBoxLetters;
			pixelPerfectUseYForSortOrderAssist = _pixelPerfectUseYForSortOrderAssist;
			pixelsPerUnit = _pixelsPerUnit;
		#endif
		if (enforcePixelPerfectOnAllSprites && 1.0f / Time.deltaTime > 20)//minimum framerate as a just in case.
		{
			SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
			foreach (SpriteRenderer sprite in sprites)
			{
				if ((sprite.gameObject.name != "MessageBoxLetter" && sprite.gameObject.name != "WaitCursor" && sprite.gameObject.name != "MessageBox") || enforcePixelPerfectOnMessageBoxLetters == true)
				{
					PixelPerfectObject ppo = sprite.GetComponent<PixelPerfectObject>();
					if (ppo == null)
					{
						ppo = sprite.gameObject.AddComponent<PixelPerfectObject>();
						ppo.useYForSortOrderAssist = pixelPerfectUseYForSortOrderAssist;
						Debug.Log("Added PixelPerfectObject script to <color=blue><b>" + sprite.gameObject.name + "</b></color> because it was missing.\n If you'd like to stop this from happening, set <color=blue><b>enforcePixelPerfectOnAllSprites</b></color> to false in the \"Game\" script.");
					}
				}
			}
		}
    }

	private void OnDestroy()
	{
		instance = null;
	}
}

#if UNITY_EDITOR
public class LayerData
{
	// Get the sorting layer names
	public static string[] GetSortingLayerNames()
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
	// Get the unique sorting layer IDs -- tossed this in for good measure
	public static int[] GetSortingLayerUniqueIDs()
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
		return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
	}
}
#endif