//  CameraFacing.cs 
//  original by Neil Carter (NCarter)
//  modified by Hayden Scott-Baron (Dock) - http://starfruitgames.com
//  allows specified orientation axis


using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
  Camera referenceCamera;
  
  public enum Axis
  {
    up,
    down,
    left,
    right,
    forward,
    back}
  ;
  public bool reverseFace = false; 
  public Axis axis = Axis.up; 
  Quaternion local;
  
  // return a direction based upon chosen axis
  public Vector3 GetAxis (Axis refAxis)
  {
    switch (refAxis) {
    case Axis.down:
      return Vector3.down; 
    case Axis.forward:
      return Vector3.forward; 
    case Axis.back:
      return Vector3.back; 
    case Axis.left:
      return Vector3.left; 
    case Axis.right:
      return Vector3.right; 
    }
    
    // default is Vector3.up
    return Vector3.up;    
  }
  
  void  Awake ()
  {
    // if no camera referenced, grab the main camera
    if (!referenceCamera)
      referenceCamera = Camera.main; 
      
    local = transform.localRotation;
  }
  
  void LateUpdate ()
  {
    Quaternion newRot = transform.localRotation;
    newRot.eulerAngles.Set (newRot.eulerAngles.x, local.eulerAngles.y, local.eulerAngles.z);
    transform.localRotation = Quaternion.Euler (new Vector3 (newRot.eulerAngles.x, local.eulerAngles.y, local.eulerAngles.z));
  }
  
  void  Update ()
  {
    // rotates the object relative to the camera
    Vector3 direction = (referenceCamera.transform.position - transform.position).normalized;
    
    float result = Vector3.Dot (direction, transform.forward);
    if (result < 0)
      reverseFace = true;
    else
      reverseFace = false;
    Vector3 targetPos = transform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
    Vector3 targetOrientation = referenceCamera.transform.rotation * GetAxis (axis);
    transform.LookAt (targetPos, targetOrientation);
  }
}