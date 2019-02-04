using UnityEngine;
using System.Collections;
/// <summary>
/// Sprite look at.
/// This will transform the sprite up axis to look directly at the top down camera.
/// this ensures the forward direction stays the same and matches the tilt of the camera
/// for better particle animations on the SpriteAnimator
/// </summary>
public class SpriteLookAt : MonoBehaviour
{
  
  /// <summary>
  /// The start.
  /// </summary>
  private Quaternion start;
  
  /// <summary>
  /// Start this instance.
  /// Gets the main camera, does a look rotation to match the camera, rotate the x forwards and subtract 90
  /// as we dont want the forward vector pointing at the camera we want the up vector.
  /// </summary>
  void Start ()
  {
    start = transform.localRotation;
  }
  
  // Update is called once per frame
  void Update ()
  {
    Quaternion lookAt = Quaternion.LookRotation (-Camera.main.transform.forward, Camera.main.transform.up);
    //    //we want the up axis (y) to point directly at the camera.
    lookAt.eulerAngles = new Vector3 (lookAt.eulerAngles.x, start.eulerAngles.y, start.eulerAngles.z);
    transform.localRotation = lookAt;
  }
}