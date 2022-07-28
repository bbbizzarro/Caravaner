using System;
using System.Linq;
using Godot;

public class JSONUtils {
    public static Godot.Collections.Dictionary<string, object> SerializeNode(object obj) { 
		var data = new Godot.Collections.Dictionary<string, object>();
		data["Filename"] = ((Node)obj).Filename;
		data["Parent"] = ((Node)obj).GetParent().GetPath();
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

    public static Godot.Collections.Dictionary<string, object> Serialize(object obj) { 
		var data = new Godot.Collections.Dictionary<string, object>();
		var fields = obj.GetType().GetFields().Where(field => Attribute.IsDefined(field, typeof(SerializeFieldAttribute)));
		foreach (System.Reflection.FieldInfo fieldInfo in fields) {
			data[fieldInfo.Name] = fieldInfo.GetValue(obj);
		}
		return data;
	}
}
