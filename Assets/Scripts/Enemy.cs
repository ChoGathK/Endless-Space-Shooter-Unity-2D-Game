using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	[Header("Object Variables")]
	public Transform target;
	private float speed = 3f;
	public Transform firePoint;
	private float countDown = 0f;
	private float fireWaitTime = 1f;
	private bool isInvisible = false;

	[Header("Prefabs")]
	public GameObject bulletPrefab;

	[Header("Temporary Variables")]
	private float tDistanceToTarget;
	private GameObject tObject;
	public Vector3 tDir;

	void Update () {
		if (target != null && isInvisible == false) {
			tDir = target.position - transform.position;
			transform.Translate (tDir.normalized * speed * Time.deltaTime, Space.World);

			tDistanceToTarget = Vector3.Distance (target.position, transform.position);

			if (tDistanceToTarget <= 0.2f) {
				Destroy (gameObject);
			}
			if(countDown <= 0.0f) {
				Fire();
				countDown = fireWaitTime;
			}
			countDown -= Time.deltaTime;
		}

	}

	void Fire() {
		tObject = (GameObject)Instantiate (bulletPrefab, firePoint.position, bulletPrefab.transform.rotation);
		tObject.transform.parent = transform;
	}

	void OnTriggerEnter(Collider temp) {
		if(temp.gameObject.tag != "EnemyObjects" && temp.gameObject.tag != "EnemyBullet" && temp.gameObject.tag != "Points" && temp.gameObject.tag != "Life"  && temp.gameObject.tag != "Features") {

			if (temp.gameObject.tag == "PlayerBullet") {
				Destroy (temp.gameObject);
			}

			GetComponent<AudioSource> ().Play ();
			gameObject.transform.localScale = new Vector3 (0f, 0f, 0f);
			makeInvisible ();
			StartCoroutine (killObject());

			Game_Controller.instance.gainPoints (20);
		}
	}

	void makeInvisible() {
		transform.localScale = new Vector3 (0f, 0f, 0f);
		isInvisible = true;
		GetComponent<MeshCollider> ().enabled = false;
	}

	IEnumerator killObject() {
		Vector3 newPos = new Vector3 (transform.position.x, transform.position.y, Game_Controller.instance.explosionPrefab.transform.position.z);
		GameObject temp = (GameObject)Instantiate (Game_Controller.instance.explosionPrefab, newPos, Game_Controller.instance.explosionPrefab.transform.rotation);

		yield return new WaitForSeconds (GetComponent<AudioSource> ().clip.length);
		Destroy(gameObject);
		Destroy (temp);
	}

	void OnCollisionEnter(Collision temp) {
		Debug.Log ("enemy collision");
	}
}
