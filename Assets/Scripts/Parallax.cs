using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

public class Parallax : MonoBehaviour
{

    public const float GIRO_SPEED = 15.0f;
    public const float SPACING = 6.0f;
    public const float CAMERA_MARGIN = 22.0f;
    public const float CENTERING_SPEED = 40.0f;
    public const float MAX_CAMERA_POSITION = 30.0f;
    
    private Vector3 _centerGiroReference;
    private Vector3 _originalPosition;

    private const string PARALLAX_SPRITE_FOLDER = "ProductionAssets/";
    private List<GameObject> _spriteList = new List<GameObject>();


    #region PUBLIC_METHODS

    //@brief Add a sprite to the current scene in front of the caméra following Parallax parameters
    //@param SpriteFilename : Filename of the sprite without extention
    public void addSprite(string spriteFilename) {
        //Creating and positionning The GameObject
        float zAxis = CAMERA_MARGIN + SPACING * _spriteList.Count;
        GameObject go = new GameObject(spriteFilename);
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        go.transform.position = new Vector3(0, 0, zAxis);

        //Loading sprite
        Sprite sprite = Resources.Load(PARALLAX_SPRITE_FOLDER + spriteFilename, typeof(Sprite)) as Sprite;
        renderer.sprite = sprite;
        _spriteList.Add(go);

        //Setting camera LookAtConstraint cible
        ConstraintSource cible = new ConstraintSource();
        cible.sourceTransform = _spriteList[_spriteList.Count - 1].transform;
        cible.weight = 1;
        try
        {
            GetComponent<LookAtConstraint>().SetSource(0,cible);
        }
        catch (System.InvalidOperationException e) //If source is null
        {
            GetComponent<LookAtConstraint>().AddSource(cible);
        }

    }
    #endregion

    #region PRIVATE_METHODS

    // Use this for initialization
    void Start()
    {
        //Setting initial phone position
        _centerGiroReference.x = Input.mousePosition.x;
        _centerGiroReference.y = Input.mousePosition.y;
        _centerGiroReference.x = Input.acceleration.x;
        _centerGiroReference.y = Input.acceleration.y;

        _originalPosition = transform.position;

        //Adding sprite manualy for testing
        addSprite("Parallax3");
        addSprite("Parallax1");
        addSprite("Parallax2");
        
    }

    // Update is called once per frame
    void Update()
    {
        updateCamera();
    }

    private void updateCamera()
    {
        //Getting giroscope current values
        Vector3 giro = Vector3.zero;
        giro.x = Input.mousePosition.x/100;
        giro.y = Input.mousePosition.y/100;
        giro.x = Input.acceleration.x;
        giro.y = Input.acceleration.y;
        
        //Calculating relative mouvement of giro
        _centerGiroReference += (giro - _centerGiroReference) / 20;
        giro = _centerGiroReference - giro;
        
        //Set new position of camera
        Vector3 newPos = transform.position + giro * GIRO_SPEED;
        newPos = _originalPosition*0.1f + newPos *0.9f;
        

        //Clamping position
        newPos.x = Mathf.Clamp(newPos.x, -MAX_CAMERA_POSITION, MAX_CAMERA_POSITION);
        newPos.y = Mathf.Clamp(newPos.y, -MAX_CAMERA_POSITION, MAX_CAMERA_POSITION);

        // Move camera
        transform.position = newPos;
    }

    #endregion

}
