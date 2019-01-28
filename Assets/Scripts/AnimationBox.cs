using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class AnimationBox : MonoBehaviour
{
    
    public const float GIRO_SPEED = 0.8f;
    public const float SPACING = 4.0f;
    public const float CAMERA_MARGIN = 7.0f; //Usefull with orthographic camera
    public const float CENTERING_SPEED = 40.0f;
    public const float MAX_CAMERA_POSITION = 10.0f;
    public const float SCROLLSPEED = 0.01f;


    private Vector3 _centerGiroReference;
    private Vector3 _originalPosition;
    
    private const string PARALLAX_ANIMATED_GAMEOBJECT_FOLDER = "AnimationsPrefabs/";

    private GameObject _camera;
    private GameObject _gameobject;
    private GameObject _cameraCible;


    #region PUBLIC_METHODS

    //@brief Add a sprite to the current scene in front of the caméra following Parallax parameters
    //@param SpriteFilename : Filename of the sprite without extention
    public AnimationBox(int charNumber, int actionId, int sucessId, Vector3 position)
    {
        string spriteFilename = "A_char" + charNumber + "_actionId" + actionId + "_SuccessId" + sucessId;
        GameObject animatedBdElement = Instantiate(Resources.Load(PARALLAX_ANIMATED_GAMEOBJECT_FOLDER + spriteFilename, typeof(GameObject)) as GameObject);
        animatedBdElement.transform.position = position;
        createCamera(position);
        resetGiro();
        createParallaxSprite(animatedBdElement, position);
        setInvisible();
    }

    public AnimationBox(GameObject prefab, Vector3 position)
    {
        createCamera(position);
        resetGiro();
        GameObject animatedBdElement = Instantiate(prefab);
        animatedBdElement.transform.position = position;
        createParallaxSprite(animatedBdElement, position);
        setInvisible();
    }

    

    public void setVisible()
    {
        _gameobject.SetActive(true);
    }

    public void setInvisible()
    {
        _gameobject.SetActive(false);
    }

    public void clear()
    {
        Destroy(_gameobject);
        Destroy(_camera);
    }


    #endregion

    #region PRIVATE_METHODS

    void createCamera(Vector3 position)
    {
        _camera = new GameObject();
        Camera camComponent = _camera.AddComponent<Camera>();
        _camera.AddComponent<LookAtConstraint>();
        _camera.transform.position = new Vector3(position.x, position.y, 0);
        camComponent.orthographic = true;
        camComponent.orthographicSize = 4.7f;
    }

    void resetGiro()
    {
        //Setting initial phone position²
        //_centerGiroReference.x = Input.mousePosition.x;
        //_centerGiroReference.y = Input.mousePosition.y;
        _centerGiroReference.x = Input.acceleration.x;
        _centerGiroReference.y = Input.acceleration.y;
        _originalPosition = _camera.transform.position;
    }

    

    void createParallaxSprite(GameObject animatedBdElement, Vector3 position)
    {
        //Positionning sprite
        float zPos = CAMERA_MARGIN;
        foreach (Transform child in animatedBdElement.transform)
        {
            float xPos = 0;

            //Translate for multiple BD block
            child.position = new Vector3(position.x,position.y,zPos);
            zPos += SPACING;
        }

        //Set camera cible
        setCameraCible(animatedBdElement.transform.GetChild(0));

        //Adding to stored GameObject
        _gameobject = animatedBdElement;
    }

    void setCameraCible(Transform transform)
    {
        ConstraintSource cible = new ConstraintSource();
//        _cameraCible.transform.position = transform.position; 
        cible.sourceTransform = transform;
        cible.weight = 1;
        try
        {
            _camera.GetComponent<LookAtConstraint>().SetSource(0, cible);
        }
        catch (System.InvalidOperationException) //If source is null
        {
            _camera.GetComponent<LookAtConstraint>().AddSource(cible);
        }
    } 

    private bool _swipeAlreadyDetected; //Save if swipe is currently running
    // Update is called once per frame
    void Update()
    {
        updateCameraPositionForGyroscopEffect();
    }

    /*
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
    */

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
        Vector3 newPos = _camera.transform.position + giro * GIRO_SPEED;
        newPos = _originalPosition * 0.2f + newPos * 0.8f;
        
        //Clamping position
        newPos.x = Mathf.Clamp(newPos.x, _originalPosition.x - MAX_CAMERA_POSITION, _originalPosition.x + MAX_CAMERA_POSITION);
        newPos.y =  Mathf.Clamp(newPos.y, _originalPosition.y - MAX_CAMERA_POSITION, _originalPosition.y + MAX_CAMERA_POSITION);
        
        // Move camera
        _camera.transform.position = newPos;
    }

    #endregion

}
