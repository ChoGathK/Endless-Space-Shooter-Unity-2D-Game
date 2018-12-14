using UnityEngine;

public class EnemyBullet : MonoBehaviour {

	[Header("Object Variables")]
	private Vector3 firstPosition;
	private float speed = 3f;


	void Awake () {
		firstPosition = transform.position;
	}

	void Update () {
		
		if(Vector3.Distance (firstPosition,transform.position) > 10f) {
			Destroy(gameObject);
		}

		transform.Translate (Vector3.down * speed * Time.deltaTime, Space.World);
	}

	void OnTriggerEnter(Collider temp) {
		if(temp.gameObject.tag == "PlayerBullet") {
			Destroy (gameObject);
			Destroy (temp.gameObject);
		}
	}
}
