using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Dialog {

	public string text;
	public string character;
	public Sprite[] sprites;
	public float duration;
	public float delay;

	public Dialog(string text, string character="fiona", float duration=4.0f, float delay=0.0f) {
		this.character = character;
		this.text = text;
		this.duration = duration;
		this.delay = delay;
	}
}

public static class DialogList {
	private static Dictionary<string, ArrayList> dialogMaster;
	private static Dictionary<string, int> dialogIndex;

	static DialogList() {
		dialogMaster = new Dictionary<string, ArrayList> ();
		dialogIndex = new Dictionary<string, int> ();
	}

	public static Dialog GetNextDialog(string dialogType) {
		int currentIndex = dialogIndex[dialogType];

		if (!dialogMaster.ContainsKey (dialogType)) {
			return null;
		}

		ArrayList dialogList = dialogMaster[dialogType];

		if(dialogList.Count >= currentIndex) {
			return null;
		}

		dialogIndex[dialogType] = currentIndex + 1;

		return (Dialog) dialogList[currentIndex];
	}

	public static Dialog GetRandomDialog(string dialogType) {
		if (!dialogMaster.ContainsKey (dialogType)) {
			return null;
		}
		
		ArrayList dialogList = dialogMaster[dialogType];
		return (Dialog) dialogList[Random.Range(0, dialogList.Count)];
	}

	public static void addDialog(string dialogType, Dialog newDialog) {
		//If we have a dialog list of this type already, add to it
		ArrayList dialogList;
		if (dialogMaster.ContainsKey (dialogType)) {
			dialogList = dialogMaster[dialogType];
		} else {
			//Otherwise start a new list for this type
			dialogList = new ArrayList ();
			dialogIndex.Add(dialogType, 0);
		}

		dialogList.Add (newDialog);
		dialogMaster[dialogType] = dialogList;
	}

	public static void loadDialogs(string path) {
		string line = "";
		StreamReader reader = new StreamReader(path); 

		while((line = reader.ReadLine()) != null)  
		{  
			string[] columns = line.Split (',');
			
			Dialog newDialog = new Dialog (columns [1], columns [2]);
			addDialog (columns [0], newDialog);
		}  
	}
}
