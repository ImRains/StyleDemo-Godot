using Godot;
using System;
using System.Collections.ObjectModel;
using System.Linq;

public partial class Camera3D : Godot.Camera3D
{
    [Export]
    public Node3D target; //旋转的目标
    [Export]
    public float xSpeed = 5f; //控制y轴上的旋转速度，如速度为0，则禁用y轴旋转
    [Export]
    public float ySpeed = 5f; //控制y轴上的旋转速度，如速度为0，则禁用y轴旋转
    [Export]
    public float yMinLimit = -0.8f; // Y轴最低
    [Export]
    public float yMaxLimit = -0.6f; // Y轴最高
    [Export]
    public float distance = 70; // 当前视距
    [Export]
    public float distanceSpeed = 100; // 视距滚动速度
    [Export]
    public float minDistance = 20; //最小视距
    [Export]
    public float maxDistance = 100; //最大视距

    [Export]
    public bool needDamping = false; //是否需要阻尼
    [Export]
    float damping = 5.0f; // 阻尼大小
    private bool mouseRightPress = false;
    private int mouseWheel = 0;
    private float x = 0.0f;
    private float y = 0.0f;
    private Node3D[] baseItems;
    public override void _Ready()
    {
        base._Ready();
        x = Transform.Basis.GetEuler().X;
        y = Transform.Basis.GetEuler().Y;
        
        var nodes = GetTree().GetNodesInGroup("faceCamera");
        baseItems = new Node3D[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            baseItems[i] = nodes[i] as Node3D;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateMouseInput();
        UpdateItemRotation();
        Quaternion rotation = Quaternion.FromEuler(new Vector3(y, x, 0.0f));
        distance -= mouseWheel * distanceSpeed * (float)delta;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        if (needDamping)
        {
            Quaternion = Quaternion.Slerp(rotation, (float)delta * damping);
            Position = Position.Lerp(Transform.Basis.Z.Normalized() * distance, (float)delta * damping);
        }
        else
        {
            Quaternion = rotation;
            Position = Transform.Basis.Z.Normalized() * distance;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        // 按住鼠标右键并滑动
        if (@event is InputEventMouseMotion && mouseRightPress)
        {
            InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
            x += -mouseMotion.Relative.X * xSpeed * 0.0002f;
            y += -mouseMotion.Relative.Y * ySpeed * 0.0002f;
            y = ClampAngle(y, yMinLimit, yMaxLimit);
        }
    }

    public void UpdateMouseInput(){
        mouseRightPress = Input.IsMouseButtonPressed(MouseButton.Right);
        bool isWheelUp = Input.IsActionJustReleased("mouseScrollUp");
        bool isWheelDown = Input.IsActionJustReleased("mouseScrollDown");
        mouseWheel = !(isWheelUp || isWheelDown) ? 0 : ( isWheelUp ? 1 : -1 );
    }

    public void UpdateItemRotation(){
        foreach (var item in baseItems)
        {
            item.Quaternion = Quaternion;
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -1)
            angle += 1;
        if (angle > 1)
            angle -= 1;
        return Mathf.Clamp(angle, min, max);
    }
}
