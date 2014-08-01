using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	public void MakeItRed() {
		GetComponent<UIWidget>().color = Color.red;
	}
	public void MakeItGreen() {
		GetComponent<UIWidget>().color = Color.green;
	}
	public void MakeItBlue() {
		GetComponent<UIWidget>().color = Color.blue;
	}
}
