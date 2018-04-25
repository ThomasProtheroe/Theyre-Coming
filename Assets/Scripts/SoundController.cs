using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	[SerializeField]
	private int queuedSourceMax;
	[SerializeField]
	private int oneShotSouceMax;

	private AudioSource[] enemyWalkSources;
	private AudioSource[] enemyProwlSources;
	private AudioSource[] enemyOneShotSources;
	private AudioSource[] priorityOneShotSources;

	private List<AudioClip> walkQueue = new List<AudioClip>();
	private List<AudioClip> prowlQueue = new List<AudioClip>();

	// Use this for initialization
	void Start () {
		enemyWalkSources = new AudioSource[queuedSourceMax];
		enemyProwlSources = new AudioSource[queuedSourceMax];
		enemyOneShotSources = new AudioSource[oneShotSouceMax];
		priorityOneShotSources = new AudioSource[oneShotSouceMax];

		for (int i = 0; i < queuedSourceMax; i++) {
			enemyWalkSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
			enemyProwlSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;

			enemyWalkSources [i].loop = true;
			enemyProwlSources [i].loop = true;
		}

		for (int i = 0; i < oneShotSouceMax; i++) {
			enemyOneShotSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
			priorityOneShotSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playEnemyWalk(AudioClip request) {
		bool played = false;
		for (int i = 0; i < enemyWalkSources.Length; i++) {
			if (enemyWalkSources [i].isPlaying) {
				continue;
			}

			enemyWalkSources [i].clip = request;
			enemyWalkSources [i].Play ();
			played = true;
			break;
		}

		//If we didn't have a free source to play the clip, queue it 
		if (!played) {
			queueRequest (request, walkQueue);
		}
	}

	public void stopEnemyWalk(AudioClip clip) {
		bool found = false;
		for (int i = 0; i < enemyWalkSources.Length; i++) {
			//We don't actually care if we ge tthe same source the request is playing on, as long as it is the same clip
			if (enemyWalkSources [i].isPlaying && enemyWalkSources [i].clip == clip) {
				found = true;
				enemyWalkSources [i].Stop ();
				enemyWalkSources [i].clip = null;

				playNextQueued (walkQueue, enemyWalkSources [i]);
				break;
			}
		}

		if (!found) {
			for (int i = 0; i < walkQueue.Count; i++) {
				if (walkQueue [i] != clip) {
					continue;
				}

				walkQueue.RemoveAt (i);
				break;
			}
		}
	}

	public void playEnemyProwl(AudioClip request) {
		bool played = false;
		for (int i = 0; i < enemyProwlSources.Length; i++) {
			if (enemyProwlSources [i].isPlaying) {
				continue;
			}

			enemyProwlSources [i].clip = request;
			enemyProwlSources [i].Play ();
			played = true;
			break;
		}

		//If we didn't have a free source to play the clip, queue it 
		if (!played) {
			queueRequest (request, prowlQueue);
		}
	}

	public void stopEnemyProwl(AudioClip clip) {
		bool found = false;
		for (int i = 0; i < enemyProwlSources.Length; i++) {
			//We don't actually care if we ge tthe same source the request is playing on, as long as it is the same clip
			if (enemyProwlSources [i].isPlaying && enemyProwlSources [i].clip == clip) {
				found = true;
				enemyProwlSources [i].Stop ();
				enemyProwlSources [i].clip = null;

				playNextQueued (prowlQueue, enemyProwlSources [i]);
				break;
			}
		}

		if (!found) {
			for (int i = 0; i < prowlQueue.Count; i++) {
				if (prowlQueue [i] != clip) {
					continue;
				}

				prowlQueue.RemoveAt (i);
				break;
			}
		}
	}

	public void playEnemyOneShot(AudioClip requestedClip) {
		for (int i = 0; i < enemyOneShotSources.Length; i++) {
			if (enemyOneShotSources [i].isPlaying) {
				continue;
			}

			enemyOneShotSources [i].PlayOneShot (requestedClip);
			break;
		}
		//If we get to this point without playing the clip, then all sources are full and we ignore the play request
	}

	public void playPriorityOneShot(AudioClip requestedClip) {
		for (int i = 0; i < priorityOneShotSources.Length; i++) {
			if (priorityOneShotSources [i].isPlaying) {
				continue;
			}

			priorityOneShotSources [i].PlayOneShot (requestedClip);
			break;
		}
		//If we get to this point without playing the clip, then all sources are full and we ignore the play request
	}
		
	private void queueRequest(AudioClip request, List<AudioClip> queue) {
		queue.Add (request);
	}

	private void playNextQueued(List<AudioClip> queue, AudioSource source) {
		if (queue.Count == 0) {
			//queue is empty
			return;
		}
	
		source.clip = queue [0];
		source.Play ();
		queue.RemoveAt (0);
	}

}