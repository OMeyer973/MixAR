using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

public class Parallax : MonoBehaviour
{

    public const float GIRO_SPEED = 15;
    public const float SPACING = 6;
    public const float CAMERA_MARGIN = 22;
    public const float CENTERING_SPEED = 40;
    public List<string> spriteFileList = new List<string>();

    private Vector3 _centerGiroReference;
    private Vector3 _originalPosition;

    private const string PARALLAX_SPRITE_FOLDER = "ProductionAssets/";
    private List<GameObject> _spriteList = new List<GameObject>();


    #region PRIVATE_METHODS

    // Use this for initialization
    void Start()
    {
        _centerGiroReference.x = Input.acceleration.x;
        _centerGiroReference.y = Input.acceleration.y;
        _originalPosition = transform.position;

        spriteFileList.Add("Parallax3");
        spriteFileList.Add("Parallax1");
        spriteFileList.Add("Parallax2");

        float i = 0;
        foreach (string file in spriteFileList)
        {
            GameObject go = new GameObject("Test");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            go.transform.position = new Vector3(0, 0, CAMERA_MARGIN + i++ * SPACING);

            Sprite sprite = Resources.Load(PARALLAX_SPRITE_FOLDER + file, typeof(Sprite)) as Sprite;
            renderer.sprite = sprite;
            _spriteList.Add(go);
        }
        ConstraintSource cible = new ConstraintSource();
        cible.sourceTransform = _spriteList[_spriteList.Count - 1].transform;
        cible.weight = 1;
        GetComponent<LookAtConstraint>().AddSource(cible);
    }

    // Update is called once per frame
    void Update()
    {
        updateCamera();
    }

    private void updateCamera()
    {
        Vector3 giro = Vector3.zero;
        giro.x = Input.acceleration.x;
        giro.y = Input.acceleration.y;
        giro *= GIRO_SPEED * Time.deltaTime * 50;


        Vector3 newPos = transform.position + giro - _centerGiroReference;

        newPos += (_originalPosition - newPos) / 60;
        Debug.Log("fin :" + (_originalPosition - newPos));
        _centerGiroReference += (giro - _centerGiroReference) / (1 - 1 / CENTERING_SPEED);

        // Move object
        transform.position = newPos;
    }

    #endregion
}
