using System;
using System.Collections.Generic;
using Godot;
public class CSV<T> where T : ISavable, IRecord<string>, new() {

	public Dictionary<string, T> LoadFromFile(string filePath) {
		var file = new File();
		var db = new Dictionary<string, T>();
		if (!file.FileExists(filePath)) { 
			GD.Print(String.Format("No save file at '{0}'!", filePath));
			return db; // Error! We don't have a save to load.
		}
		file.Open(filePath, File.ModeFlags.Read);

		while (file.GetPosition() < file.GetLen()) {
			var data = new Godot.Collections.Dictionary<string, object>(
				(Godot.Collections.Dictionary)JSON.Parse(file.GetLine()).Result);
			var record = new T();
			record.Load(data);
			db.Add(record.GetKey(), record);
		}

		file.Close();
		return db;
	}
}
