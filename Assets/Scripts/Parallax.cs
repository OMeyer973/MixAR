using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class Parallax : MonoBehaviour
{
    public Camera camera;

    public const float GIRO_SPEED = 2.0f;
    public const float SPACING = 4.0f;
    public const float CAMERA_MARGIN = 7.0f; //Usefull with orthographic camera
    public const float CENTERING_SPEED = 40.0f;
    public const float MAX_CAMERA_POSITION = 15.0f;
    public const float SCROLLSPEED = 0.01f;


    private Vector3 _centerGiroReference;
    private Vector3 _originalPosition;
    
    private const string PARALLAX_ANIMATED_GAMEOBJECT_FOLDER = "Animations/";
    private List<GameObject> _bdElemList = new List<GameObject>();
    public GameObject cameraCible;
    
    #region PUBLIC_METHODS

    public void resetGiro()
    {
        //Setting initial phone position²
        //_centerGiroReference.x = Input.mousePosition.x;
        //_centerGiroReference.y = Input.mousePosition.y;
        _centerGiroReference.x = Input.acceleration.x;
        _centerGiroReference.y = Input.acceleration.y;
        _originalPosition = camera.transform.position;
    }

    //@brief Add a sprite to the current scene in front of the caméra following Parallax parameters
    //@param SpriteFilename : Filename of the sprite without extention
    public void addSprite(int charNumber, int actionId, int SucessId) {
        resetGiro();

        //Loading animated element
        string spriteFilename = "A_char"+charNumber+"_actionId"+actionId+"_SuccessId"+SucessId;
        GameObject animatedBdElement = Instantiate(Resources.Load(PARALLAX_ANIMATED_GAMEOBJECT_FOLDER + spriteFilename, typeof(GameObject)) as GameObject);
        
        //Positionning sprite
        float zPos = CAMERA_MARGIN;
        foreach (Transform child in animatedBdElement.transform)
        {
            float xPos = 0;
            foreach (GameObject go in _bdElemList)
                xPos += go.transform.GetChild(0).GetComponent<Collider>().bounds.size.y;
            child.position = new Vector3(xPos, 0, zPos);
            zPos += SPACING;
        }

        //Set camera cible
        if (_bdElemList.Count == 0)
            setCameraCible(animatedBdElement.transform.GetChild(0));
        
        //Adding to stored GameObject
        _bdElemList.Add(animatedBdElement);   
    }

    public void clear()
    {
        foreach (GameObject sprite in _bdElemList)
            Destroy(sprite);
    }

    #endregion

    #region PRIVATE_METHODS

    void setCameraCible(Transform transform)
    {
        ConstraintSource cible = new ConstraintSource();
        cameraCible.transform.position = transform.position; 
        cible.sourceTransform = cameraCible.transform;
        cible.weight = 1;
        try
        {
            camera.GetComponent<LookAtConstraint>().SetSource(0, cible);
        }
        catch (System.InvalidOperationException) //If source is null
        {
            camera.GetComponent<LookAtConstraint>().AddSource(cible);
        }
    } 

    private bool _swipeAlreadyDetected; //Save if swipe is currently running
    // Update is called once per frame
    void Update()
    {
        detectSwapBetweenBDElements();
        updateCameraPositionForGyroscopEffect();
    }

    private Touch lastTouchDetected;
    private void detectSwapBetweenBDElements()
    {
        float deltaX = 0; //deltaY = 0;
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
                lastTouchDetected = touch;
            if (touch.phase == TouchPhase.Moved)
            {
                deltaX = lastTouchDetected.position.x - touch.position.x; //greater than 0 is right and less than zero is left
                //deltaY = touch.position.y - lastTouchDetected.position.y; //greater than 0 is up and less than zero is down
                lastTouchDetected = touch;

                //Moving camera
                float clampedX = Mathf.Clamp(deltaX * SCROLLSPEED + camera.transform.position.x, _bdElemList[0].transform.GetChild(0).position.x, _bdElemList[_bdElemList.Count - 1].transform.GetChild(0).position.x);
                camera.transform.position = new Vector3(clampedX, camera.transform.position.y, camera.transform.position.z);
                _originalPosition = new Vector3(clampedX, _originalPosition.y, _originalPosition.z);
                cameraCible.transform.position = new Vector3(clampedX, cameraCible.transform.position.y, cameraCible.transform.position.z);
            }
            else if (touch.phase == TouchPhase.Ended)
                lastTouchDetected = new Touch();
        }
    }

    private void updateCameraPositionForGyroscopEffect()
    {
        
        //Getting giroscope current values
        Vector3 giro = Vector3.zero;
        //giro.x = Input.mousePosition.x/100;
        //giro.y = Input.mousePosition.y/100;
        giro.x = Input.acceleration.x;
        giro.y = Input.acceleration.y;
        
        //Calculating relative mouvement of giro
        _centerGiroReference += (giro - _centerGiroReference) / 10;
        giro = _centerGiroReference - giro;

        //Set new position of camera
        Vector3 newPos = camera.transform.position + giro * GIRO_SPEED;
        newPos = _originalPosition * 0.2f + newPos * 0.8f;
        
        //Clamping position
        newPos.x = Mathf.Clamp(newPos.x, _originalPosition.x - MAX_CAMERA_POSITION, _originalPosition.x + MAX_CAMERA_POSITION);
        newPos.y =  Mathf.Clamp(newPos.y, _originalPosition.y - MAX_CAMERA_POSITION, _originalPosition.y + MAX_CAMERA_POSITION);
        
        // Move camera
        camera.transform.position = newPos;
    }

    #endregion

}
