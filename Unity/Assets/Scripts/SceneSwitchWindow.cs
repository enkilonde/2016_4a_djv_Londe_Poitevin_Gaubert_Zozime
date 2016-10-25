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

    [UnityEditor.MenuItem("Tools/Shortcuted/Snap Selected _g")]
    internal static void SnapSelected()
    {
        if (UnityEditor.Selection.transforms.Length == 0) return;
        if (UnityEditor.Selection.transforms[0].gameObject.tag != "Tiles") return;
        Vector3 posTemp = UnityEditor.Selection.transforms[0].position;
        UnityEditor.Selection.transforms[0].position = new Vector3((int)posTemp.x / 10 * 10 + 5, 0, (int)posTemp.z / 10 * 10 + 5);
    }

    [UnityEditor.MenuItem("Tools/Shortcuted/Fix Names _r")]
    internal static void testSelectedRot()
    {

        GameObject[] Tiles = GameObject.FindGameObjectsWithTag("Tiles");

        for (int i = 0; i < Tiles.Length; i++)
        {
            if (Tiles[i].name.Contains("straight")) Tiles[i].name = "roadtile_straight";

            if (Tiles[i].name.Contains("checkpoint")) Tiles[i].name = "roadtile_checkpoint";

            if (Tiles[i].name.Contains("curve")) Tiles[i].name = "roadtile_curve";
        }




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