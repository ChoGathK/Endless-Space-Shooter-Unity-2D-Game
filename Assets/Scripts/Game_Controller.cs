using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Game_Controller : MonoBehaviour {

	[Header("Factory Element")]
	public static Game_Controller instance;

	[Header("GUI Elements")]
	public GameObject pausePanel;
	public GameObject resumeGameButton;
	public GameObject pausePanelScoreTitle;
	public Text pausePanelMainTitle;
	public Text centerMessage;
	public Text healthText;
	public Text pointsText;
	public Text timeText;
	public GameObject topCenter;
	public Scrollbar healthBar;
	public GameObject[] livesObject;
	public Slider volumeChange;

	[Header("Game Constants")]
	private float gameTimeScale;
	private int maxLife = 3;
	private int elapsedLife = 3;
	public int healthValue = 100;
	public int userPoints = 0;
	private int stepHealthValDec = 20;
	private float stepHealthDec = 0.2f;
	public Camera gameCamera;
	public Bounds gameBounds;
	public GameObject musicManagerObject;
	public enum SpecialFeatureList {superShooter = 0, fasterShooter = 1, immortalShooter = 2, doubleShooter = 3, ghostShooter = 4};

	[Header("Game Sounds")]
	public AudioClip gameIsOverSound;
	public AudioClip levelPassedSound;
	public AudioClip gameFinishedSound;

	[Header("Situations")]
	public bool isLevelPassed = false;
	public bool isPaused = false;
	public bool isDead = false;
	public bool isStatusBusy = false;

	[Header("Prefabs")]
	public GameObject explosionPrefab;
	public GameObject pointEffectPrefab;
	public GameObject lifeEffectPrefab;

	[Header("Temporary Variables")]
	public AudioSource tCompAudioSource;
	public float tTime;


	void Awake () {
		if (instance != null) {
			Debug.Log ("More than one Game Controller in scene!");
			DestroyImmediate (gameObject);
			return;
		}

		initializeGame ();

		instance = this;
		DontDestroyOnLoad (gameObject);
	}

	void Update () {
		if(isDead != true) {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				if(isPaused == false) {
					pauseGame ();
				}
				else {
					resumeGame ();
				}
			}
			if(isPaused != true) {
				tTime += Time.deltaTime;
				timeText.text = ((int)(tTime)).ToString ();
			}
		}
	}
		

	void initializeGame() {//OYUUN YENIDEN BASLAMASINI TEKRAR KONTROL ET
		Time.timeScale = 1.0f;
		gameTimeScale = Time.timeScale;
		elapsedLife = maxLife;
		healthValue = 100;
		userPoints = 0;
		tTime = 0f;

		isLevelPassed = false;
		isPaused = false;
		isDead = false;
		isStatusBusy = false;

		musicManagerObject = GameObject.Find ("_MM");

		if(musicManagerObject != null) {
			volumeChange.onValueChanged.AddListener(musicManagerObject.GetComponent<Music_Controller> ().changeVolume);
			volumeChange.value = musicManagerObject.GetComponent<AudioSource> ().volume;
		}
		OrthographicBounds ();

		StartCoroutine (fadeOutMessage (centerMessage.color, centerMessage.fontSize, null, 12));
	}

	void pauseGame() {
		resumeGameButton.SetActive (true);
		pausePanelMainTitle.text = "PAUSED";
		pausePanelScoreTitle.SetActive (false);
		pausePanel.SetActive (true);
		Time.timeScale = 0f;
		isPaused = true;
	}

	public void resumeGame() {
		pausePanel.SetActive (false);
		centerMessage.enabled = false;
		isPaused = false;
		Time.timeScale = gameTimeScale;
	}

	private void OrthographicBounds() {
		float verticalHeightSeen = gameCamera.orthographicSize * 2.0f;
		float verticalWidthSeen = verticalHeightSeen * gameCamera.aspect;
		gameBounds = new Bounds(gameCamera.transform.position, new Vector3(verticalWidthSeen, verticalHeightSeen, 0));
	}

	public bool deadInitialize() {
		bool retVal = false;
		if(elapsedLife != 0) {
			elapsedLife--;
			initializeNewLife ();
			retVal = false;
		}
		else {
			GetComponent<AudioSource>().Stop ();
			
			tCompAudioSource.clip = gameFinishedSound;
			tCompAudioSource.Play ();

			FinishGame ();

			resumeGameButton.SetActive (false);
			pausePanelScoreTitle.SetActive (true);
			pausePanelMainTitle.text = "FINISHED";
			pausePanelScoreTitle.GetComponent<Text>().text = "Score : "+userPoints.ToString ();

			Time.timeScale = 0f;

			isDead = true;
			retVal = true;

		}

		return retVal;
	}


	public void gainLifeOrPoint() {
		
		if(elapsedLife == maxLife) {
			if(healthValue == 100) {
				gainPoints (100);
			}
			else {
				healthValue = 100;
				StartCoroutine (fadeOutMessage (Color.red, 30, "Your HP 100", 20));
				changeHealthBar (1);
			}
		}
		else {
			increaseLives ();

			StartCoroutine (fadeOutMessage (Color.red, 30, "+1 Life UP",  20));

			elapsedLife++;
		}
	}

	void initializeNewLife() {
		healthValue = 100;
		healthBar.size = 1.0f;
		healthBar.interactable = true;
		healthText.text = healthValue.ToString ();

		decreaseLives ();
	}

	public void initializeSpecialFeature(Game_Controller.SpecialFeatureList featureID, bool status) {
		topCenter.transform.GetChild ((int)featureID).gameObject.SetActive (status);
	}

	public IEnumerator fadeOutMessage(Color nColor,int fSize, string message, int fadeStep) {
		centerMessage.enabled = true;

		if(message!=null) {
			centerMessage.text = message;
		}
		Color OldColor = centerMessage.color;
		float alpha = 1.0f;
		float fadeSpeed = 1f/fadeStep;

		centerMessage.fontSize = fSize;
		int oldFontSize = centerMessage.fontSize;
		for(int i=0; i < (int)fadeStep;i++) {
			alpha -= (fadeSpeed);
			centerMessage.color = new Color (nColor.r, nColor.g, nColor.b, alpha);
			yield return new WaitForSeconds (0.08f);
		}
		centerMessage.fontSize = oldFontSize;
		centerMessage.color = OldColor;
		centerMessage.enabled = false;
	}

	void increaseLives() {
		
		for(int i=elapsedLife;i<elapsedLife+1;i++) {
			livesObject [i].SetActive (true);
		}
	}

	void decreaseLives() {
		
		for(int i=elapsedLife;i<livesObject.Length;i++) {
			livesObject [i].SetActive (false);
		}
	}

	public void gainPoints(int point) {
		userPoints += point;
		//Game_Controller.totalPoints += point;
		changeScoreBoards ();
	}

	public void changeScoreBoards() {
		pointsText.text = userPoints.ToString ();
	}

	public void changeHealthBar (int mode) {
		if(isDead == true) {
			return;
		}

		if(mode == 1) {
			healthBar.size  = 1.0f;
			healthText.text = healthValue.ToString ();
		}
		else {
			if(healthValue > stepHealthValDec) {
				healthBar.size  -= stepHealthDec;
				healthValue -= stepHealthValDec;
				healthText.text = healthValue.ToString ();
			}
			else {
				healthValue = 0;
				healthBar.size  = 0f;
				healthBar.interactable = false;
				healthText.text = healthValue.ToString ();
			}
		}
	}

	public void FinishGame() {
		pausePanel.SetActive (true);
		//Level_Controller.instance.loadLevel(0);
	}

	void OnDestroy() {
		//Destroy(musicManagerObject);
	}
}
