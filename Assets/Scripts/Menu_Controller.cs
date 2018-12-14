using UnityEngine;
using UnityEngine.UI;

public class Menu_Controller : MonoBehaviour {


	[Header("Object Variables")]
	public GameObject mainPanel;
	public GameObject optionsPanel;


	public void menuButtonClick(string pButtonName) {

		if(pButtonName == "back") {
			optionsPanel.SetActive (false);
			mainPanel.SetActive (true);
		}
		else if(pButtonName == "options") {
			optionsPanel.SetActive (true);
			mainPanel.SetActive (false);
		}
		else if(pButtonName == "exit") {
			Application.Quit ();
		}
		else if(pButtonName == "start") {
			Level_Controller.instance.loadLevel(1);
		}
		else if(pButtonName == "resume") {
			Game_Controller.instance.resumeGame ();
		}
		else if(pButtonName == "restart") {
			Level_Controller.instance.restartScene();
		}
	}
}
