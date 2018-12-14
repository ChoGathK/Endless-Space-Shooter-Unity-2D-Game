using UnityEngine;
//using System.Collections;
using UnityEngine.SceneManagement;

public class Level_Controller : MonoBehaviour {
	
	[Header("Factory Element")]
	public static Level_Controller instance;

	[Header("Object Variables")]
	public GameObject gmObject;
	private Scene lastScene;

	void Awake () {
		if (instance != null) {
			Debug.Log ("More than one Game Controller in scene!");
			DestroyImmediate (gameObject);
			return;
		}

		initializeObject ();

		instance = this;
		DontDestroyOnLoad (gameObject);
	}

	void initializeObject() {
		SceneManager.sceneLoaded += isSceneLoaded;
		SceneManager.activeSceneChanged += isActiveSceneChanged;
	}

	void Update () {
	
	}

	public void loadLevel(int levelIndex) {
		SceneManager.LoadScene (levelIndex);
	}

	public void restartScene() {
		destroyGM ();

		int scene = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}

	void isSceneLoaded(Scene loadedScene, LoadSceneMode y) {
		if(loadedScene.name == lastScene.name) {
			
		}
		loadedScene = lastScene;
	}

	void isActiveSceneChanged(Scene x, Scene loadedScene) {

		if(loadedScene.buildIndex == 0) {
			gmObject = GameObject.Find ("_GM");
			if(gmObject != null) {
				Debug.Log ("bunu yoket");
				//Destroy (gmObject);
			}
		}
	}

	void destroyGM() {
		gmObject = GameObject.Find ("_GM");
		if(gmObject != null) {
			Destroy (gmObject);
		}
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded -= isSceneLoaded;
		SceneManager.activeSceneChanged -= isActiveSceneChanged;
	}
}
