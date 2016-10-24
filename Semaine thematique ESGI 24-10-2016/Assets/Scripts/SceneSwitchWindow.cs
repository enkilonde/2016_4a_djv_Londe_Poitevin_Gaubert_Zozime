using UnityEngine;
using System.IO;
using System.Collections;
using System;
#if UNITY_EDITOR
public class SceneSwitchWindow : UnityEditor.EditorWindow
{


	[UnityEditor.MenuItem("Tools/SceneSwitcher")]
	internal static void CreateWindow()
	{
		SceneSwitchWindow Window = (SceneSwitchWindow)GetWindow(typeof(SceneSwitchWindow), false, "SceneSwitch");
	}


	internal void OnGUI()
	{
		//GUI de la fenetre ici
		UnityEditor.EditorGUILayout.BeginVertical();

		GUILayout.Label("Scenes In Build", UnityEditor.EditorStyles.boldLabel);
		for (var i = 0; i < UnityEditor.EditorBuildSettings.scenes.Length; i++)
		{
			var scene = UnityEditor.EditorBuildSettings.scenes[i];
			var sceneName = Path.GetFileNameWithoutExtension(scene.path);
			var pressed = GUILayout.Button(i + ": " + sceneName, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });

			if (pressed)
			{

				if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path);
				}

			}
		}
		UnityEditor.EditorGUILayout.EndVertical();
	}

}
#endif