using Godot;
using System;

public partial class CameraController : Node3D
{
	[ExportGroup("Camera Settings")]
	[Export] public float Height = 0.6f;
	[Export] public float Distance = 0.0f; // 0 untuk first person
	[Export] public float VerticalAngle = 0.0f; // Sudut vertical dalam derajat
	
	private Camera3D _camera;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		UpdateCameraPosition();
	}

	public override void _Process(double delta)
	{
		UpdateCameraPosition();
	}

	private void UpdateCameraPosition()
	{
		if (_camera == null) return;
		
		// Set posisi kamera berdasarkan Height dan Distance
		Vector3 localPos = new Vector3(0, Height, Distance);
		_camera.Position = localPos;
		
		// Set rotasi vertical
		_camera.Rotation = new Vector3(Mathf.DegToRad(VerticalAngle), 0, 0);
	}

	// Method untuk mengubah height secara dinamis
	public void SetHeight(float newHeight)
	{
		Height = newHeight;
		UpdateCameraPosition();
	}

	// Method untuk mengubah distance secara dinamis
	public void SetDistance(float newDistance)
	{
		Distance = newDistance;
		UpdateCameraPosition();
	}

	// Method untuk mengubah vertical angle secara dinamis
	public void SetVerticalAngle(float newAngle)
	{
		VerticalAngle = newAngle;
		UpdateCameraPosition();
	}
}
