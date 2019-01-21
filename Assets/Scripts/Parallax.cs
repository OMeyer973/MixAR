using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

public class Parallax : MonoBehaviour
{
    public Camera camera;

    public const float GIRO_SPEED = 10.0f;
    public const float SPACING = 4.0f;
    public const float CAMERA_MARGIN = 7.0f;
    public const float CENTERING_SPEED = 40.0f;
    public const float MAX_CAMERA_POSITION = 30.0f;
    
    private Vector3 _centerGiroReference;
    private Vector3 _originalPosition;

    private const string PARALLAX_SPRITE_FOLDER = "Animations/";
    private List<GameObject> _spriteList = new List<GameObject>();


    #region PUBLIC_METHODS

    public void resetGiro()
    {
        //Setting initial phone position
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
        int layer = 0;
        bool spriteFound = true;
        string spriteFilename = "A_char"+charNumber+"_actionId"+actionId+"_SuccessId"+SucessId+"_layer";
        while (spriteFound == true)
        {
            //Loading sprite
            Sprite sprite = Resources.Load(PARALLAX_SPRITE_FOLDER + spriteFilename + layer, typeof(Sprite)) as Sprite;
            
            if (sprite == null)
                spriteFound = false;
            else
            {
                //Creating and positionning The GameObject
                float zAxis = CAMERA_MARGIN + SPACING * _spriteList.Count;
                GameObject go = new GameObject(spriteFilename + layer);
                SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                go.transform.position = new Vector3(0, 0, zAxis);

                //Applying sprite on gameObject
                renderer.sprite = sprite;
                _spriteList.Add(go);
            }
            layer++;
        }
        if(_spriteList.Count > 1)
        {
            //Setting camera LookAtConstraint cible
            ConstraintSource cible = new ConstraintSource();
            cible.sourceTransform = _spriteList[1].transform;
            cible.weight = 1;
            try
            {
                camera.GetComponent<LookAtConstraint>().SetSource(0,cible);
            }
            catch (System.InvalidOperationException e) //If source is null
            {
                camera.GetComponent<LookAtConstraint>().AddSource(cible);
            }
        }

    }
    #endregion

    #region PRIVATE_METHODS

    void clear()
    {
        foreach(GameObject sprite in _spriteList){
            Destroy(sprite);
        }
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
        //giro.x = Input.mousePosition.x/100;
        //giro.y = Input.mousePosition.y/100;
        giro.x = Input.acceleration.x;
        giro.y = Input.acceleration.y;
        
        //Calculating relative mouvement of giro
        _centerGiroReference += (giro - _centerGiroReference) / 10;
        giro = _centerGiroReference - giro;
        
        //Set new position of camera
        Vector3 newPos = camera.transform.position + giro * GIRO_SPEED;
        newPos = _originalPosition*0.2f + newPos *0.8f;
        

        //Clamping position
        newPos.x = Mathf.Clamp(newPos.x, -MAX_CAMERA_POSITION, MAX_CAMERA_POSITION);
        newPos.y = Mathf.Clamp(newPos.y, -MAX_CAMERA_POSITION, MAX_CAMERA_POSITION);

        // Move camera
        camera.transform.position = newPos;
    }

    #endregion

}
