using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public string text = "";
    public bool hasBeenShowed = false;

    private GameObject _camera;
    private GameObject _gameobject;
    private GameObject _cameraCible;

    private GameObject _rightMask;
    private GameObject _leftMask;


    #region PUBLIC_METHODS

    //@brief Add a sprite to the current scene in front of the caméra following Parallax parameters
    //@param SpriteFilename : Filename of the sprite without extention
    public void initAction(int charNumber, int actionId, int sucessId, Vector3 position, string textAssociated = "")
    {
        text = textAssociated;
        string spriteFilename = "A_char" + charNumber + "_actionId" + actionId + "_SuccessId" + sucessId;
        GameObject animatedBdElement = Resources.Load(PARALLAX_ANIMATED_GAMEOBJECT_FOLDER + spriteFilename, typeof(GameObject)) as GameObject;
        init(animatedBdElement, position);
    }

    public void initTrap(int trapId, Vector3 position, string textAssociated = "")
    {
        text = textAssociated;
        string spriteFilename = "T_" + trapId;
        GameObject animatedBdElement = Resources.Load(PARALLAX_ANIMATED_GAMEOBJECT_FOLDER + spriteFilename, typeof(GameObject)) as GameObject;
        init(animatedBdElement, position);
    }

    public void init(GameObject prefab, Vector3 position)
    {
        createCamera(position);
        resetGiro();
        GameObject animatedBdElement = Instantiate(prefab);
        animatedBdElement.transform.position = position;
        _gameobject = animatedBdElement;
        createParallaxSprite(position);
        setInvisible();
    }

    

    public void setVisible()
    {
        _gameobject.SetActive(true);
        _camera.SetActive(true);
    }

    public void setInvisible()
    {
        _gameobject.SetActive(false);
        _camera.SetActive(false);
    }

    public void showText()
    {
        if(text != "")
            AnimationManager.Instance.hoverAnimationTextGameObject.SetActive(true);
        AnimationManager.Instance.TextAnimationText.text = text;
        hasBeenShowed = true;
    }

    public void hideText()
    {
        AnimationManager.Instance.hoverAnimationTextGameObject.SetActive(false);
        hasBeenShowed = false;
    }

    public void clear()
    {
        Destroy(_gameobject);
        Destroy(_camera);
        Destroy(_rightMask);
        Destroy(_leftMask);
    }


    

    #endregion

    #region PRIVATE_METHODS

    void createCamera(Vector3 position)
    {
        _camera = new GameObject("Camera");
        Camera camComponent = _camera.AddComponent<Camera>();
        LookAtConstraint lac = _camera.AddComponent<LookAtConstraint>();
        lac.constraintActive = true;
        _camera.transform.position = new Vector3(position.x, position.y, 0);
        camComponent.orthographic = true;
        camComponent.orthographicSize = 4.6f;
        camComponent.clearFlags = CameraClearFlags.SolidColor;
        camComponent.backgroundColor = Color.white;
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

    

    void createParallaxSprite(Vector3 position)
    {
        //Positionning sprite
        float zPos = CAMERA_MARGIN;
        foreach (Transform child in _gameobject.transform)
        {
            float xPos = 0;

            //Translate for multiple BD block
            child.position = new Vector3(position.x,position.y,zPos);
            zPos += SPACING;
        }
        //Set mask

        //right
        _rightMask = new GameObject("white mask");
        _rightMask.transform.SetParent(_gameobject.transform);
        SpriteRenderer sprite = _rightMask.AddComponent<SpriteRenderer>();
        sprite.sprite = Resources.Load<Sprite>("white");
        _rightMask.transform.localScale = new Vector3(_gameobject.GetComponent<BoxCollider>().size.x, _gameobject.GetComponent<BoxCollider>().size.y, 1);
        _rightMask.transform.position = new Vector3(_gameobject.GetComponent<BoxCollider>().size.x - 0.1f, 0, 1);
        //left
        _leftMask = Instantiate(_rightMask);
        _leftMask.transform.position = new Vector3(-_gameobject.GetComponent<BoxCollider>().size.x + 0.1f, 0, 1);
        _leftMask.transform.SetParent(_gameobject.transform);

        //Set camera cible
        setCameraCible(_gameobject.transform.GetChild(0));
    }

    void setCameraCible(Transform tr)
    {
        ConstraintSource cible = new ConstraintSource();
//        _cameraCible.transform.position = transform.position; 
        cible.sourceTransform = tr;
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
