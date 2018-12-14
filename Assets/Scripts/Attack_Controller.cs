using UnityEngine;
using System.Collections;
using System.Linq;
using System.Xml.Linq;

public class Attack_Controller : MonoBehaviour {

	[Header("Attack Variables")]
	private Transform[] attackStartPoints;
	private Transform[] attackEndPoints;
	private int startPointSize;
	private int endPointSize;
	private float distanceBetweenPoints = 0.8f;

	private int attackCounter = 0;
	private int attackLimit = 2;
	private int lifeCounter = 15;
	private int pointCounter = 5;
	private int specialFeatureCounter = 8;

	private int specialFeatureLimit = 8;
	private int pointLimit = 5;
	private int lifeLimit = 15;

	public Transform mainObjectStartTransform;
	public Transform mainObjectEndTransform;

	[Header("Timing")]
	private float attackWaitTime = 3.0f;
	private float attackCountDown = 3.0f ;

	[Header("Prefabs")]
	public GameObject meteorPrefab;
	public GameObject pointPrefab;
	public GameObject lifePrefab;
	public GameObject specialFeaturePrefab;
	public GameObject[] enemiesPrefab;
	public GameObject startEndPointsPrefab;

	[Header("Spawned Objects")]
	public Transform[] spawnedEnemies;
	public Transform[] spawnedMeteors;
	public Transform[] spawnedPoints;
	public Transform[] spawnedLives;
	public Transform[] spawnedSpecialFeatures;

	[Header("Temporary Variables")]
	private Vector3 tSpawnPos;
	private GameObject tObject;
	private int tStartIndex;
	private int tEndIndex;
	private int tRIndex;

	void Awake() {
		initialStartEndPoints ();
		initializeSpawnPoints ();
	}

	void initialStartEndPoints() {
		int totalPoints = (int)(Mathf.Round((Game_Controller.instance.gameBounds.max.x - Game_Controller.instance.gameBounds.min.x)/distanceBetweenPoints));

		for(int i=0;i<totalPoints;i++) {
			tSpawnPos = new Vector3(Game_Controller.instance.gameBounds.min.x+0.5f+(distanceBetweenPoints*i), Game_Controller.instance.gameBounds.max.y+0.3f, startEndPointsPrefab.transform.position.z);
			tObject = (GameObject)Instantiate (startEndPointsPrefab, tSpawnPos, startEndPointsPrefab.transform.rotation);
			tObject.transform.parent = mainObjectStartTransform;

			tSpawnPos = new Vector3(Game_Controller.instance.gameBounds.min.x+0.5f+(distanceBetweenPoints*i), Game_Controller.instance.gameBounds.min.y-0.3f, startEndPointsPrefab.transform.position.z);
			tObject = (GameObject)Instantiate (startEndPointsPrefab, tSpawnPos, startEndPointsPrefab.transform.rotation);
			tObject.transform.parent = mainObjectEndTransform;
		}
		//Instantiate ();
	}

	void Update () {
		if(attackCountDown <= 0.0f) {
			StartCoroutine(startAttack());
			attackCountDown = attackWaitTime;
		}
		attackCountDown -= Time.deltaTime;
	}

	void initializeSpawnPoints() {
		startPointSize = mainObjectStartTransform.childCount;
		attackStartPoints = new Transform[startPointSize];
		for(int i=0; i<startPointSize; i++) {
			attackStartPoints [i] = mainObjectStartTransform.GetChild (i);
		}

		endPointSize = mainObjectEndTransform.childCount;
		attackEndPoints = new Transform[endPointSize];
		for(int i=0; i<endPointSize; i++) {
			attackEndPoints [i] = mainObjectEndTransform.GetChild (i);
		}
	}


	IEnumerator startAttack() {

		if(attackCounter<=attackLimit) {
			attackCounter++;
		}

		spawnedEnemies = new Transform[attackCounter];
		spawnedMeteors = new Transform[attackCounter];
		spawnedPoints = new Transform[attackCounter];
		spawnedLives = new Transform[1];
		spawnedSpecialFeatures = new Transform[1];


		for(int i = 0; i< attackCounter; i++) {
			sendMeteor(i); 
			sendEnemy(i); 
			yield return new WaitForSeconds (0.5f);
		}

		if(pointCounter == pointLimit) {
			for(int i = 0; i< attackCounter; i++) {
				sendPoint(i); 
				yield return new WaitForSeconds (0.5f);
			}

			pointCounter = 0;
		}
		else {
			pointCounter++;
		}

		if(lifeCounter == lifeLimit) {
			if(spawnedLives[0] == null) {
				for(int i = 0; i< 1; i++) {
					sendLife(i); 
				}
			}
			lifeCounter = 0;
		}
		else {
			lifeCounter++;
		}

		if(specialFeatureCounter == specialFeatureLimit) {
			if(spawnedSpecialFeatures[0] == null) {
				for(int i = 0; i< 1; i++) {
					sendSpecialFeature(i); 
				}
			}
			specialFeatureCounter = 0;
		}
		else {
			specialFeatureCounter++;
		}




	}

	void sendLife(int eIndex) {
		tStartIndex  = Random.Range (0, startPointSize);
		tEndIndex  = Random.Range (0, endPointSize);

		tSpawnPos = new Vector3 (attackStartPoints [tStartIndex].position.x, attackStartPoints [tStartIndex].position.y, lifePrefab.transform.position.z);

		tObject = (GameObject)Instantiate (lifePrefab, tSpawnPos, lifePrefab.transform.rotation);
		tObject.GetComponent<Life>().target = attackEndPoints[tEndIndex];
		spawnedLives[eIndex] = tObject.transform;
	}

	void sendPoint(int eIndex) {
		tStartIndex  = Random.Range (0, startPointSize);
		tEndIndex  = Random.Range (0, endPointSize);

		tSpawnPos = new Vector3 (attackStartPoints [tStartIndex].position.x, attackStartPoints [tStartIndex].position.y, pointPrefab.transform.position.z);

		tObject = (GameObject)Instantiate (pointPrefab, tSpawnPos, pointPrefab.transform.rotation);
		tObject.GetComponent<Points>().target = attackEndPoints[tEndIndex];
		spawnedPoints[eIndex] = tObject.transform;
	}

	void sendMeteor(int eIndex) {
		tStartIndex  = Random.Range (0, startPointSize);
		tEndIndex = Random.Range (0, endPointSize);

		tSpawnPos = new Vector3 (attackStartPoints [tStartIndex].position.x, attackStartPoints [tStartIndex].position.y, meteorPrefab.transform.position.z);

		tObject = (GameObject)Instantiate (meteorPrefab, tSpawnPos, meteorPrefab.transform.rotation);
		tObject.GetComponent<Meteor>().target = attackEndPoints[tEndIndex];
		spawnedMeteors [eIndex] = tObject.transform;
	}

	void sendEnemy(int eIndex) {
		tStartIndex  = Random.Range (0, startPointSize);
		tEndIndex  = Random.Range (0, endPointSize);
		tRIndex  = Random.Range (0, enemiesPrefab.Length);

		tSpawnPos = new Vector3 (attackStartPoints [tStartIndex].position.x, attackStartPoints [tStartIndex].position.y, enemiesPrefab[tRIndex].transform.position.z);

		tObject = (GameObject)Instantiate (enemiesPrefab[tRIndex], tSpawnPos, enemiesPrefab[tRIndex].transform.rotation);
		tObject.GetComponent<Enemy>().target = attackEndPoints[tEndIndex];
		spawnedEnemies [eIndex] = tObject.transform;
	}

	void sendSpecialFeature(int eIndex) {
		tStartIndex  = Random.Range (0, startPointSize);
		tEndIndex  = Random.Range (0, endPointSize);

		tSpawnPos = new Vector3 (attackStartPoints [tStartIndex].position.x, attackStartPoints [tStartIndex].position.y, specialFeaturePrefab.transform.position.z);

		tObject = (GameObject)Instantiate (specialFeaturePrefab, tSpawnPos, specialFeaturePrefab.transform.rotation);
		tObject.GetComponent<FeatureBall>().target = attackEndPoints[tEndIndex];
		spawnedSpecialFeatures[eIndex] = tObject.transform;
	}
}
