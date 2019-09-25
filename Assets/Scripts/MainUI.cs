using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MainUI : MonoBehaviour {
    public static MainUI Instance;

    public Button button;
    public GameObject O, X;

    public GameObject pos;

    public Action onbutton;
    public Action<int> onpos;

    public List<GameObject> gos = new List<GameObject>();

	// Use this for initialization
	void Awake () {
        Instance = this;

        button.onClick.AddListener(OnButton);

        var initPos = pos.transform.localPosition;
        var offX = 258;
        var offY = 120;
        for(var i = 0; i < 9; i++) {
            var col = i % 3;
            var row = i / 3;
            var np = (GameObject)GameObject.Instantiate(pos);
            np.transform.parent = pos.transform.parent;
            np.transform.localScale = Vector3.one;
            np.transform.localPosition = Vector3.zero;
            var newPos = initPos + new Vector3(offX*col, -offY*row ,0);
            np.transform.localPosition = newPos;
            var id = i;
            np.GetComponent<Button>().onClick.AddListener(()=>{
                OnPos(id);
            });

            gos.Add(np);
        }

        pos.gameObject.SetActive(false);

        this.transform.Find("Text").GetComponent<Text>().text = "1";
	}
    private void OnButton() {
        if(onbutton != null) {
            onbutton();
        }
    }
	
    private void OnPos(int pos) {
        if(onpos != null) {
            onpos(pos);
        }
    }
}
