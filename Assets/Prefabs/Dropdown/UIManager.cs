using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	public Animator contentPanel;
	public Animator gearImage;

	void Start()
	{
		RectTransform transform = contentPanel.gameObject.transform as RectTransform;        
		Vector2 position = transform.anchoredPosition;
		position.y -= transform.rect.height;
		transform.anchoredPosition = position;
	}
	
	public void ToggleMenu()
	{
		contentPanel.enabled = true;
		
		bool isHidden = contentPanel.GetBool("isHidden");
		contentPanel.SetBool("isHidden", !isHidden);
		gearImage.enabled = true;
		gearImage.SetBool("isHidden", !isHidden);
	}

}
