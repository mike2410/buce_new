using UnityEngine;
using System.Collections;


/*
  Flyer-Helper-class, draws a raycast from a flying objects to the ground and makes an associated sprite travel along the ground based on surface-normals
*/

public class FlyerHelper : MonoBehaviour {

     
    public Transform raycastOriginTransform;
    public GameObject raycastHitIndicator;
    public Vector3 raycastHitIndicatorOffset = new Vector3(0, 0.0135f, -0.25f);
    public Transform parentTransform;

    private Vector3 gameOriginPosition;

    public Color raycastHitIndicatorColor;
    public Color lineColor;

    private LineRenderer line;
    
    // Use this for initialization
	void Start () {

        parentTransform = this.gameObject.transform;
        gameOriginPosition = new Vector3(0, 8.2f, 0); //approximately the middle of the gamescene, ignoring y
        
        //instantiate hitIndicator gameObejct ~ "sprite"
        raycastHitIndicator = Instantiate(raycastHitIndicator, raycastOriginTransform.position, raycastOriginTransform.rotation) as GameObject;
        raycastHitIndicator.transform.parent = parentTransform;
        raycastHitIndicator.renderer.material.color = raycastHitIndicatorColor;
        
        line = raycastHitIndicator.GetComponent<LineRenderer>();
        line.material.SetColor("_TintColor", lineColor);
        line.SetPosition(0, gameObject.transform.position);
	}
	
	// Update is called once per frame
	void Update () {

        toggleHelper(false);

        if (isInBounds(parentTransform.transform.position))
        {

            toggleHelper(true);
            


            RaycastHit hitPosition;
            Ray downRay = new Ray(raycastOriginTransform.position, Vector3.down);


            if (Physics.Raycast(downRay, out hitPosition, 5))
            {



                //place the line-renderes starting- and ending points
                line.SetPosition(1, hitPosition.point);
                line.SetPosition(0, gameObject.transform.position);


                //translate hitIndicator to coordinates where down-ray hit
                raycastHitIndicator.transform.position = hitPosition.point;

                //rotate hitIndicator according to normal of surface hit
                raycastHitIndicator.transform.rotation = Quaternion.FromToRotation(raycastHitIndicator.transform.up, hitPosition.normal) * raycastHitIndicator.transform.rotation;






            }

        }
	
	}

    private void toggleHelper(bool toggleBool)
    {
        raycastHitIndicator.gameObject.SetActive(toggleBool);
        line.gameObject.SetActive(toggleBool);
    }

    private bool isInBounds(Vector3 transformVector) {
        if (Vector3.Distance(transformVector, gameOriginPosition) < 2.3f) { //2.3f = value of minimum proximity to origin
            return true;
        }
        else
        {
            return false;
        }
    }
}
