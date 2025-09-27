using System;
using UnityEngine;

[Serializable]
public class InputBinding
{
	public string actionName = null;
	public string keyString = null;

	private KeyCode _keyCode = KeyCode.None;
	public KeyCode KeyCode
	{
		get
		{
			if (_keyCode == KeyCode.None)
				_keyCode = Enum.Parse<KeyCode>(keyString);
			return _keyCode;
		}
	}
}