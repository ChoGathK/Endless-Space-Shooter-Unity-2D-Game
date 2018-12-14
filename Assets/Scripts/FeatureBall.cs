using UnityEngine;
using System.Collections;

public class FeatureBall : MonoBehaviour {


	[Header("Object Variables")]
	public Transform target;
	private float speed = 2f;
	private bool isInvisible = false;
	public SpriteRenderer spriteBall;
	private Color[] spriteColor;
	private Color[] ballColor;
	private int identityBall;
	private Game_Controller.SpecialFeatureList feature;

	[Header("Temporary Variables")]
	private Vector3 tDir;
	private float tDistanceToTarget;

	void Start() {
		initialize ();
	}

	void initialize() {
		int rTemp = Random.Range (0, 5);

		spriteColor = new Color[5]{Color.red, Color.yellow, Color.blue, Color.red, Color.black};
		identityBall = rTemp;
		spriteBall.color = spriteColor [rTemp];
		spriteBall.sprite = Resources.Load<Sprite>("Sprites/tspec"+rTemp.ToString ());

		feature = (Game_Controller.SpecialFeatureList)identityBall;
	}

	void Update () {
		if (target != null && isInvisible == false) {
			tDir = target.position - transform.position;
			transform.Translate (tDir.normalized * speed * Time.deltaTime, Space.World);
			tDistanceToTarget = Vector3.Distance (target.position, transform.position);

			if (tDistanceToTarget <= 0.2f) {
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider temp) {
		if(temp.gameObject.tag != "EnemyObjects" && temp.gameObject.tag != "EnemyBullet" && temp.gameObject.tag != "Points" && temp.gameObject.tag != "PlayerBullet" && temp.gameObject.tag != "Life" && temp.gameObject.tag != "Features") {

			GetComponent<AudioSource> ().Play ();
			gameObject.transform.localScale = new Vector3 (0f, 0f, 0f);
			makeInvisible ();

			StartCoroutine (Game_Controller.instance.fadeOutMessage (Color.cyan, 35, feature+" is Active",  16));

			StartCoroutine (killObject());

			temp.gameObject.GetComponent<Spaceship>().initializeNewFeatures(feature, 0f);
		}
	}

	void makeInvisible() {
		transform.localScale = new Vector3 (0f, 0f, 0f);
		isInvisible = true;
		GetComponent<SphereCollider> ().enabled = false;
	}

	IEnumerator killObject() {
		Vector3 newPos = new Vector3 (transform.position.x, transform.position.y+0.4f, Game_Controller.instance.lifeEffectPrefab.transform.position.z);
		GameObject temp = (GameObject)Instantiate (Game_Controller.instance.lifeEffectPrefab, newPos, Game_Controller.instance.lifeEffectPrefab.transform.rotation);

		yield return new WaitForSeconds (GetComponent<AudioSource> ().clip.length);
		Destroy(gameObject);
		Destroy (temp);
	}
}
