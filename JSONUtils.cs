using System;
using System.Linq;
using Godot;
using Godot.Collections;

public class JSONUtils {
	//private static DeserializeArray(System.Reflection.FieldInfo fieldInfo, System.Object obj) { 
	//	Godot.Collections.Array arrayData = (Godot.Collections.Array)data[fieldInfo.Name];
	//	if (fieldInfo.FieldType.GenericTypeArguments[0] == typeof(int)) {
	//		var fieldList = new System.Collections.Generic.List<int>();
	//		foreach (float item in arrayData) { 
	//			fieldList.Add(Mathf.RoundToInt(item));
	//		}
	//	}
	//}

	public static Dictionary<string, object> ReadJSON(string data) {
		return new Dictionary<string, object>((Dictionary<string, object>)(JSON.Parse(data).Result));
	}

	public static void Deserialize(object obj, Dictionary<string, object> data) { 
		var fields = obj.GetType()
			.GetFields(System.Reflection.BindingFlags.Public 
					 | System.Reflection.BindingFlags.NonPublic
					 | System.Reflection.BindingFlags.Instance)
			.Where(field => Attribute.IsDefined(field, typeof(SerializeFieldAttribute)));
		foreach (System.Reflection.FieldInfo fieldInfo in fields) {
			if (data.ContainsKey(fieldInfo.Name)) {
				// Godot saves both ints and floats as System.Singles, aka floats.
				if (fieldInfo.FieldType == typeof(int)) 
					fieldInfo.SetValue(obj, Mathf.RoundToInt((float)data[fieldInfo.Name]));
				else if (data[fieldInfo.Name].GetType() == typeof(Godot.Collections.Array)) {
					Godot.Collections.Array arrayData = (Godot.Collections.Array)data[fieldInfo.Name];
					if (fieldInfo.FieldType.GenericTypeArguments[0] == typeof(int)) {
						var fieldList = new System.Collections.Generic.List<int>();
						foreach (float item in arrayData) { 
							fieldList.Add(Mathf.RoundToInt(item));
						}
						fieldInfo.SetValue(obj, fieldList);
					}
					else if (fieldInfo.FieldType.GenericTypeArguments[0] == typeof(float)) { 
						var fieldList = new System.Collections.Generic.List<float>();
						foreach (float item in arrayData) { 
							fieldList.Add(item);
						}
						fieldInfo.SetValue(obj, fieldList);
					}
				}
				else 
					fieldInfo.SetValue(obj, data[fieldInfo.Name]);
			}
		}

	}

	public static Godot.Collections.Dictionary<string, object> SerializeNode(object obj) {
		var data = Serialize(obj);
		// Add node header
		data["Filename"] = ((Node)obj).Filename;
		data["Parent"] = ((Node)obj).GetParent().GetPath();
		return data;
	}

	public static Godot.Collections.Dictionary<string, object> Serialize(object obj) { 
		var data = new Godot.Collections.Dictionary<string, object>();
		var fields = obj.GetType()
			.GetFields(System.Reflection.BindingFlags.Public 
					 | System.Reflection.BindingFlags.NonPublic
					 | System.Reflection.BindingFlags.Instance)
			.Where(field => Attribute.IsDefined(field, typeof(SerializeFieldAttribute)));
		foreach (System.Reflection.FieldInfo fieldInfo in fields) {
			data[fieldInfo.Name] = fieldInfo.GetValue(obj);
			//if (fieldInfo.FieldType == typeof(ISavable)) {
			//	data[fieldInfo.Name] = ((ISavable)fieldInfo.GetValue(obj)).Save();
			//}
			//else { 
			//	data[fieldInfo.Name] = fieldInfo.GetValue(obj);
			//}
		}
		return data;
	}
}
