using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_Aspect_Ratio_Scale : MonoBehaviour {
	public Vector2 scaleOnRatio1 = new Vector2(1f, 1f);
	private RectTransform rectMyTrans;
	private float widthHeightRatio;
	private float widthRatio;
	private float heightRatio;

	private float designedWidth = 480f;
	private float designedHeight = 720f;

	Vector3 calculateNewPosition() {
		Vector3 res = Vector3.zero;
		float tWidth = ((rectMyTrans.rect.width * rectMyTrans.localScale.x)) / 2f;
		float tHeight = ((rectMyTrans.rect.height * rectMyTrans.localScale.x)) / 2f;

		if((rectMyTrans.position.x + tWidth)>Screen.width) {
			res.x = Screen.width - tWidth;
		}
		else if((rectMyTrans.position.x - tWidth) < 0) {
			res.x = tWidth;
		}
		else {
			res.x = rectMyTrans.position.x;
		}

		if((rectMyTrans.position.y + tHeight)>Screen.height) {
			res.y = Screen.height - tHeight;
		}
		else if((rectMyTrans.position.y - tHeight) < 0) {
			res.y = tHeight;
		}
		else {
			res.y = rectMyTrans.position.y;
		}

		return res;
	}


	// Use this for initialization
	void Start () {
		rectMyTrans = GetComponent<RectTransform>();
		setScale ();
	}
	
	// Update is called once per frame
	void setScale () {
		widthRatio = (float)Screen.width / designedWidth;
		heightRatio = (float)Screen.height / designedHeight;


		widthHeightRatio = (widthRatio > heightRatio) ? heightRatio : widthRatio;

		rectMyTrans.localScale = new Vector3 (scaleOnRatio1.x * widthHeightRatio, scaleOnRatio1.y * widthHeightRatio, 1);

		rectMyTrans.position = calculateNewPosition ();
	}
}
