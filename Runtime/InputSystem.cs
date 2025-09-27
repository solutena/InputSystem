using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InputSystem : MonoBehaviour
{
	internal static List<InputMap> Prevs { get; private set; } = new();
	internal static InputMap Current { get; private set; }
	static InputMap Buffer { get; set; } = null;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void AfterSceneLoad()
	{
		new GameObject("InputSystem", typeof(InputSystem));
	}

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		if (Current != null)
			Current.Update();
		if (Buffer != null)
		{
			if (Current != null)
				Current.Swap(Buffer);
			Current = Buffer;
			Buffer = null;
		}
	}

	public static void PrevInputMap(InputMap inputMap)
	{
		if (Prevs.Remove(inputMap) == false)
		{
			Debug.LogError($"Prevs 에서 {inputMap.name} 삭제하지 못했습니다");
			return;
		}
		Buffer = Prevs.LastOrDefault();
	}

	public static void SetInputMap(InputMap inputMap)
	{
		Prevs.Remove(inputMap);
		Prevs.Add(inputMap);
		Buffer = inputMap;
	}
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(InputSystem))]
public class InputManagerEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (InputSystem.Current != null)
		{
			GUILayout.BeginVertical("HelpBox");
			{
				GUILayout.Label(InputSystem.Current.name);
				foreach (var binding in InputSystem.Current.actions.Keys)
				{
					GUIStyle style = new();
					style.normal.textColor = InputSystem.Current.IsDisable(binding) ? Color.red : Color.green;
					GUILayout.Label($"{binding.actionName}		{binding.KeyCode}", style);
				}
			}
			foreach (var inputMap in InputSystem.Current.includeMaps)
			{
				GUILayout.Label(inputMap.name);
				foreach (var binding in inputMap.actions.Keys)
				{
					GUIStyle style = new();
					style.normal.textColor = InputSystem.Current.IsDisable(binding) ? Color.red : Color.green;
					GUILayout.Label($"{binding.actionName}		{binding.KeyCode}", style);
				}
			}
			GUILayout.EndVertical();
		}
		if (InputSystem.Prevs != null)
		{
			GUILayout.Label("Prevs");
			GUILayout.BeginVertical("HelpBox");
			foreach (var inputState in InputSystem.Prevs)
				GUILayout.Label(inputState.name);
			GUILayout.EndVertical();
		}
		Repaint();
	}
}
#endif