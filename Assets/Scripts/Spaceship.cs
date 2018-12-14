using UnityEngine;
using System.Collections;
using System.Linq;

public class Spaceship : MonoBehaviour {

	[Header("Object Variables")]
	public Camera GameCamera;
	private float speed = 8f;
	private float currentSpeed = 8f;
	private bool isInvisible = false;
	public Transform firePoint;
	MeshRenderer meshOfShip;
	public MobileJoystick joystick;
    public MobileJoystick firestick;
    private Rigidbody controller;
	private Transform camTransform;

	[Header("Prefabs")]
	public GameObject bulletPrefab;

	[Header("Transform Variables")]
	public Vector3 newPosition;
	public Vector3 diffPos;
	private Vector3 firstLocalScale;

	[Header("Special Feature Variables")]
	private const int featureLimit = 6;
	private float featureTimeLimit = 20f;
	private Special_Features[] features;
	private bool isHaveSuperPower;
	private Vector3 oldBulletScale;
	private Transform bulletTransform;
	public GameObject shield;

	[Header("Audio Clips")]
	public AudioClip boomSound;
	public AudioClip laserSound;

	[Header("Temporary Variables")]
	private int tDamageCounter = 0;
	private Vector3 tfirePositionRight;
	private Vector3 tfirePositionLeft;
	private GameObject tBullet;
	private GameObject tBullet2;
	private int tCount;
	private Vector3 dir;
	private float fCounter;


	void Awake() {
		meshOfShip = GetComponent<MeshRenderer> ();
		firstLocalScale = transform.localScale;
		isInvisible = false;
		features = new Special_Features[featureLimit];
		for(int i = 0; i < featureLimit; i++) {
			features [i] = new Special_Features (((Game_Controller.SpecialFeatureList)i), 0f, featureTimeLimit, false);
		}
		bulletTransform = bulletPrefab.GetComponent<Transform>();
		oldBulletScale = bulletPrefab.GetComponent<Transform> ().localScale;

		camTransform = Camera.main.transform;
		controller = GetComponent<Rigidbody> ();
 	}

	void OnDrawGizmos()
	{
		//float verticalHeightSeen = GameCamera.orthographicSize * 2.0f;
		//float verticalWidthSeen = verticalHeightSeen * GameCamera.aspect;

		//Gizmos.color = Color.red;
		//Gizmos.DrawWireCube(GameCamera.transform.position, new Vector3(verticalWidthSeen, verticalHeightSeen, 0));

	}

	void OnApplicationQuit() {
		bulletTransform.localScale = oldBulletScale;
	}

	public void initializeNewFeatures(Game_Controller.SpecialFeatureList newFeature, float counter) {
		int nIndex = (int)newFeature;
		features [nIndex].IsActive = true;
		features [nIndex].Counter = counter;
		isHaveSuperPower = true;

		if (Game_Controller.SpecialFeatureList.superShooter == newFeature) {
			bulletTransform.localScale = new Vector3 (0.17f, 0.32f, 0.066f);
		}
		else if (Game_Controller.SpecialFeatureList.fasterShooter == newFeature) {
			speed = 9f;
		}
		else if (Game_Controller.SpecialFeatureList.ghostShooter == newFeature) {
			tDamageCounter = 0;
			shield.SetActive (true);
		}
		else if (Game_Controller.SpecialFeatureList.immortalShooter == newFeature) {
			shield.SetActive (true);
		}

		Game_Controller.instance.initializeSpecialFeature(newFeature, true);
	}

	bool inBoundsControl(Vector3 pos) {
		return ((Game_Controller.instance.gameBounds.min.x+(Mathf.Abs (meshOfShip.bounds.min.x)+meshOfShip.bounds.max.x)) < pos.x && (Game_Controller.instance.gameBounds.max.x+meshOfShip.bounds.max.x-meshOfShip.bounds.min.x) > pos.x) ? ((Game_Controller.instance.gameBounds.min.y < pos.y && Game_Controller.instance.gameBounds.max.y > pos.y) ? true : false) : false;
	}


	void Update () {

		if(isInvisible == false && Game_Controller.instance.isPaused == false && Game_Controller.instance.isDead == false) {


			if(isHaveSuperPower == true) {
				var tFeature = from f in features where f.IsActive == true select f;
				tCount = tFeature.Count<Special_Features> ();
				if (tCount  > 0) {
					foreach (Special_Features sF in tFeature) {

						if(sF.Counter > sF.Limit) {
							sF.Counter = 0f;
							sF.IsActive = false;
							Game_Controller.instance.initializeSpecialFeature (sF.Feature, false);

							if (Game_Controller.SpecialFeatureList.superShooter == (sF.Feature)){
								bulletTransform.localScale = oldBulletScale;
							}
							else if (Game_Controller.SpecialFeatureList.fasterShooter == (sF.Feature)){
								speed = currentSpeed;
							}
							else if (Game_Controller.SpecialFeatureList.ghostShooter == (sF.Feature)){
								shield.SetActive (false);
							}
							else if (Game_Controller.SpecialFeatureList.immortalShooter == (sF.Feature)){
								shield.SetActive (false);
							}
						}
						else {
							sF.Counter += Time.deltaTime;
						}
					}
				}
				else {
					isHaveSuperPower = false;
				}

			}

			if(joystick.enabled == true && joystick.position != Vector2.zero) {
				dir= new Vector3(joystick.position.x, joystick.position.y, 0f);
			}
			else {
				dir= new Vector3(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0f);
			}


			if(dir.magnitude > 1) {
				dir.Normalize ();
			}

			newPosition = camTransform.TransformDirection (dir);
			newPosition = new Vector3 (newPosition.x, newPosition.y, 0f);
			newPosition = newPosition.normalized * dir.magnitude;
		
				
			diffPos = controller.position + newPosition * speed * Time.deltaTime;

			if(inBoundsControl(diffPos) == true) {
				controller.MovePosition (controller.position + newPosition * speed * Time.deltaTime);
			}

			fCounter+=Time.deltaTime;
			if(firestick.IsFingerDown() == true && fCounter > 0.3f) {
				Fire ();
				fCounter=0f;
			}
            else if(Input.GetKeyDown(KeyCode.LeftShift) || (Input.GetKeyDown(KeyCode.RightShift))) {
				Fire ();
			}
		}
	}


	void Fire() {
		if (features [(int)Game_Controller.SpecialFeatureList.doubleShooter].IsActive == true) { 
			tfirePositionLeft = new Vector3 (firePoint.position.x - 0.15f, firePoint.position.y-0.7f, firePoint.position.z);
			tfirePositionRight = new Vector3 (firePoint.position.x + 0.15f, firePoint.position.y-0.7f, firePoint.position.z);
			tBullet = (GameObject)Instantiate (bulletPrefab, tfirePositionLeft, bulletPrefab.transform.rotation);
			tBullet2 = (GameObject)Instantiate (bulletPrefab, tfirePositionRight, bulletPrefab.transform.rotation);
			if(features[(int)Game_Controller.SpecialFeatureList.fasterShooter].IsActive == true) {
				tBullet.GetComponent<Bullet> ().speed = 15f;
				tBullet2.GetComponent<Bullet> ().speed = 15f;
			}
		}
		else {
			tBullet = (GameObject)Instantiate (bulletPrefab, firePoint.position, bulletPrefab.transform.rotation);

			if(features[(int)Game_Controller.SpecialFeatureList.fasterShooter].IsActive == true) {
				tBullet.GetComponent<Bullet> ().speed = 15f;
			}
		}
		GetComponent<AudioSource> ().clip = laserSound;
		GetComponent<AudioSource> ().Play ();
	}

	void makeInvisible() {
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		transform.localScale = new Vector3 (0f, 0f, 0f);
		isInvisible = true;
	}

	void makeVisible() {
		initializeNewFeatures (Game_Controller.SpecialFeatureList.immortalShooter, featureTimeLimit-1.2f);
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
		transform.localScale = firstLocalScale;
		isInvisible = false;
	}


	void clearAllSpecialFeatures() {

		foreach(Special_Features fVal in features) {
			if(fVal.IsActive == true) {
				fVal.IsActive = false;
				fVal.Counter = 0f;
				Game_Controller.instance.initializeSpecialFeature (fVal.Feature, false);
			}
		}
		shield.SetActive (false);
		tDamageCounter = 0;
		bulletTransform.localScale = oldBulletScale;
		speed = currentSpeed;
		isHaveSuperPower = false;
	}

	void OnTriggerEnter(Collider temp) {

		if(features[(int)Game_Controller.SpecialFeatureList.immortalShooter].IsActive == false) { 
			if(temp.gameObject.tag != "PlayerBullet" && temp.gameObject.tag != "Points" && temp.gameObject.tag != "Life"  && temp.gameObject.tag != "Features") {
				if(features[(int)Game_Controller.SpecialFeatureList.ghostShooter].IsActive == false) {

					if(temp.gameObject.tag == "EnemyBullet") {
						Destroy(temp.gameObject);
					}
					Game_Controller.instance.changeHealthBar (2);

					if(Game_Controller.instance.healthValue <= 0) {

						GetComponent<AudioSource> ().clip = boomSound;
						GetComponent<AudioSource> ().Play ();

						makeInvisible ();

						clearAllSpecialFeatures ();

						StartCoroutine (killObject());
					}
				}
				else {
					tDamageCounter += 20;

					if(tDamageCounter >= 100) {
						features [(int)Game_Controller.SpecialFeatureList.ghostShooter].Counter =  features [(int)Game_Controller.SpecialFeatureList.ghostShooter].Limit;
						tDamageCounter = 0;
					}
				}
			}
		}
	}

	IEnumerator killObject() {
		bool situation = Game_Controller.instance.deadInitialize ();

		Vector3 newPos = new Vector3 (transform.position.x-0.5f, transform.position.y, Game_Controller.instance.explosionPrefab.transform.position.z);
		GameObject temp = (GameObject)Instantiate (Game_Controller.instance.explosionPrefab, newPos, Game_Controller.instance.explosionPrefab.transform.rotation);

		yield return new WaitForSeconds (GetComponent<AudioSource> ().clip.length-1.5f);

		if(situation == true) {
			Destroy(gameObject);
		}
		else {
			makeVisible ();
		}
		Destroy (temp);
	}
}
