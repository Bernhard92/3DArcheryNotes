using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System.Linq; 
using System; 


public class Controller : MonoBehaviour {

	public Canvas menuCanvas;
	public Canvas nameCanvas;
	public Canvas pointEntryCanvas;
	public Canvas rankingsCanvas;
	public GameObject[] nameEntrys; 
	public Text pointEntryName; 
	public ToggleGroup toggleGroup;
	public Text targetText; 
	public Text[] rankingNames;
	public Text[] rankingPoints; 

	public InputField killOne; 
	public InputField bodyOne;
	public InputField killTwo; 
	public InputField bodyTwo;
	public InputField killThree; 
	public InputField bodyThree; 
	public Toggle threeArrows; 
	public Toggle twoArrows; 
	private int arrows; 
	public Text thirdArrowPointsText; 
	public Toggle thirdArrowPointsOne; 
	public Toggle thirdArrowPointsTwo; 

	public Text firstArrowKillPoints; 
	public Text firstArrowBodyPoints; 
	public Text secondArrowKillPoints; 
	public Text secondArrowBodyPoints; 
	public Text thirdArrowKillPoints; 
	public Text thirdArrowBodyPoints; 
	public Button doneButton; 

	private string[] players; 
	private int[] playerPoints; 
	private int playerAmount; 
	private int currentPlayer = -1; 
	private int target = 1; 
	private string rulesURL = "http://www.papagoarchery.com/forms/Field_Guidelines-e.pdf";

	void Start() {	 
		//on first load
		if (!PlayerPrefs.HasKey("killOne")) {
			PlayerPrefs.SetInt("killOne", 20);
			PlayerPrefs.SetInt("bodyOne", 16);
			PlayerPrefs.SetInt("killTwo", 14);
			PlayerPrefs.SetInt("bodyTwo", 10);
			PlayerPrefs.SetInt("killThree", 8);
			PlayerPrefs.SetInt("bodyThree", 4);
			PlayerPrefs.SetInt ("Arrows", 3); 
		}
		arrows = PlayerPrefs.GetInt("Arrows"); 

	}

	public void InitGame(int playerAmount) {
		players = new string[playerAmount]; 
		playerPoints = new int[playerAmount];
		menuCanvas.gameObject.SetActive (false); 
		nameCanvas.gameObject.SetActive (true); 
		this.playerAmount = playerAmount;  
		NameEntrys (); 

	}

	private void NameEntrys() {
		//Set right amount of Inputfields interactabel
		for (int i = 0; i < playerAmount; i++) {
			nameEntrys [i].GetComponentInChildren<InputField> ().interactable = true; 
		}//Disable the other
		for (int i = playerAmount; i < nameEntrys.Length; i++) {
			nameEntrys [i].GetComponentInChildren<InputField> ().interactable = false; 
		}
	}

	public void BackFromNameEntrys() {
		for (int i = 0; i < playerAmount; i++) {
			nameEntrys [i].GetComponentInChildren<InputField> ().text = ""; 
		}
		nameCanvas.gameObject.SetActive (false); 
		menuCanvas.gameObject.SetActive (true);
	}

		
	//in this screen the user can pick his points
	public void ToPointEntrys() {
		bool emptyField = false; 
		for (int i = 0; i < playerAmount; i++) {
			players [i] = nameEntrys [i].GetComponentInChildren<InputField> ().text; 
			if(players[i].Equals("")) {
				emptyField = true; 
				Debug.Log ("Empty Field");
			}
		}

		//disables the 3. arrow if the user unchecked it
		thirdArrowPointsOne.gameObject.SetActive(arrows == 3);
		thirdArrowPointsTwo.gameObject.SetActive(arrows == 3);
		thirdArrowPointsText.gameObject.SetActive(arrows == 3); 

		//loads the arrow values an sets the options
		firstArrowKillPoints.text  = PlayerPrefs.GetInt("killOne").ToString();
       	firstArrowBodyPoints.text = PlayerPrefs.GetInt("bodyOne").ToString(); 
        secondArrowKillPoints.text = PlayerPrefs.GetInt("killTwo").ToString();
        secondArrowBodyPoints.text = PlayerPrefs.GetInt("bodyTwo").ToString();
        thirdArrowKillPoints.text = PlayerPrefs.GetInt("killThree").ToString(); 
        thirdArrowBodyPoints.text = PlayerPrefs.GetInt("bodyThree").ToString(); 

		
		if (!emptyField) {
			nameCanvas.gameObject.SetActive (false); 
			pointEntryCanvas.gameObject.SetActive (true); 
			//NextPlayer
			targetText.text = "Target: " + target;
			currentPlayer++; 
			currentPlayer %= playerAmount; 
			pointEntryName.text = players [currentPlayer];
			PointEntry (); 
		}			
	}

	public void PointEntry() {		
		Toggle activeToggle = toggleGroup.ActiveToggles ().FirstOrDefault (); 
		if (activeToggle != null) {
			Debug.Log (activeToggle.GetComponentInChildren<Text> ().text);
			playerPoints [currentPlayer] += int.Parse(activeToggle.GetComponentInChildren<Text> ().text); 
			activeToggle.GetComponent<Toggle> ().isOn = false; 
			//Nest Target
			if (currentPlayer == playerAmount-1) {
				target++; 
				targetText.text = "Target: " + target; 
			}

			//NextPlayer
			currentPlayer++; 
			currentPlayer %= playerAmount; 
			pointEntryName.text = players [currentPlayer]; 
		
		}
	}

	public void Done() {

		pointEntryCanvas.gameObject.SetActive (false); 
		rankingsCanvas.gameObject.SetActive (true); 
		int[] sortedPoints = sortPoints (); 
		string[] sortedNames = sortNames (); 


		//enable player amount
		for (int i = 0; i < playerAmount; i++) {
			rankingNames [i].gameObject.SetActive (true); 
			rankingPoints [i].gameObject.SetActive (true); 
			rankingPoints [i].text = sortedPoints [i].ToString();
			rankingNames [i].text = i+1 + ". " + sortedNames[i];
		}
		//disable the other
		for (int i = playerAmount; i < rankingNames.Length; i++) {
			rankingNames [i].gameObject.SetActive (false); 
			rankingPoints [i].gameObject.SetActive (false); 
		}


	}

	private string[] sortNames() {
		string[] sortedNames = new string[playerAmount]; 
		int biggest = 0; 
		int posInArray = 0;  

		for (int j = 0; j < playerAmount; j++) {
			for (int i = 0; i < playerAmount; i++) {
				if (playerPoints [i] > biggest) {
					biggest = playerPoints [i]; 
					sortedNames [j] = players [i]; 
					posInArray = i; 
				}
			}
			players [posInArray] = ""; 
			playerPoints [posInArray] = 0; 
			biggest = 0; 
		}

		return sortedNames; 
	}

	private int[] sortPoints() {
		int[] sortedPoints = new int[playerAmount]; 
		for (int i = 0; i < playerAmount; i++) {
			sortedPoints [i] = playerPoints [i]; 
		}
		Array.Sort (sortedPoints); 
		Array.Reverse (sortedPoints); 
		return sortedPoints; 
	}

	public void GameEnd() {
		for (int i = 0; i < playerAmount; i++) {
			nameEntrys [i].GetComponentInChildren<InputField> ().text = ""; 
		}
		players = null;  
		playerPoints = null; 
		playerAmount = 0; 
		currentPlayer = -1; 
		target = 1; 
		rankingsCanvas.gameObject.SetActive (false); 
		menuCanvas.gameObject.SetActive (true); 
	}

	//All values get saved bevor the settings menu gets closed
	public void BackFromModes() {
		PlayerPrefs.SetInt ("killOne", int.Parse(killOne.text));
		PlayerPrefs.SetInt ("bodyOne", int.Parse(bodyOne.text));
		PlayerPrefs.SetInt ("killTwo", int.Parse(killTwo.text));
		PlayerPrefs.SetInt ("bodyTwo", int.Parse(bodyTwo.text));
		PlayerPrefs.SetInt ("killThree", int.Parse(killThree.text));
		PlayerPrefs.SetInt ("bodyThree", int.Parse(bodyThree.text)); 
		PlayerPrefs.SetInt ("Arrows", threeArrows.isOn? 3:2);
		arrows = threeArrows.isOn? 3:2; 
	}

	//Change arrow values back to default
    public void SetDefaultValues() {
        killOne.text = "20";
        bodyOne.text = "16"; 
        killTwo.text = "14";
        bodyTwo.text = "10"; 
        killThree.text = "8"; 
        bodyThree.text = "4"; 
        threeArrows.isOn = true;
        twoArrows.isOn = false;  

    }

    //Loads points for the settings menu
    public void GetArrowPoints() {
    	if (!PlayerPrefs.HasKey("killOne")) {
    		Debug.Log("Empty"); 
    		SetDefaultValues(); 
    	} else {
    		killOne.text = PlayerPrefs.GetInt("killOne").ToString();
        	bodyOne.text = PlayerPrefs.GetInt("bodyOne").ToString(); 
        	killTwo.text = PlayerPrefs.GetInt("killTwo").ToString();
        	bodyTwo.text = PlayerPrefs.GetInt("bodyTwo").ToString();
        	killThree.text = PlayerPrefs.GetInt("killThree").ToString(); 
        	bodyThree.text = PlayerPrefs.GetInt("bodyThree").ToString(); 
        	if(PlayerPrefs.GetInt("Arrows") == 3) {
        		threeArrows.isOn = true; 
        		twoArrows.isOn = false; 
        	} else {
        		threeArrows.isOn = false; 
        		twoArrows.isOn = true;
        	}
    	}
    }

   










}
