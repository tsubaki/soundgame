using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class AssetCorrector
{
	public List<string> uuidList = new List<string>();

	ClassReferenceCollection classCollection = new ClassReferenceCollection();

	
	public void Collection()
	{
		uuidList.Clear();
		classCollection.Collection();

		// すべてのアセットを探す.
		var files = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
			.Where (item => Path.GetExtension (item) != ".meta")
			.Where (item => Path.GetExtension (item) != ".unity")
			.Where (item => Path.GetExtension (item) != ".shader")
			.Where (item => Path.GetExtension (item) != ".cg")
				.Where (item => Regex.IsMatch(item, "[\\/]Resources[\\/]" ) == false)
				.Where (item => Regex.IsMatch(item, "[\\/]Editor[\\/]" ) == false);
		foreach (var path in files) {
			var guid = AssetDatabase.AssetPathToGUID( path );
			uuidList.Add(guid);
		}

		var resourcesFiles = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
				.Where (item => item.IndexOf ("\\Resources\\") != -1)
				.Where (item => Path.GetExtension (item) != ".meta")
				.ToArray();
		foreach( var refs in AssetDatabase.GetDependencies (resourcesFiles) ){
			RemoveList(refs);
		}

		// シーンビューを登録
		var scenes = EditorBuildSettings.scenes
			.Where( item => item.enabled == true)
			.Select( item => item.path )
			.ToArray();
		foreach( var refs in AssetDatabase.GetDependencies (scenes) ){
			
			RemoveList(refs);
		} 
	}

	void RemoveList(string path){

		var guid = AssetDatabase.AssetPathToGUID(path);
		if( uuidList.Contains( guid ) == false ){
			return;
		}
		uuidList.Remove(guid);

		if( classCollection.references.ContainsKey(path) == false ){
			return;
		}

		foreach( var type in classCollection.references[path])
		{
			var codePath = classCollection.codeFileList[type];
			RemoveList(codePath);
		}
	}
}