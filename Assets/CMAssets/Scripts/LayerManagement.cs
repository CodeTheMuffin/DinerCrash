using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/*
 *	This script was NOT created by Caliber Mengsk. There is not any good documentation on how to add layers, so I used code freely available from a website.
 *	https://www.programmersought.com/article/7403668961/
 *	All credit to them.
 *	
 *	This document has been cleaned and modified to work for adding the RenderTexture layer for the camera system to work with render textures.
 */

public class LayerManagement : MonoBehaviour
{
#if UNITY_EDITOR
	public static bool HasLayer(string layer)
	{
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/Tagmanager.asset"));
		SerializedProperty it = tagManager.GetIterator();
		while (it.NextVisible(true))
		{
			if (it.name == "layers")
			{
				for (int i = 0; i < it.arraySize; i++)
				{
					SerializedProperty sp = it.GetArrayElementAtIndex(i);
					if (!string.IsNullOrEmpty(sp.stringValue))
					{
						if (sp.stringValue.Equals(layer))
						{
							sp.stringValue = string.Empty;
							tagManager.ApplyModifiedProperties();
						}
					}
				}
			}
		}
		for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
		{
			if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
				return true;
		}
		return false;
	}

	public static void AddLayer(string layer)
	{
		if (!HasLayer(layer))
		{
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty it = tagManager.GetIterator();
			while (it.NextVisible(true))
			{
				if (it.name == "layers")
				{
					for (int i = 0; i < it.arraySize; i++)
					{
						if (i == 3 || i == 6 || i == 7)
						{
							continue;
						}
						SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
						if (string.IsNullOrEmpty(dataPoint.stringValue))
						{
							dataPoint.stringValue = layer;
							tagManager.ApplyModifiedProperties();
							return;
						}
					}
				}
			}
		}
	}
#endif
}
