using System;
using System.Linq;
using Godot;
using Godot.Collections;

public class JSONUtils {
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
		}
		return data;
	}
}
