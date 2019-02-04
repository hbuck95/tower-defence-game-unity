using UnityEngine;
using System.Collections;
/// <summary>
/// Sprite animator. Custom Sprite Animation Object, attach this to the game object
/// that has a sprite sheet as a material. Define the total cols and rows, you can 
/// then select from which col and row the animation should start. I needed to be 
/// able to have a sprite sheet with multiple animations on it. You then tell it how many
/// cells to animate for. You can repeat the animation indefinitely or set a num of times
/// to repeat the animation. I also built in an autodestruct flag which will allow you to destroy
/// the game object automatically once the animation is completed.
/// 
/// The sprite sheet should be set up with the first frame in the top left corner and the animation 
/// will increase from left to right, top to bottom, reading the code below it may not seem this way
/// thing is I havent used OpenGL before and it appears that OpenGL inverts the entire texture, so flips it 
/// both ways, it then starts from the bottom going up.
/// </summary>
public class SpriteAnimator : MonoBehaviour
{
  
  //The total number of rows and cols on the sprite sheet
  public int colCount = 8;
  public int rowCount = 8;
  
  //Information about which portion of the SpriteSheet to animate, 
  //start row and col number, index is zero based.
  public int startColNum = 0;
  public int startRowNum = 0;
  //this is the total cells to animate for this specific animation
  //could be the entire sprite sheet or part thereof
  public int totalCells = 64;
  //the frames per second
  public int fps = 20;
  
  //set this to 0 for eternal loop
  public int repeat = 1;
  //Destroys the gameobject attached to this animation when repeat count 
  //has been reached
  public bool autoDestruct = true;
  private int repeatCount = 0;
  
  /// <summary>
  /// The current col number.
  /// </summary>
  private int currentColNum;
  /// <summary>
  /// The current row number.
  /// </summary>
  private int currentRowNum;
  /// <summary>
  /// The size of the entire sprite sheet i.e cols * rows stored in a Vector2
  /// which will be given to the shader to adjust the scale to ensure we take up
  /// a frame at a time
  /// </summary>
  private Vector2 size;
  
  /// <summary>
  /// My renderer, this will be the meshrenderer with the material attached to it.
  /// Inherited automatically, this could be null if the person has not assigned a material yet
  /// </summary>
  private Renderer myRenderer;
  private float time = 0f;
  private int prevIndex = -1;
  
  /// <summary>
  /// The animation complete flag, to indicate when the animation has finished
  /// </summary>
  private bool animationComplete = false;
  
  // Use this for initialization
  void Start ()
  {
    size = new Vector2 (1.0f / colCount, 1.0f / rowCount);

    
    myRenderer = GetComponent<Renderer>();
    if (myRenderer == null)
      enabled = false;
    
    myRenderer.material.SetTextureScale ("_MainTex", size);
  }
  
  // Update is called once per frame
  void Update ()
  {
    
    if (animationComplete)
      return;
    time += Time.deltaTime;
    int index = (int)(time * fps) % totalCells;
    
    if (index != prevIndex) {
      
      if (prevIndex == -1) {
        repeatCount++;
        currentColNum = startColNum;
        currentRowNum = startRowNum;
      }
      float offsetX = currentColNum * size.x;
      float offsetY = (1.0f - (size.y * (currentRowNum + 1)));
      Vector2 offset = new Vector2 (offsetX, offsetY);
      
      this.GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset);
      prevIndex = index;
      
      if (++currentColNum / colCount == 1) {
        currentColNum = 0;
        if (++currentRowNum / rowCount == 1) {
          currentRowNum = 0;  
        }
      }
      
      if (index + 1 == totalCells) {
        if (repeatCount == repeat) {
          animationComplete = true;
          if (autoDestruct) {
            if (transform.parent) {
              Destroy (transform.parent.gameObject);
            }
            
            Destroy (gameObject);
          }
          
        } else {
          repeatCount++;
          currentColNum = startColNum;
          currentRowNum = startRowNum;
        }
      }
    }
  }
}