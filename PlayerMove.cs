using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class PlayerMove : CharacterBody3D
{
	[Export]
	private float _initialVelocity = 5.0f;

	private float _currentVelocity = 0f;

	[Export]
	private float _acceleration = 13.0f;

	[Export]
	private float _jumpVelocity = 6.5f;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseNotion)
		{
			var dx = -eventMouseNotion.Relative.X * 0.005;
			var dy = -eventMouseNotion.Relative.Y * 0.005;
			RotateY((float)dx);

			Camera3D cam = FindChild("Camera3D") as Camera3D;
			cam.Rotation = new Vector3((float)Mathf.Clamp(cam.Rotation.X + dy, -Math.PI / 2, Math.PI / 2), 0, 0);
		}
		if (@event is InputEventKey inputEventKey)
		{
			switch (inputEventKey.Keycode)
			{
				case Key.Escape:
					Input.MouseMode = Input.MouseModeEnum.Visible;
					break;
				case Key.L:
					Input.MouseMode = Input.MouseModeEnum.Captured;
					break;
			}
		}
	}



	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity += GetGravity() * (float)delta;

		if (Input.IsActionJustPressed("SPACE") && IsOnFloor())
			velocity.Y = _jumpVelocity;

		Vector2 inputDir = Input.GetVector("MOVE_LEFT", "MOVE_RIGHT", "MOVE_FORWARD", "MOVE_BACKWARD");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			if (Input.IsActionPressed("SHIFT") && inputDir.Y < 0 && IsOnFloor())
				_currentVelocity = _acceleration;
			else
				_currentVelocity = _initialVelocity;

			velocity.X = direction.X * _currentVelocity;
			velocity.Z = direction.Z * _currentVelocity;
		}
		else if(IsOnFloor())
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _acceleration);
			velocity.Z = Mathf.MoveToward(Velocity.X, 0, _acceleration);
		}

		GD.Print($"{velocity.X} | {velocity.Z}");

		Velocity = velocity;
		MoveAndSlide();
	}

}
