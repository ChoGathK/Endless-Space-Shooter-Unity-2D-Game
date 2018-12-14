using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour {


	[Header("Object Variables")]
	public Transform target;
	private float speed = 2f;
	private float spinSpeed = 250f;
	private bool isInvisible = false;

	[Header("Temporary Variables")]
	private Vector3 tDir;
	private float tDistanceToTarget;

	void Update () {
		if (target != null && isInvisible == false) {
			tDir = target.position - transform.position;
			transform.Translate (tDir.normalized * speed * Time.deltaTime, Space.World);
			transform.Rotate (Vector3.up+Vector3.right, spinSpeed * Time.deltaTime);
			tDistanceToTarget = Vector3.Distance (target.position, transform.position);

			if (tDistanceToTarget <= 0.2f) {
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider temp) {
		if(temp.gameObject.tag != "EnemyObjects" && temp.gameObject.tag != "EnemyBullet" && temp.gameObject.tag != "Points" && temp.gameObject.tag != "Life") {

			if (temp.gameObject.tag == "PlayerBullet") {
				Destroy (temp.gameObject);
			}


			GetComponent<AudioSource> ().Play ();
			gameObject.transform.localScale = new Vector3 (0f, 0f, 0f);
			makeInvisible ();
			StartCoroutine (killObject());

			Game_Controller.instance.gainPoints (10);
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
}
