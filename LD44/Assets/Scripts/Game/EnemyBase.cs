﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : BattleUnit {
	public Health health;

	Pathfinder pathfinder;
	[SerializeField] int pathPos;
	[SerializeField] Vector2[] path;
	public float speed = 5;

	LTDescr moveDesc = null;

	void Start() {
		health.Init();
		
		pathfinder = new Pathfinder(GameManager.Instance.Player.aINavMeshGenerator);
	}

	public override void Die() {
		//anim.SetTrigger("Die");
		Destroy(gameObject);
	}

	public void TryMoveToHearth(){
		if(!isMoving)
			MoveTo(GameManager.Instance.Player.Buildings[0].transform.position);
	}

	public override void MoveTo(Vector2 pos) {
		MoveTo(pos, true);
	}

	void MoveTo(Vector2 pos, bool callByUser) {
		if (isMoving) {
			if (moveDesc != null) {
				LeanTween.cancel(moveDesc.id);
				moveDesc = null;
			}
			isMoving = false;
		}


		path = pathfinder.FindPath(transform.position, pos);
		if (path != null) {
			isMoving = true;
			pathPos = 0;
			Move();
		}
	}

	void Move() {
		if (path == null || pathPos == path.Length || !isMoving) {
			isMoving = false;
			return;
		}

		if ((((Vector2)(transform.position)) - path[pathPos]).x <= 0)
			transform.rotation = Quaternion.Euler(0, 0, 0);
		else
			transform.rotation = Quaternion.Euler(0, 180, 0);


		moveDesc = transform.LeanMove(path[pathPos], (((Vector2)(transform.position)) - path[pathPos]).magnitude / speed)
		.setOnComplete(() => {
			++pathPos;
			Move();
		});
	}

	void SetAlpha(SpriteRenderer image, float a) {
		Color color = image.color;
		color.a = a;
		image.color = color;
	}

	void Awake() {
		EventManager.AddOrRemoveBuildingsEvent += AddOrRemoveBuildingsEvent;
	}

	void OnDestroy() {
		EventManager.AddOrRemoveBuildingsEvent -= AddOrRemoveBuildingsEvent;
	}

	void AddOrRemoveBuildingsEvent(EventData ed) {
		if(pathfinder != null)
			TryMoveToHearth();
	}
}
