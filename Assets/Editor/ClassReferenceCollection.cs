using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;

public class ClassReferenceCollection 
{
	public Dictionary<System.Type, string> codeFileList = new Dictionary<System.Type, string>();
	public Dictionary<string, List<System.Type>> references = new Dictionary<string, List<System.Type>>();

	public void Collection()
	{
		codeFileList.Clear();
		references.Clear();

		List<System.Type> alltypes = new List<System.Type>();

		if( File.Exists("Library/ScriptAssemblies/Assembly-CSharp.dll"))
			alltypes.AddRange(Assembly.LoadFile ("Library/ScriptAssemblies/Assembly-CSharp.dll").GetTypes ());
		if( File.Exists("Library/ScriptAssemblies/Assembly-CSharp-Editor.dll"))
			alltypes.AddRange(Assembly.LoadFile ("Library/ScriptAssemblies/Assembly-CSharp-Editor.dll").GetTypes ());
		if( File.Exists("Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll"))
			alltypes.AddRange(Assembly.LoadFile ("Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll").GetTypes ());

		alltypes = alltypes	.Where(item => item.IsClass || item.IsInterface).ToList();

		var codes = Directory.GetFiles ("Assets", "*.cs", SearchOption.AllDirectories);
		foreach (var codePath in codes) 
		{
			var code = System.IO.File.ReadAllText(codePath);
			foreach( var type in alltypes ){

				if( codeFileList.ContainsKey(type ))
					continue;

				if( string.IsNullOrEmpty( type.Namespace ) == false ){
					var namespacepattern = string.Format("namespace[\\s.]{0}[{{\\s\\n]",type.Namespace );
					if( Regex.IsMatch( code, namespacepattern ) == false ){
						continue;
					}
				}

				var pattern = string.Format("class[\\s.]{0}[\\s\\n:{{]",type.Name );
				if( Regex.IsMatch(code, pattern) ){
					codeFileList.Add(type, codePath);
				}
			}
		}


		foreach( var codepath in codes ){
			CheckCode(codepath, alltypes);
		}
	}


	void CheckCode(string codePath, List<System.Type> alltypes)
	{
		if(codePath == null || references.ContainsKey(codePath)){
			return;
		}

		var code = System.IO.File.ReadAllText(codePath);
		var list =  new List<System.Type>();
		references[codePath] = list;
	
		foreach( var type in alltypes ){


			if( string.IsNullOrEmpty(type.Namespace) == false){
				var namespacepattern = string.Format("[namespace|using][\\s\\.]{0}[{{\\s\\n;]",type.Namespace );
				if( Regex.IsMatch( code, namespacepattern ) == false ){
					continue;
				}
			}


			string match = string.Format("[\\]\\.\\s<(]{0}[\\.\\s\\n>,(){{]", type.Name);
			if( Regex.IsMatch(code, match) ){
				list.Add(type);
				var path = codeFileList[type];
				CheckCode(path, alltypes);
			}
		}
	}
}
