using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour {


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
			transform.Rotate (Vector3.up, spinSpeed * Time.deltaTime);
			tDistanceToTarget = Vector3.Distance (target.position, transform.position);

			if (tDistanceToTarget <= 0.2f) {
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider temp) {
		if(temp.gameObject.tag != "EnemyObjects" && temp.gameObject.tag != "EnemyBullet" && temp.gameObject.tag != "Points" && temp.gameObject.tag != "PlayerBullet" && temp.gameObject.tag != "Life"  && temp.gameObject.tag != "Features") {
			
			GetComponent<AudioSource> ().Play ();
			gameObject.transform.localScale = new Vector3 (0f, 0f, 0f);
			makeInvisible ();
			StartCoroutine (killObject());

			Game_Controller.instance.gainLifeOrPoint();
		}
	}

	void makeInvisible() {
		transform.localScale = new Vector3 (0f, 0f, 0f);
		isInvisible = true;
		GetComponent<MeshCollider> ().enabled = false;
	}

	IEnumerator killObject() {
		Vector3 newPos = new Vector3 (transform.position.x, transform.position.y+0.4f, Game_Controller.instance.lifeEffectPrefab.transform.position.z);
		GameObject temp = (GameObject)Instantiate (Game_Controller.instance.lifeEffectPrefab, newPos, Game_Controller.instance.lifeEffectPrefab.transform.rotation);

		yield return new WaitForSeconds (GetComponent<AudioSource> ().clip.length);
		Destroy(gameObject);
		Destroy (temp);
	}
}
