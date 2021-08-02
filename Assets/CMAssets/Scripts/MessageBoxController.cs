using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MessageBoxController : MonoBehaviour
{
	[TextArea(5, 10)]
	public string Notes = "This is a basic message box. You only have to call \"MessageBoxController.SayMessage(themessageyouwanttosay)\" from any script.";
	public static MessageBoxController instance;
	public static float lastMessage = 0;

	[TextArea]
	[HideInInspector]
	public string message;
	public Color fontColor;
	public GameObject waitCursor;
	public string characterMap;
	public List<Sprite> characterObjects = new List<Sprite>();
	public Vector3 fontOffset;
	public float fastSpeedMultiplier = 2f;
	public Vector3 positionOffset = new Vector3(-4,4,15);

	string curMessage;
	float _speed;
	
	int count = 0;
	bool wait = false;
	public static Color defaultColor = new Color(0, 191, 243, 1);

	float curTime = 0;

	List<GameObject> gos = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
        if(message != "")
		{
			MessageBoxController.SayMessage(message);
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (wait)
		{
			waitCursor.GetComponent<Animator>().SetBool("waiting", true);
			if (Input.GetButtonDown("Fire1"))
			{
				if (count >= message.Length)
				{
					message = "";
					Clear();
					Time.timeScale = 1;
					Game.pauseTime = false;
					lastMessage = Time.unscaledTime;
					this.GetComponent<SpriteRenderer>().enabled = false;
				}
				else
				{
					message = message.Substring(36);
					MessageBoxController.SayMessage(message,_speed);
				}
			}
		}
	}

	private void LateUpdate()
	{
		transform.localPosition = positionOffset;
	}

	public static void SayMessage(string newMessage, float speed = .1f)
	{
		MessageBoxController.instance._SayMessage(newMessage, speed);
	}

	public static void SayMessage(string newMessage, Color newColor, float speed = .1f)
	{
		MessageBoxController.instance._SayMessage(newMessage, newColor, speed);
	}

	void _SayMessage(string newMessage, Color newColor, float speed = .1f)
	{
		GetComponent<SpriteRenderer>().color = newColor;
		float fontDarker = .3f;
		waitCursor.GetComponent<SpriteRenderer>().color = newColor - new Color(fontDarker, fontDarker, fontDarker, 0);
		fontColor = newColor - new Color(fontDarker, fontDarker, fontDarker, 0);
		_SayMessage(newMessage, speed);
	}

	void _SayMessage(string newMessage, float speed = .1f)
	{
		_speed = speed;
		Game.pauseTime = true;
		Time.timeScale = 0;
		this.GetComponent<SpriteRenderer>().enabled = true;
		message = newMessage.ToUpper();
		string[] lines = message.Split('\n');
		for (int i = 0; i < lines.Length; i++)
		{
			while(lines[i].Length < 12)
			{
				lines[i] += " ";
			}
		}
		message = "";
		for (int i = 0; i < lines.Length; i++)
		{
			message += lines[i];
		}
		Clear();

		waitCursor.GetComponent<SpriteRenderer>().color = fontColor;
		StartCoroutine(NextLetter());
	}

	public void Clear()
	{
		wait = false;
		for (int i = 0; i < gos.Count; i++)
		{
			DestroyImmediate(gos[i]);
		}
		gos.Clear();
		count = 0;
		if (IsInvoking("NextLetter"))
		{
			CancelInvoke("NextLetter");
		}
		waitCursor.GetComponent<Animator>().SetBool("waiting", false);
	}

	IEnumerator NextLetter()
	{
		if(count >= message.Length)
		{
			wait = true;
		}
		float lastTime = Time.unscaledTime;
		while (!wait && count < message.Length)
		{
			float thisTime = Time.unscaledTime;
			float delta = thisTime - lastTime;//Delta time must be manually calculated in while loop as Time.deltaTime and Time.unscaledDeltaTime both only update at the beginning of the frame.
			lastTime = thisTime;
			if (Input.GetButton(Game.interactButton))
			{
				curTime += delta * fastSpeedMultiplier;
			}
			else
			{
				curTime += delta;
			}
			if (curTime > _speed)
			{
				char curChar = message[count];
				if (count < message.Length && message.Length > 0)
				{
					try
					{
						while (curChar == ' ' && count < 35 )
						{
							count++;
							if (count < message.Length)
							{
								curChar = message[count];
							}
						}
					}catch(Exception ex)
					{
						print("Message:" + ex.Message + " Count: " + count + " Length: " + message.Length);
					}

					if (curChar != ' ')
					{
						curTime -= _speed;
						int index = characterMap.IndexOf(curChar);
						GameObject go = new GameObject();
						go.transform.parent = transform;
						float left = (count % 12) * .625f;

						go.transform.localPosition = new Vector3(left, 3 - Mathf.Floor(count / 12), 0) + fontOffset;
						SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
						sr.spriteSortPoint = SpriteSortPoint.Pivot;
						sr.color = fontColor;
						sr.sortingOrder = 101;
						sr.sprite = characterObjects[index];
						sr.size = new Vector2(5, 5);
						sr.transform.localEulerAngles = Vector3.zero;
						sr.name = "MessageBoxLetter";
						sr.gameObject.layer = gameObject.layer;
						gos.Add(go);
					}
				}
				count++;
				if (count >= 36 || count >= message.Length)
				{
					wait = true;
				}
				
			}
			yield return new WaitForEndOfFrame();
		}
	}


	public static string ReplaceFirst(string text, string search, string replace)
	{
		int pos = text.IndexOf(search);
		if (pos < 0)
		{
			return text;
		}
		return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
	}
}
