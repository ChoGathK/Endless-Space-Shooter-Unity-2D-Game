using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class Virtual_Joystick : MonoBehaviour,IPointerDownHandler, IDragHandler, IPointerUpHandler {


	private Image bgImg;
	private Image joystickImg;
	public GameObject fireButton;
	public bool isFiring;
	public bool isEnabled;

	public Vector3 inputDirection{ get; set;}


	public virtual void OnDrag(PointerEventData ped) {
		
		if(isEnabled == true) {
			Vector2 pos = Vector2.zero;
			if(RectTransformUtility.ScreenPointToLocalPointInRectangle (bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
				pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
				pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

				float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 : pos.x * 2 ;
				float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 : pos.y * 2;

				inputDirection = new Vector3 (x, 0, y);
				inputDirection = (inputDirection.magnitude > 1) ? inputDirection.normalized : inputDirection;
				joystickImg.rectTransform.anchoredPosition = new Vector3 (inputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3),inputDirection.z * (bgImg.rectTransform.sizeDelta.y / 3));
			}
		}
	}

	public virtual void OnPointerDown(PointerEventData ped) {
		
	}

	public virtual void OnPointerUp(PointerEventData ped) {
		inputDirection = Vector3.zero;
		joystickImg.rectTransform.anchoredPosition = Vector3.zero;

	}
	void Awake () {
		bgImg = GetComponent<Image> ();
		joystickImg = transform.GetChild(0).GetComponent <Image> ();

		#if UNITY_ANDROID
			inputDirection = Vector3.zero;
			isFiring = false;
			isEnabled = true;
		#else
			isEnabled = false;
			bgImg.enabled = false;
			joystickImg.enabled = false;
			fireButton.SetActive (false);
		#endif
	}

	public void fireButtonClickDown () {
		if (isEnabled == true) {
			isFiring = true;
		}
	}

	public void fireButtonClickUp () {
		if (isEnabled == true) {
			isFiring = false;
		}
	}
}
