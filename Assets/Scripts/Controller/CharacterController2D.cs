using UnityEngine;
using System.Collections;

public class CharacterController2D : RaycastController
{
	[SerializeField] bool debug = false;

	float maxClimbAngle = 60;
	float maxDescendAngle = 60;
	bool triggerDown = false;
	CollisionInfo collisions;
	public CollisionInfo Collisions => collisions;

	public override void Start()
	{
		base.Start();
		collisions.faceDir = 1;

	}



	public override void Move(Vector2 moveAmount)
	{
		UpdateRaycastOrigins();

		collisions.Reset();
		collisions.moveAmountOld = moveAmount;

		if (moveAmount.x != 0)
		{
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		if (moveAmount.y < 0)
		{
			DescendSlope(ref moveAmount);
		}

		HorizontalCollisions(ref moveAmount);
		if (moveAmount.y != 0)
		{
			VerticalCollisions(ref moveAmount);
		}


		transform.Translate(moveAmount);


	}

	public void TriggerGoDown() => triggerDown = true;

	void HorizontalCollisions(ref Vector2 moveAmount)
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			if (debug)
			{
				var _dis = hit.distance == 0 ? rayLength : hit.distance;
				
			}
			if (hit)
			{

				if (hit.distance == 0)
				{
					continue;
				}

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				//올라갈 수 있는 경사의 경우 오른다.
				if (i == 0 && slopeAngle <= maxClimbAngle)
				{
					//Desend > Climb 우선순위, 올라갈 수 있다면 내려가는 것을 취소
					if (collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}

					//경사에 붙어서 올라가게 한다.
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					//위치 변환 벡터의 각도를 경사로에 맞게 변환
					ClimbSlope(ref moveAmount, slopeAngle);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				//올라갈 수 없는 경사의 경우 충돌면까지 이동
				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance; 
					//경사로를 올라가는 중이였다면 경사의 끝까지 간다.
					if (collisions.climbingSlope)
					{
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount)
	{
		float directionY = Mathf.Sign(moveAmount.y);
		float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
		
		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			if (debug)
			{
				var _dis = hit.distance == 0 ? rayLength : hit.distance;
				Debug.DrawRay(rayOrigin, Vector2.up * _dis * directionY, Color.red);
			}
			if (hit)
			{
				if (hit.collider.tag == "Through")
				{
					if (directionY == 1 || hit.distance == 0)
					{
						continue;
					}
					if (collisions.fallingThroughPlatform)
					{
						continue;
					}
					if (triggerDown)
					{
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform", .5f);
						continue;
					}
				}

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				//경사 이동중 위에 부딪혔다면 그 이상 이동하지 않게 한다
				if (collisions.climbingSlope)
				{
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		if (collisions.climbingSlope)
		{
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					if(slopeAngle <= maxClimbAngle)
						collisions.slopeAngle = slopeAngle;
					else
					{
						collisions.climbingSlope = false;
						collisions.slopeAngle = 0;
					}
				}
			}
		}
	}

	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
	{
		float moveDistance = Mathf.Abs(moveAmount.x);
		float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY)
		{
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector2 moveAmount)
	{
		float directionX = Mathf.Sign(moveAmount.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if (Mathf.Sign(hit.normal.x) == directionX)
				{
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
					{
						float moveDistance = Mathf.Abs(moveAmount.x);
						float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
						moveAmount.y -= descendmoveAmountY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	void ResetFallingThroughPlatform()
	{
		collisions.fallingThroughPlatform = false;
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
