using UnityEngine;
using System.Collections;

public class MobileJoystick : MonoBehaviour {


	// amount of time the user has to place finger on screen before joystick appears
	public float fingerTimeBeforeJoystickAppears = 0.1f;

	// If after scaling the image you want to ensure the thumbstick
	// rotates in a tight circle you can click this option
	public bool  forceCircleConstraint;

	// joystick image must contain GUITexture
	public GameObject imageJoystick;

	// bg for joystick must contain GUITexture
	public GameObject imageBG;	

	// public property that any script can get the position of the joystick and use it to move an object
	public Vector2 position;

	// which side of screen you want the stick to appear
	// left stick uses left hand side only
	// for right stick mark as false and it uses the right hand side of screen
	public bool  isLeftStick;

	// How much the position is multiplied by
	// If using Unity Controller scripts it can be
	// used to change the speed of the player
	public float positionMultiplier = 5;

	// makes the joystick act as a button
	public bool  isButton = false;

	public int tapCount;

	// private vars no need to worry
	private Rect isInMotionStartingArea;
	private float rotFTime;
	private float motionFTime;	
	private bool  isMotionPress = false;
	private bool  movementStickShowing = false;
	private float maxAmount  = 0.09f;
	private int motionFingerNum = -1;
	private int lastFingerId= -1;	


	private float imageJoystickScale;


	void  Awake (){
		SetUpImagesIfMissing();

	}

	void  Start (){

		imageJoystickScale =  imageJoystick.transform.localScale.x /  imageJoystick.transform.localScale.y;
		// hide sticks intially
		ShowHideMotion(false);

	}
	void  Update (){
		//// create bounds
		if(isLeftStick)
			isInMotionStartingArea = new Rect (50, 50, Screen.width/2 - 100, Screen.height - 100);
		else
			isInMotionStartingArea	= new Rect (Screen.width / 2 + 50, 50, Screen.width / 2 - 100, Screen.height - 100);
		// Touches 
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount == 0) {
				ShowHideMotion(false);
			}

			foreach(Touch touch in Input.touches)
			{
				if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {

					if (motionFingerNum == touch.fingerId) {
						motionFingerNum = -1;
						lastFingerId = -1;
						position = Vector2.zero;
						ShowHideMotion(false);

					}

				}
				if (Time.time > motionFTime + fingerTimeBeforeJoystickAppears && motionFingerNum == touch.fingerId ) {
					if (movementStickShowing)
						UpdateMotion(touch.position);
					else
						ShowHideMotion(true);

				}

				if (touch.phase == TouchPhase.Began) {

					if ( motionFingerNum == -1 && isInMotionStartingArea.Contains (touch.position)) {
						SetMotionPoint (touch.position);
						motionFTime = Time.time;
						motionFingerNum = touch.fingerId;
						lastFingerId = 1;
					}

				}
			}
		}

		// if using editor then allow the mouse button to be the "touches"
		else {
			if (Input.GetMouseButtonDown(0)) {
				if (isInMotionStartingArea.Contains (Input.mousePosition)) {
					lastFingerId = 1;

					isMotionPress = true;
					SetMotionPoint(Input.mousePosition);
					motionFTime = Time.time;
				} else { isMotionPress = false; }

			}
			if (Input.GetMouseButton(0) ) {
				if (isMotionPress && Time.time > motionFTime + fingerTimeBeforeJoystickAppears) {
					if (movementStickShowing)
						UpdateMotion (Input.mousePosition);
					else
						ShowHideMotion(true);
				}

			}
			if (Input.GetMouseButtonUp(0)) {
				lastFingerId = -1;
				position = Vector2.zero;				
				ShowHideMotion(false);
			}
		}
	}


	// you can omit images if you want to have an invisible controller
	void  SetUpImagesIfMissing (){
		if(imageJoystick == null)
		{
			imageJoystick = new GameObject("Joy");
			imageJoystick.AddComponent<GUITexture>();
		}
		if(imageBG == null)
		{
			imageBG = new GameObject("BG");
			imageBG.AddComponent<GUITexture>();
		}

	}

	Vector3  ClampStick ( Vector3 p, Vector3 downPosition){

		if (Vector3.Distance (p, downPosition) > maxAmount) {
			Vector3 dir;
			dir = downPosition - p ;

			dir.Normalize ();
			return (dir * maxAmount) + p;    
		}
		return downPosition;
	}

	Vector3  ClampStickScaleJoyStick ( Vector3 p , Vector3 downPosition){

		if (Vector3.Distance (p, downPosition) > maxAmount) {
			Vector3 dir;
			dir = downPosition - p ;

			dir.Normalize ();
			dir.x = dir.x * imageJoystickScale;
			return (dir * maxAmount) + p;    
		}
		return downPosition;
	}	


	void  UpdateMotion (  Vector3 newPos  ){
		if(!isButton)
		{
			imageJoystick.transform.position = new Vector3 (newPos.x / Screen.width , newPos.y / Screen.height, 0);
			if(!forceCircleConstraint)
				imageJoystick.transform.position = ClampStick(imageBG.transform.position, imageJoystick.transform.position);
			else
			{
				imageJoystick.transform.position = ClampStickScaleJoyStick(imageBG.transform.position, imageJoystick.transform.position);
				position.x = position.x /imageJoystickScale;
			}

			position = (imageJoystick.transform.position - imageBG.transform.position) * positionMultiplier;

		}
	}

	void  SetMotionPoint (  Vector3 newPos  ){
		imageBG.transform.position = imageJoystick.transform.position = new Vector3 (newPos.x / Screen.width, newPos.y / Screen.height, 0);
		position = Vector2.zero;
	}

	void  ShowHideMotion ( bool show  ){
		if(show)
		{
			imageJoystick.SetActive(true);
			//imageBG.GetComponent<GUITexture>().active = true;
			movementStickShowing = true;
		}
		else
        {
            imageJoystick.SetActive(false);
            //imageBG.GetComponent<GUITexture>().active = false;
            movementStickShowing = false;

		}

	}

	void  Disable (){
		gameObject.SetActive (false);	
	}

	public bool IsFingerDown (){
		return (lastFingerId != -1);
	}

}