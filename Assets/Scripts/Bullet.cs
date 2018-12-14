using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	[Header("Object Variables")]
	private Vector3 firstPosition;
	public float speed = 8f;

	void Awake () {
		firstPosition = transform.position;
	}
	

	void Update () {

		if(Vector3.Distance (firstPosition,transform.position) > 10f) {
			Destroy(gameObject);
		}

		transform.Translate (Vector3.up * speed * Time.deltaTime, Space.World);
	}

	void OnTriggerEnter(Collider temp) {
		if(temp.gameObject.tag == "EnemyBullet") {
			Destroy (gameObject);
		}
	}
}
