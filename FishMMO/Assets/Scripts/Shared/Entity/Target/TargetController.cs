﻿#if !UNITY_SERVER
using FishMMO.Client;
#endif
using UnityEngine;

[RequireComponent(typeof(Character))]
public class TargetController : MonoBehaviour
{
	public const float MAX_TARGET_DISTANCE = 50.0f;
	public const float TARGET_UPDATE_RATE = 0.05f;

#if !UNITY_SERVER
	private float nextTick = 0.0f;
	private Cached3DLabel targetLabel;
#endif

	public Character Character;
	public LayerMask LayerMask;
	public TargetInfo LastTarget;
	public TargetInfo Current;

#if !UNITY_SERVER
	void OnDestroy()
	{
		Character = null;
		LastTarget = default;
		Current = default;
		LabelMaker.Cache(targetLabel);
	}

	void Update()
	{
		// update target label for the client
		if (nextTick < 0.0f)
		{
			nextTick = TARGET_UPDATE_RATE;

			LastTarget = Current;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

			Current = GetTarget(this, ray, MAX_TARGET_DISTANCE);

			// same target label remains
			if (LastTarget.Target != Current.Target)
			{
				if (targetLabel == null)
				{
					// construct the target label
					targetLabel = Character.LabelMaker.Display("", Character.Transform.position, Color.green, 1.0f, 0.0f, true);
					targetLabel.gameObject.SetActive(false);
				}

				if (LastTarget.Target != null)
				{
					Outline outline = LastTarget.Target.GetComponent<Outline>();
					if (outline != null)
					{
						outline.enabled = false;
					}
					targetLabel.gameObject.SetActive(false);
				}

				if (Current.Target != null)
				{
					Outline outline = Current.Target.GetComponent<Outline>();
					if (outline != null)
					{
						Vector3 newPos = Current.Target.position;

						Collider collider = Current.Target.GetComponent<Collider>();
						BoxCollider box = collider as BoxCollider;
						if (box != null)
						{
							newPos.y += box.bounds.size.y + 0.15f;
						}
						else
						{
							SphereCollider sphere = collider as SphereCollider;
							if (sphere != null )
							{
								newPos.y += sphere.radius + 0.15f;
							}
						}
						targetLabel.SetPosition(newPos);
						targetLabel.SetText(Current.Target.name);
						targetLabel.gameObject.SetActive(true);
						outline.enabled = true;
					}
				}
			}
		}
		nextTick -= Time.deltaTime;
	}
#endif

	public static TargetInfo GetTarget(TargetController controller, Ray ray, float maxDistance)
	{
		float distance = maxDistance.Clamp(0.0f, MAX_TARGET_DISTANCE);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, distance, controller.LayerMask))
		{
			//Debug.DrawLine(ray.origin, hit.point, Color.red, 1);
			//Debug.Log("hit: " + hit.transform.name + " pos: " + hit.point);
			return new TargetInfo(hit.transform, hit.point);
		}
		return new TargetInfo(null, ray.GetPoint(distance));
	}
}