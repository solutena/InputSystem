using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InputMap", menuName = "Input System/Input Map")]
public class InputMap : ScriptableObject
{
	internal readonly Dictionary<InputBinding, InputAction> actions = new();
	internal readonly Dictionary<InputBinding, bool> disabledBuffer = new();
	internal readonly HashSet<InputBinding> disabledBindings = new();
	internal readonly HashSet<InputMap> includeMaps = new();

	[SerializeField] List<InputBinding> serialized;
	Dictionary<string, InputBinding> binding;

	private void OnEnable()
	{
		binding = serialized.ToDictionary(x => x.actionName);
	}

	public InputBinding GetBinding(string actionName)
	{
		if (binding.TryGetValue(actionName, out var inputBinding) == false)
		{
			Debug.LogError($"Cannot find actionName '{actionName}' in inputMap '{name}'.");
			return null;
		}
		return inputBinding;
	}

	public bool IsDisable(InputBinding binding) => disabledBindings.Contains(binding);

	public void SetDisable(InputBinding inputBinding, bool isOn)
	{
		if (inputBinding == null)
			throw new ArgumentNullException(nameof(inputBinding));
		disabledBuffer[inputBinding] = isOn;
		if (isOn)
			disabledBindings.Add(inputBinding);
		else
			disabledBindings.Remove(inputBinding);
	}

	public void SetAction(InputBinding inputBinding, InputAction inputAction)
	{
		if (inputBinding == null)
			throw new ArgumentNullException(nameof(inputBinding));
		if (inputAction == null)
			throw new ArgumentNullException(nameof(inputAction));

		actions[inputBinding] = inputAction;
	}

	public void IncludeMap(InputMap inputMap)
	{
		if (inputMap == this)
			throw new ArgumentException("Cannot include itself.");
		if (inputMap.includeMaps.Contains(this))
			throw new InvalidOperationException("Circular reference detected.");
		includeMaps.Add(inputMap);
	}

	public void Update()
	{
		foreach (var (binding, action) in actions)
		{
			if (action == null)
				continue;
			Execute(binding, action);
		}
		foreach (var inputState in includeMaps)
		{
			foreach (var (binding, action) in inputState.actions)
			{
				if (action == null)
					continue;
				Execute(binding, action);
			}
		}
	}

	void Execute(InputBinding binding, InputAction inputAction)
	{
		if (disabledBuffer.ContainsKey(binding))
		{
			if (disabledBuffer[binding])
				inputAction.Cancel();
			else
				inputAction.Refresh(binding.KeyCode);
			disabledBuffer.Remove(binding);
			return;
		}
		if (disabledBindings.Contains(binding))
			return;
		inputAction.Update(binding.KeyCode);
	}

	public void Swap(InputMap buffer)
	{
		foreach (var action in actions.Values.Except(buffer.actions.Values))
		{
			if (action == null)
				continue;
			action.Cancel();
		}

		var actionSet = new HashSet<InputAction>(actions.Values);
		foreach (var (binding, action) in buffer.actions.Where(x => actionSet.Contains(x.Value) == false))
		{
			if (action == null || action.IsRefreshed == false)
				continue;
			action.Refresh(binding.KeyCode);
		}
	}
}