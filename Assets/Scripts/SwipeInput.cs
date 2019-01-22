 using UnityEngine;
 
 public class SwipeInput : MonoBehaviour
 {
     public bool swiping;
 
     public float minSwipeDistance;
     public float errorRange;
 
     public SwipeDirection direction = SwipeDirection.None;
 
     public enum SwipeDirection {Right, Left, Up, Down, None}
 
     private Touch initialTouch;
 
     void Start()
     {
         Input.multiTouchEnabled = true;
     }
 
     void Update()
     {
         if (Input.touchCount <= 0)
             return;
 
         foreach (var touch in Input.touches)
         {
             if (touch.phase == TouchPhase.Began)
             {
                 initialTouch = touch;
             }
             else if (touch.phase == TouchPhase.Moved)
             {
                 var deltaX = touch.position.x - initialTouch.position.x; //greater than 0 is right and less than zero is left
                 var deltaY = touch.position.y - initialTouch.position.y; //greater than 0 is up and less than zero is down
                 var swipeDistance = Mathf.Abs(deltaX) + Mathf.Abs(deltaY);
 
                 if (swipeDistance > minSwipeDistance && (Mathf.Abs(deltaX) > 0 || Mathf.Abs(deltaY) > 0))
                 {
                     swiping = true;
 
                     CalculateSwipeDirection(deltaX, deltaY);
                 }
             }
             else if (touch.phase == TouchPhase.Ended)
             {
                 initialTouch = new Touch();
                 swiping = false;
                 direction = SwipeDirection.None;
             }
             else if (touch.phase == TouchPhase.Canceled)
             {
                 initialTouch = new Touch();
                 swiping = false;
                 direction = SwipeDirection.None;
             }
         }
     }
 
     void CalculateSwipeDirection(float deltaX, float deltaY)
     {
         bool isHorizontalSwipe = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);
 
         // horizontal swipe
         if (isHorizontalSwipe && Mathf.Abs(deltaY) <= errorRange)
         {
             //right
             if (deltaX > 0)
                 direction = SwipeDirection.Right;
             //left
             else if (deltaX < 0)
                 direction = SwipeDirection.Left;
         }
         //vertical swipe
         else if (!isHorizontalSwipe && Mathf.Abs(deltaX) <= errorRange)
         {
             //up
             if (deltaY > 0)
                 direction = SwipeDirection.Up;
             //down
             else if (deltaY < 0)
                 direction = SwipeDirection.Down;
         }
         //diagonal swipe
         else
         {
             swiping = false;
         }
     }
 }
 