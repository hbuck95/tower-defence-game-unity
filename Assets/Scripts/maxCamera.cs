// based off of original script found at: http://www.unifycommunity.com/wiki/index.php?title=MouseOrbitZoom
// Changed the way the camera allows for movement, added safeguards for min/max positions.

using UnityEngine;
using System.Collections;


namespace Assets
{
    public class maxCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 targetOffset;
        public float distance = 5.0f;
        public float maxDistance = 200;
        public float minDistance = .6f;
        public float xSpeed = 75f;
        public float ySpeed = 75f;
        public int yMinLimit = -80;
        public int yMaxLimit = 80;
        public int zoomRate = 40;
        public float panSpeed = 0.3f;

        public float zoomDampening = Mathf.Infinity;//stop the camera from self-scrolling when releasing the button, and allow for smooth scrolling.

        private float _maxX = 230f;
        private float _minX = 90f;
        private float _maxY = 90f;
        private float _minY = 45f;
        private float _maxZ = 150f;
        private float _minZ = 30f;

        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float currentDistance;
        private float desiredDistance;
        private Quaternion currentRotation;
        private Quaternion desiredRotation;
        private Quaternion rotation;
        private Vector3 position, startPos;
        public bool canMove = true;//stops camera movement when hitting the max positions
        private bool initialised = false;
        private Quaternion startRot;

        public void Start()
        {
            StartCoroutine(StartCamera());
        }

        private IEnumerator StartCamera()
        {
            yield return new WaitForSeconds(1);
            if (!target)
            {
                GameObject go = new GameObject("Cam Target");
                go.transform.position = transform.position + (transform.forward * distance);
                target = go.transform;
            }

            distance = Vector3.Distance(transform.position, target.position);
            currentDistance = distance;
            desiredDistance = distance;

            //be sure to grab the current rotations as starting points.
            position = transform.position;
            rotation = transform.rotation;
            currentRotation = transform.rotation;
            desiredRotation = transform.rotation;
            startRot = transform.rotation;
            startPos = transform.position;

            xDeg = Vector3.Angle(Vector3.right, transform.right);
            yDeg = Vector3.Angle(Vector3.up, transform.up);
            initialised = true;
        }

        public void Init()
        {
            StartCoroutine(StartCamera());
        }

        /*
         * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
         */
        void LateUpdate() {
            if (Time.timeScale == 0 || !initialised)
                return;

            if (Input.GetMouseButton(1))
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                //Clamp the vertical axis for the orbit
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                // set camera rotation 
                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                currentRotation = transform.rotation;

                rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                transform.rotation = rotation;

            }
            else if (Input.GetMouseButtonDown(2))
            {
                //grab the rotation of the camera so we can move in a psuedo local XY space
                target.rotation = transform.rotation;
                target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
            }


            //stop the camera from zooming when hitting a boundary
            if (canMove) {
                desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * (Time.timeScale * 30);
                currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.timeScale * zoomDampening);
                transform.position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            }

            //set the relative position coordinate to +/- 1 of the min/max to stop the camera from continuously hitting the boundary
            if (transform.position.y > _maxY)
                transform.position = new Vector3(transform.position.x, _maxY - 1, transform.position.z);

            if (transform.position.y < _minY)
                transform.position = new Vector3(transform.position.x, _minY + 1, transform.position.z);

            if (transform.position.x > _maxX)
                transform.position = new Vector3(_maxX - 1, transform.position.y, transform.position.z);

            if (transform.position.x < _minX)
                transform.position = new Vector3(_minX + 1, transform.position.y, transform.position.z);

            if (transform.position.z > _maxZ)
                transform.position = new Vector3(transform.position.x, transform.position.y, _maxZ - 1);

            if (transform.position.z < _minZ)
                transform.position = new Vector3(transform.position.x, transform.position.y, _minZ + 1);

        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.R)) {//reset the camera position
                transform.rotation = startRot;
                transform.position = startPos;
                xDeg = Vector3.Angle(Vector3.right, transform.right);
                yDeg = Vector3.Angle(Vector3.up, transform.up);
                Input.ResetInputAxes();
            }
        }

        private static float ClampAngle(float angle, float min, float max) {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}