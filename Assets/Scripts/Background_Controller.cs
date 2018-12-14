using UnityEngine;

public class Background_Controller : MonoBehaviour {

	[Header("Object Variables")]
	private float scrollSpeed = -4f;
	public float tileSizeZ = -40.5f;
	private Vector3 startPosition;

	void Start ()
	{
		startPosition = transform.position;
	}

	void Update ()
	{
		transform.position = startPosition + Vector3.up * 	Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
	}
}
