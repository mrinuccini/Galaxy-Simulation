using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour
{

	protected Transform _XForm_Camera;
	protected Transform _XForm_Parent;

	protected Vector3 _LocalRotation;
	protected float _CameraDistance = 242f;

	public float MouseSensitivity = 4f;
	public float ScrollSensitvity = 2f;
	public float OrbitDampening = 10f;
	public float ScrollDampening = 6f;

	public float focusPointMoveSpeed = 9f;

	public bool CameraDisabled = false;


	// Use this for initialization
	void Start()
	{
		this._XForm_Camera = this.transform;
		this._XForm_Parent = this.transform.parent;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.R)) 
		{
			_XForm_Parent.transform.position = Vector3.zero;
		}
	}

	void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
			CameraDisabled = !CameraDisabled;

		if (!CameraDisabled && Input.GetKey(KeyCode.Mouse0))
		{
			//Rotation of the Camera based on Mouse Coordinates
			if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
			{
				_LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
				_LocalRotation.y += Input.GetAxis("Mouse Y") * MouseSensitivity;
			}
		}

		//Zooming Input from our Mouse Scroll Wheel
		if (Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

			ScrollAmount *= (this._CameraDistance * 0.3f);

			this._CameraDistance += ScrollAmount * -1f;

			this._CameraDistance = Mathf.Clamp(this._CameraDistance, 1.5f, 1000f);
		}

		//Actual Camera Rig Transformations
		Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
		this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

		if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
		{
			this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
		}

		// Move the focus point
		float xMov = Input.GetAxis("Horizontal");
		float yMov = Input.GetAxis("Vertical");

		Vector2 movement = new Vector2(xMov, yMov);

		_XForm_Parent.transform.position += (_XForm_Camera.transform.right * movement.x + _XForm_Camera.transform.forward * movement.y) * focusPointMoveSpeed * Time.deltaTime;
	}
}