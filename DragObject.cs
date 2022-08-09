using Godot;
using System;

public class DragObject : Node2D {
	static DragObject currDragObj;
	private bool isDragging = false;
	private Caravaner.Animation animation;
	private Caravaner.Animation rotation;
	private Caravaner.Animation shadowAnim;
	private Caravaner.Physics2D physics;
	private Caravaner.Animation positionAnim;
	private Caravaner.Animation positionXAnim;
	private Caravaner.Animation positionYAnim;
	private Tween tween;
	private Vector2 mouseOffset = Vector2.Zero;
	private Sprite sprite;
	private Sprite shadowSprite;
	private DragDropHandler dragDropHandler;
	private IconContainer iconContainer;
	private bool mouseOver;

	public static bool HasDragObj() {
		return currDragObj != null;
	}

	public static bool IsDragging() { 
		if (currDragObj == null) {
			return false;
		}
		else {
			return currDragObj.isDragging;
		}
	}

	public override void _Ready() {
		animation = new Caravaner.Animation();
		rotation = new Caravaner.Animation();
		positionAnim = new Caravaner.Animation();
		positionXAnim = new Caravaner.Animation();
		positionYAnim = new Caravaner.Animation();
		shadowAnim = new Caravaner.Animation();
		physics = new Caravaner.Physics2D();
		tween = (Tween)GetNode("Tween");
		// Connect mouse enter/exit functions
		Connect("mouse_entered", this, nameof(OnMouseEntered));
		Connect("mouse_exited", this, nameof(OnMouseExited));
		physics.Set(0f, Position.y + 100, Vector2.Zero, Position, 0.5f, 0f);
		sprite = (Sprite)GetNode("Sprite");
		shadowSprite = (Sprite)GetNode("Shadow");
		dragDropHandler = (DragDropHandler)GetNode("/root/Main/DragDropHandler");
	}

	public void SetIconContainer(IconContainer iconContainer) {
		this.iconContainer = iconContainer;
	}

	public override void _Process(float delta) {

		if (Input.IsActionJustPressed("ui_click")) { 
			OnDrag();
		}
		else if (Input.IsActionJustReleased("ui_click")) { 
			OnDrop();
		}
		else { 
			HandleDrag();
		}
		if (mouseOver && currDragObj == null) {
			OnMouseEntered();
		}
		if (animation.IsAnimating()) { 
			HandleAnimation(delta);
		}
		if (rotation.IsAnimating()) {
			Rotation = rotation.Animate(delta);
		}
		if (shadowAnim.IsAnimating()) {
			float v = shadowAnim.Animate(delta);
			float s = 0.5f * (1f - v) + v;
			shadowSprite.Scale = new Vector2(s, s);
			shadowSprite.Position = new Vector2(shadowSprite.Position.x, 32f * v + 64f * (1f - v));
		}
		if (!isDragging) { 
			if (physics.Update(1.5f * delta)) { 
				Position = physics.position;
				//sprite.Scale = new Vector2(1f, Mathf.Clamp((physics.velocity.Length() / 100f), 1f, 1.5f));
				float v = Mathf.Abs(physics.position.y - physics.yLimit)/ Mathf.Abs(physics.initPosition.y - physics.yLimit);
				float s = 0.5f * v + (1f - v);
				float p = 64f * v + (1f - v) * 32f;
				shadowSprite.Position = new Vector2(shadowSprite.Position.x, p);
				shadowSprite.Scale = new Vector2(s, s);
			}
		}

		if (positionAnim.IsAnimating()) {
			float v = positionAnim.Animate(delta);
		}
		if (positionXAnim.IsAnimating()) {
			float v = positionXAnim.Animate(delta);
			Position = new Vector2(v, Position.y);
		}
		if (positionYAnim.IsAnimating()) {
			float v = positionYAnim.Animate(delta);
			Position = new Vector2(Position.x, v);
		}
	}

	public void AnimateToPosition(Vector2 from, Vector2 position) {
		//GD.Print(from, position);
		//positionXAnim.Start(from.x, position.x, 10, Caravaner.AnimType.Constant, false);
		//positionYAnim.Start(from.y, position.y, 10, Caravaner.AnimType.Constant, false);
		tween.InterpolateProperty(this, "position", from, position, 0.2f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
		tween.Start();
	}

	private void OnDrag() { 
		if (currDragObj == this) {
			isDragging = true;
			//animation.Stop();
			shadowAnim.Start(1f, 0f, 15f, Caravaner.AnimType.Constant, false);
			rotation.Start(sprite.Rotation, 0f, 7f, Caravaner.AnimType.Constant, false);
			if (iconContainer != null) { 
				iconContainer.RemoveIcon(this);
				iconContainer = null;
			}
			mouseOffset = Position - GetGlobalMousePosition();
		}
	}

	private void OnDrop() { 
		if (currDragObj == this) {
			isDragging = false;
			if (IconContainer.DropIn(this)) {
				//GD.Print("Dropped in a container!");
				physics.Stop();
				shadowSprite.Position = new Vector2(0, 32);
				shadowSprite.Scale = new Vector2(0, 0);
				//animation.Start(sprite.Scale.y, 1f, 15f, Caravaner.AnimType.Constant, false);
				//rotation.Start(sprite.Rotation, 0f, 7f, Caravaner.AnimType.Constant, false);
				rotation.Start(0.05f, 0.05f, 4f, Caravaner.AnimType.Sin, true);
				return;
			}
			OnMouseExited();
			Vector2 mSpeed = Input.GetLastMouseSpeed();
			mSpeed = new Vector2(Mathf.Clamp(mSpeed.x, -200f, 200f), Mathf.Clamp(mSpeed.y, -200f, 200f));
			physics.Set(900f, Position.y + 50, mSpeed, Position, 0.5f, 2f);
			shadowAnim.Start(shadowSprite.Scale.x, 1f, 8f, Caravaner.AnimType.Constant, false);
		}
	}

	public void Initialize(Vector2 startPosition, Vector2 velocity, float yLim) {
		Position = startPosition;
		physics.Set(900f, Position.y + yLim, velocity, Position, 0.5f, 2f);
	}

	private void HandleAnimation(float delta) {
		float v = animation.Animate(delta);
		sprite.Scale = new Vector2(v, v);
	}

	private void HandleDrag() { 
		if (currDragObj == this && isDragging) {
			Position = mouseOffset + GetGlobalMousePosition();
		}
	}

	public void OnMouseEntered() { 
		mouseOver = true;
		if (currDragObj == null) {
			DragObject.currDragObj = this;
			animation.Start(1f, 1.2f, 15f, Caravaner.AnimType.Constant, false);
			//animation.Start(1f, 1.15f, 3f, Caravaner.AnimType.Constant, true);
			rotation.Start(0.05f, 0.05f, 4f, Caravaner.AnimType.Sin, true);
		}
	}

	public void OnMouseExited() { 
		mouseOver = false;
		if (currDragObj == this && !isDragging) {
			DragObject.currDragObj = null;
			animation.Start(sprite.Scale.y, 1f, 15f, Caravaner.AnimType.Constant, false);
			rotation.Start(sprite.Rotation, 0f, 7f, Caravaner.AnimType.Constant, false);
		}
	}
}

namespace Caravaner { 
	public enum AnimType { 
		Constant,
		Sin,
		SinGrowth
	}

	public class Animation {
		float time;
		float start;
		float end;
		float speed;
		AnimType type;
		bool repeat; 

		public Animation() {
			time = 1f;
		}

		public void Start(float start, float end, float speed, AnimType type, bool repeat) {
			this.start = start;
			this.end = end;
			this.speed = speed;
			this.time = 0f;
			this.type = type;
			this.repeat = repeat;
		}

		public void Stop() {
			time = 1f;
		}

		public float Animate(float delta) {
			Advance(delta);
			switch (type) {
				case AnimType.Constant:
					return Constant();
				case AnimType.Sin:
					return Sin();
				case AnimType.SinGrowth:
					return SinGrowth();
				default:
					return 0f;
			}
		}

		public float T() {
			return time;
		}

		public float Constant() {
			return (1f - time) * start + time * end;
		}

		public float Sin() {
			float t = Mathf.Sin(2f*Mathf.Pi * time) ;
			return t* start;
		}

		public float SinGrowth() {
			float alpha = 1f;
			float beta = 2.05f;
			float gamma = 1f;
			float t = time * (alpha * Mathf.Sin(beta * Mathf.Pi * time) + gamma);
			return (1f - t) * start + t * end;
		}

		public void Advance(float delta) { 
			time = Mathf.Clamp(time + delta * speed, 0f, 1f);
			if (!IsAnimating() && repeat) {
				time = 0f;
				float temp = start;
				start = end;
				end = temp;
			}
		}

		public bool IsAnimating() {
			return time < 1f;
		}
	}

	public class Physics2D {
		public Vector2 initPosition;
		public Vector2 position;
		public Vector2 velocity;
		private Vector2 gravity = Vector2.Down;
		private float G;
		public float yLimit;
		private float drag;
		private float EPSILON = 0.00001f;
		private float timer;
		private float timeLimit;

		public Physics2D() {
			gravity = new Vector2(0, 1f);
		}

		public void Set(float G, float yLimit, Vector2 velocity, Vector2 position, float drag, float timeLimit) {
			initPosition = position;
			this.position = position;
			this.G = G;
			this.yLimit = yLimit;
			this.drag = drag;
			this.velocity = velocity;
			this.timeLimit = timeLimit;
			timer = 0;
		}

		public void Stop() {
			timer = timeLimit + 1;
		}

		public bool Update(float delta) {
			//if (velocity.Length() < EPSILON) {
			//	return;
			//}
			if (timer >= timeLimit) {
				return false;
			}
			timer += delta;
			velocity += G * delta * gravity;
			position += delta * velocity; 
			Impact(position);
			return true;
		}

		private void Impact(Vector2 newPosition) { 
			if (newPosition.y > yLimit) {
				position = new Vector2(position.x, yLimit);
				velocity = new Vector2(drag * velocity.x, -drag * velocity.y);
			}
		}
	}
}
