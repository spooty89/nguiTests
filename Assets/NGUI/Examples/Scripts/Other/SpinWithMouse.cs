using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin With Mouse")]
public class SpinWithMouse : MonoBehaviour
{
	public Transform target;
	public float speed = 1f;

	Transform mTrans;

	public bool momentum = true;
	public bool smoothDragStart = true;
	public float momentumAmount = 35f;
	bool dragging;
	protected Vector2 mMomentum = Vector2.zero;
	protected Vector2 mLastPos = Vector2.zero;
	protected Vector2 mDragStartOffset = Vector2.zero;
	protected Plane mPlane;
	protected Vector3 mPoint;
	protected GameObject releaseTransform;

	protected float x, y;
	protected float distance;

	void Start ()
	{
		mTrans = transform;
	}

	void OnPress(bool pressed) {
		mPlane = new Plane(UICamera.currentCamera.transform.rotation * Vector3.back, target != null ? target.position : mTrans.position);
		if(UICamera.currentTouchID == -2) {

			Ray ray = UICamera.currentCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));

			float dist;
			mPlane.Raycast(ray, out dist);
			mPoint = ray.GetPoint(dist);
			distance = Vector3.Distance(UICamera.currentCamera.transform.position, mPoint);
			
			x = UICamera.currentCamera.transform.eulerAngles.y;
			y = UICamera.currentCamera.transform.eulerAngles.x;
		}
	}

	void OnDragStart() {
		if(UICamera.currentTouchID == -1) {
			dragging = true;
			mDragStartOffset = smoothDragStart ? UICamera.currentTouch.totalDelta : Vector2.zero;
		}
	}

	void OnDrag (Vector2 delta)
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
		if(UICamera.currentTouchID == -1) {
			if (target != null)
			{
				target.localRotation = Quaternion.Euler(UICamera.currentCamera.transform.TransformDirection(new Vector3(Mathf.Abs(delta.normalized.y) * delta.y * speed, -Mathf.Abs(delta.normalized.x) * delta.x * speed, 0f))) * target.localRotation;
			}
			else
			{
				mTrans.localRotation = Quaternion.Euler(UICamera.currentCamera.transform.TransformDirection(new Vector3(Mathf.Abs(delta.normalized.y) * delta.y * speed, -Mathf.Abs(delta.normalized.x) * delta.x * speed, 0f))) * mTrans.localRotation;
			}
			
			if(Momentum()) {
				Ray ray = smoothDragStart ?
					UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - mDragStartOffset) :
						UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
				
				float dist = 0f;
				
				if (mPlane.Raycast(ray, out dist))
				{
					Vector2 currentPos = new Vector2(ray.GetPoint(dist).x, ray.GetPoint(dist).y);
					Vector2 offset = currentPos - mLastPos;
					mLastPos = currentPos;
					mMomentum = Vector2.Lerp(mMomentum, mMomentum + offset * (0.01f * momentumAmount), 0.67f);
				}
			}
		} else if (UICamera.currentTouchID == -2) {
			x += delta.x * distance * speed;
			y -= delta.y * distance * speed;
			y = ClampAngle(y, -360, 360);

			Quaternion rotation = Quaternion.Euler(y, x, 0);
			UICamera.currentCamera.transform.rotation = rotation;
			UICamera.currentCamera.transform.position = (rotation * new Vector3(0, 0, -distance) + mPoint);
		}
	}
	
	void OnDragEnd ()
	{
		if(releaseTransform != null)
			Destroy(releaseTransform);
		releaseTransform = new GameObject();
		releaseTransform.transform.position = UICamera.currentCamera.transform.position;
		releaseTransform.transform.rotation = UICamera.currentCamera.transform.rotation;
		dragging = false;
	}

	void LateUpdate() {
		float delta = RealTime.deltaTime;
		if (!dragging)
		{
			if (mMomentum.magnitude > 0.0001f)
			{
				Vector3 meh = UICamera.currentCamera.transform.TransformDirection(new Vector3(mMomentum.magnitude * 0.05f, mMomentum.magnitude * 0.05f, 0f));
				mMomentum -= new Vector2(meh.x, meh.y);
				
				// Move the scroll view
				Vector3 offset = NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
				if (target != null)
				{
					target.localRotation = Quaternion.Euler(releaseTransform.transform.TransformDirection(new Vector3(Mathf.Abs(offset.normalized.y) * offset.y * speed, -Mathf.Abs(offset.normalized.x) * offset.x * speed, 0f))) * target.localRotation;
				}
				else
				{
					mTrans.localRotation = Quaternion.Euler(releaseTransform.transform.TransformDirection(new Vector3(Mathf.Abs(offset.normalized.y) * offset.y * speed, -Mathf.Abs(offset.normalized.x) * offset.x * speed, 0f))) * mTrans.localRotation;
				}
			}
			else
			{
				mMomentum = Vector3.zero;
			}
		}
		else
		{
			// Dampen the momentum
			NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
		}
	}

	float ClampAngle(float angle, float min, float max) {
		if(angle < -360) angle += 360;
		if(angle > 360f) angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}

	bool Momentum() {
		if(!momentum) {
			mMomentum = Vector2.zero;
		}
		return momentum;
	}
}