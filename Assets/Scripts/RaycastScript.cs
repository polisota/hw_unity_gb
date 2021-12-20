using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastScript : MonoBehaviour
{
    public float rayDistance;

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(transform.position, ray.direction * rayDistance);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, rayDistance))
            {
                SceneManager.LoadScene("MiniGameLock_1", LoadSceneMode.Additive);
            }
        }
        
    }
}
