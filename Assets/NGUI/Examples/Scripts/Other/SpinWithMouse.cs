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

	void Start ()
	{
		mTrans = transform;
	}

	void OnPress(bool pressed) {
		if(momentum)
			mPlane = new Plane(UICamera.currentCamera.transform.rotation * Vector3.back, mLastPos);
	}

	void OnDragStart() {
		dragging = true;
		mDragStartOffset = smoothDragStart ? UICamera.currentTouch.totalDelta : Vector2.zero;
	}

	void OnDrag (Vector2 delta)
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;

		if (target != null)
		{
			target.localRotation = Quaternion.Euler(Mathf.Abs(delta.normalized.y) * delta.y * speed, -Mathf.Abs(delta.normalized.x) * delta.x * speed, 0f) * target.localRotation;
		}
		else
		{
			mTrans.localRotation = Quaternion.Euler(Mathf.Abs(delta.normalized.y) * delta.y * speed, -Mathf.Abs(delta.normalized.x) * delta.x * speed, 0f) * mTrans.localRotation;
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
	}
	
	void OnDragEnd ()
	{
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
					target.localRotation = Quaternion.Euler(Mathf.Abs(offset.normalized.y) * offset.y * speed, -Mathf.Abs(offset.normalized.x) * offset.x * speed, 0f) * target.localRotation;
				}
				else
				{
					mTrans.localRotation = Quaternion.Euler(Mathf.Abs(offset.normalized.y) * offset.y * speed, -Mathf.Abs(offset.normalized.x) * offset.x * speed, 0f) * mTrans.localRotation;
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

	bool Momentum() {
		if(!momentum) {
			mMomentum = Vector2.zero;
		}
		return momentum;
	}
}