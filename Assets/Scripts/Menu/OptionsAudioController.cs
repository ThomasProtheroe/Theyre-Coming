using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsAudioController : MonoBehaviour {
	public GameObject keybindingsMenu;
	public GameObject optionsMenu;
	public GameObject mainMenu;

	public Slider masterSlider;
	public Slider musicSlider;
	public Slider effectsSlider;

	public AudioSource musicSource;

	public AudioClip buttonClick;
	public AudioClip buttonHover;
	[SerializeField]
	private int sourceMax;
	private AudioSource[] buttonSources;

	void Start() {
		if (!PlayerPrefs.HasKey ("masterVolume")) {
			PlayerPrefs.SetFloat ("masterVolume", 1.0f);
		}

		if (!PlayerPrefs.HasKey ("musicVolume")) {
			PlayerPrefs.SetFloat ("masterVolume", 1.0f);
		}

		if (!PlayerPrefs.HasKey ("effectsVolume")) {
			PlayerPrefs.SetFloat ("masterVolume", 1.0f);
		}

		//Set up all audio sources
		buttonSources = new AudioSource[sourceMax];
		for (int i = 0; i < sourceMax; i++) {
			buttonSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
		}

		masterSlider.value = PlayerPrefs.GetFloat ("masterVolume");
		musicSlider.value = PlayerPrefs.GetFloat ("musicVolume");
		effectsSlider.value = PlayerPrefs.GetFloat ("effectsVolume");
	}

	public void playHover() {
		for (int i = 0; i < buttonSources.Length; i++) {
			if (buttonSources [i].isPlaying) {
				continue;
			}

			buttonSources [i].PlayOneShot (buttonHover);
			break;
		}
		//If we get to this point without playing the clip, then all sources are full and we ignore the play request
	}

	public void playClick() {
		for (int i = 0; i < buttonSources.Length; i++) {
			if (buttonSources [i].isPlaying) {
				continue;
			}

			buttonSources [i].PlayOneShot (buttonClick);
			break;
		}
		//If we get to this point without playing the clip, then all sources are full and we ignore the play request
	}

	public void previewAudioSettings() {
		//Preview effects volume
		if (buttonSources != null) {
			foreach (AudioSource source in buttonSources) {
				source.volume = masterSlider.value * effectsSlider.value;
			}
		}
			
		//Preview music volume
		musicSource.volume = masterSlider.value * musicSlider.value;
	}

	public void saveAudioSettings() {
		PlayerPrefs.SetFloat ("masterVolume", masterSlider.value);
		PlayerPrefs.SetFloat ("musicVolume", musicSlider.value);
		PlayerPrefs.SetFloat ("effectsVolume", effectsSlider.value);
	}

	public void showKeybindingsMenu() {
		keybindingsMenu.SetActive (true);
		optionsMenu.SetActive (false);
		mainMenu.SetActive (false);
	}

	public void showOptionsMenu() {
		keybindingsMenu.SetActive (false);
		optionsMenu.SetActive (true);
		mainMenu.SetActive (false);
	}

	public void showMainMenu() {
		saveAudioSettings ();
		keybindingsMenu.SetActive (false);
		optionsMenu.SetActive (false);
		mainMenu.SetActive (true);
	}
}
