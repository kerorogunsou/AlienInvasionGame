using UnityEngine;
using System.Collections.Generic;

public class control_stuff : MonoBehaviour {
	public GameObject alienPrefab;
	public GameObject endgameMenu;
	public Camera globalCamera;
    const int alienSquadSize = 3;
	Vector3 defaultAlienPosition = new Vector3(-3f, 1.5f, 0);

	float timescale;
	List<GameObject> aliens = new List<GameObject>();
	bool alienSquadInitialized;

    public GameObject ChooseRandomAlien() {
        if (aliens.Count > 0) {
            return aliens[Random.Range(0, aliens.Count - 1)];
        }
        else {
            return null;
        }
    }

	void Start () {
		endgameMenu.SetActive (false);
		timescale = Time.timeScale;
        InitializeAliens(alienSquadSize);
	}
	void InitializeAliens(int alienCount) {
		aliens.RemoveRange (0, aliens.Count);
		for(int i = 0; i < alienCount; i++){
            aliens.Add((GameObject)Instantiate(alienPrefab, defaultAlienPosition + VecRand(), Quaternion.identity));	
		}
        alienSquadInitialized = true;
	}
	Vector3 VecRand() {
		return globalCamera.ViewportToWorldPoint(new Vector3(0.4f, 0.5f, globalCamera.nearClipPlane)) + new Vector3(Random.Range (-0.3f, 0.5f), Random.Range (-0.3f, 0.5f), 0f);
	}

	void Update () {
		for(int i = 0; i < aliens.Count; i++) {
			if(aliens[i] == null) {
				aliens.RemoveAt (i);
                i--;
			}
		}
        bool allAliensHaveDied = aliens.Count <= 0 && alienSquadInitialized;
        if (allAliensHaveDied) {
			endgameMenu.SetActive (true);
            alienSquadInitialized = false;
			Time.timeScale = 0;
		}
	}
	public void Exit(){
		Application.Quit();
	}
	public void ResetApp() {
		Time.timeScale = timescale;
		Application.LoadLevel(1);
	}
}
