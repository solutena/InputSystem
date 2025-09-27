using System;
using System.Diagnostics;
using UnityEngine;

public class InputAction
{
	public Action Down { get; set; } = null;
	public Action Up { get; set; } = null;
	public Action Hold { get; set; } = null;
	public bool IsHolding { get; private set; } = false;
	public bool IsRefreshed { get; set; } = false;

	public void Update(KeyCode keyCode)
	{
		if (Input.GetKeyDown(keyCode))
		{
			IsHolding = true;
			Down?.Invoke();
		}
		if (IsHolding == false)
			return;
		if (Input.GetKey(keyCode))
			Hold?.Invoke();
		else
		{
			IsHolding = false;
			Up?.Invoke();
		}
	}

	public void Cancel()
	{
		if (IsHolding == false)
			return;
		IsHolding = false;
		Up?.Invoke();
	}

	public void Refresh(KeyCode keyCode)
	{
		if (Input.GetKey(keyCode) == false)
			return;
		IsHolding = true;
		Down?.Invoke();
	}
}
