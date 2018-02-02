using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Scenes {

	private static Dictionary<string, string> parameters;

	public static AsyncOperation Load(string sceneName, Dictionary<string, string> parameters = null) {
		Scenes.parameters = parameters;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;
		return asyncLoad;
	}

	public static AsyncOperation Load(string sceneName, string paramKey, string paramValue) {
		Scenes.parameters = new Dictionary<string, string>();
		Scenes.parameters.Add(paramKey, paramValue);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;
		return asyncLoad;
	}

	public static Dictionary<string, string> getSceneParameters() {
		return parameters;
	}

	public static string getParam(string paramKey) {
		if (parameters == null) return "";
		return parameters[paramKey];
	}

	public static void setParam(string paramKey, string paramValue) {
		if (parameters == null)
			Scenes.parameters = new Dictionary<string, string>();
		Scenes.parameters.Add(paramKey, paramValue);
	}

}
