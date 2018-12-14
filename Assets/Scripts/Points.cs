using UnityEngine;
using System.Collections;

public class Points : MonoBehaviour {

	[Header("Object Variables")]
	private float speed = 2.5f;
	private int pointValue;
	public bool isInvisible = false;
	private Color[] randColors;
	public GameObject pointTextObject;
	public Transform target;

	[Header("Temporary Variables")]
	public Vector3 tDir;
	private float tDistanceToTarget;


	void Start () {
		randColors = new Color[8]{Color.black, Color.red, Color.blue, Color.white, Color.cyan, Color.green, Color.magenta, Color.yellow};
		initialize ();
	}

	void initialize() {
		int rTemp = Random.Range (1, 8);
		pointValue = rTemp * 10;
		pointTextObject.GetComponent<TextMesh>().text = pointValue.ToString ();

		MeshRenderer temp = GetComponent<MeshRenderer> ();
		temp.material.color = randColors[rTemp];
	}


	void Update () {
		if (target != null && isInvisible != true) {
			tDir = target.position - transform.position;
			transform.Translate (tDir.normalized * speed * Time.deltaTime, Space.World);
			tDistanceToTarget = Vector3.Distance (target.position, transform.position);

			if (tDistanceToTarget <= 0.2f) {
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider temp) {
		if(temp.gameObject.tag != "EnemyObjects" && temp.gameObject.tag != "EnemyBullet" && temp.gameObject.tag != "Points" && temp.gameObject.tag != "Life"  && temp.gameObject.tag != "PlayerBullet"  && temp.gameObject.tag != "Features") {

			GetComponent<AudioSource> ().Play ();
			gameObject.transform.localScale = new Vector3 (0f, 0f, 0f);

			makeInvisible ();
			StartCoroutine (killObject());

			Game_Controller.instance.gainPoints (pointValue);
		}
	}

	void makeInvisible() {
		transform.localScale = new Vector3 (0f, 0f, 0f);
		isInvisible = true;
		GetComponent<SphereCollider> ().enabled = false;
	}

	IEnumerator killObject() {
		Vector3 newPos = new Vector3 (transform.position.x, transform.position.y, Game_Controller.instance.pointEffectPrefab.transform.position.z);
		GameObject temp = (GameObject)Instantiate (Game_Controller.instance.pointEffectPrefab, newPos, Game_Controller.instance.pointEffectPrefab.transform.rotation);

		yield return new WaitForSeconds (GetComponent<AudioSource> ().clip.length);
		Destroy(gameObject);
		Destroy (temp);
	}
}
