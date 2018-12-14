using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {

    bool shouldDrag = false;

	void Start () {
		
	}
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(worldPos.x, worldPos.y), Vector2.zero, 1000f);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    shouldDrag = true;
                }
            }
        }else if (Input.GetMouseButtonUp(0))
        {
            shouldDrag = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if(shouldDrag == true)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            }
            
        }
    }
}
