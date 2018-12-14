using UnityEngine;
using System.Collections;

public class Music_Controller : MonoBehaviour {

	[Header("Factory Element")]
	public static Music_Controller instance;

	[Header("Object Variables")]
	private AudioSource gameMusic;

	void Awake () {

		if (instance != null) {
			Debug.Log ("More than one Music Manager in scene!");
			DestroyImmediate (gameObject);
			return;
		}

		gameMusic = GetComponent<AudioSource> ();

		instance = this;
		DontDestroyOnLoad (gameObject);
	}

	public void changeVolume(float newValue) {
		gameMusic.volume = newValue;
	}
}
