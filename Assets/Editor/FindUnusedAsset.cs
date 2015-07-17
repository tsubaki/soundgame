using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

public class FindUnusedAsset : EditorWindow
{
	AssetCorrector corrector = new AssetCorrector();

	Vector2 scroll;

	[MenuItem("Window/Assets")]
	static void Init()
	{
		var window = FindUnusedAsset.CreateInstance<FindUnusedAsset> ();
		window.Show ();
		window.corrector.Collection ();
	}

	void OnGUI()
	{
		if (GUILayout.Button ("Delete")) {
			RemoveFiles();
			Close ();
		}

		scroll = EditorGUILayout.BeginScrollView (scroll);

		foreach (var guid in corrector.uuidList) {
			EditorGUILayout.BeginHorizontal();
			if( GUILayout.Button("", GUILayout.Width(15)) ){
				corrector.uuidList.Remove(guid);
			}
			EditorGUILayout.LabelField(AssetDatabase.GUIDToAssetPath( guid ));
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView ();
	}

	void RemoveFiles()
	{
		try{
			var files = corrector.uuidList.Select (guid => AssetDatabase.GUIDToAssetPath (guid)).ToArray ();
			//EditorUtility.DisplayProgressBar("export package", "", 0);
			//AssetDatabase.ExportPackage (files, "backuppackage.unitypackage");

			int i=0;
			foreach( var file in files ){
				i++;
				EditorUtility.DisplayProgressBar("delete unused assets",file, (float)i/files.Length );
				AssetDatabase.DeleteAsset( file);
			}

			EditorUtility.DisplayProgressBar("clean directory", "", 1);
			foreach( var dir in Directory.GetDirectories("Assets") )
			{
				RemoveEmptyDirectry(dir);
			}

			AssetDatabase.Refresh();

		}finally{
			EditorUtility.ClearProgressBar();
		}
	}

	void RemoveEmptyDirectry(string path)
	{
		var dirs = Directory.GetDirectories(path);
		foreach( var dir in dirs ){
			RemoveEmptyDirectry(dir);
		}

		if( Directory.GetFiles(path, "*.*").Any(item => Path.GetExtension(item) != ".meta") == false && 
		    Directory.GetDirectories(path).Count() == 0){
			UnityEditor.FileUtil.DeleteFileOrDirectory(path);
		}
	
	}
}
