using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class PlayerMove : CharacterBody3D
{
	[Export]
	private float INITIAL_VELOCITY = 1.0f;
	[Export]
	private float MAXIMUM_SPEED = 5.0f;
	[Export]
	private float BOOST = 0.1f;
	[Export]
	private float BRAKING = 0.1f;
	[Export]
	private float JUMP_VELOCITY = 4.5f;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion e)
		{
			var dx = -e.Relative.X * 0.005;
			var dy = -e.Relative.Y * 0.005;
			RotateY((float)dx);

			Camera3D cam = FindChild("Camera3D") as Camera3D;
			cam.Rotation = new Vector3((float)Mathf.Clamp(cam.Rotation.X + dy, -Math.PI / 2, Math.PI / 2), 0, 0);
		}
	}



	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity += GetGravity() * (float)delta;


		// Handle Jump.
		if (Input.IsActionJustPressed("SPACE") && IsOnFloor())
			velocity.Y = JUMP_VELOCITY;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("MOVE_LEFT", "MOVE_RIGHT", "MOVE_FORWARD", "MOVE_BACKWARD");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{

			if (velocity.X == 0)
				velocity.X = direction.X * INITIAL_VELOCITY;
			if (velocity.Z == 0)
				velocity.Z = direction.Z * INITIAL_VELOCITY;

			if (direction.X<0 && velocity.X>0 || direction.X>0 && velocity.X<0)
				velocity.X=Mathf.MoveToward(velocity.X, 0, BRAKING);
			else
				velocity.X=direction.X * Mathf.MoveToward(Mathf.Abs(Velocity.X), MAXIMUM_SPEED, BOOST);

			if (direction.Z<0 && velocity.Z>0 || direction.Z>0 && velocity.Z<0)
				velocity.Z = Mathf.MoveToward(velocity.Z, 0, BRAKING);
			else
				velocity.Z = direction.Z * Mathf.MoveToward(Mathf.Abs(Velocity.Z), MAXIMUM_SPEED, BOOST);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, BRAKING);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, BRAKING);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

}
