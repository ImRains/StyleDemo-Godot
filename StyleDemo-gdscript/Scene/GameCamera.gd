extends Camera3D
##旋转的目标
@export var target:Node3D
##控制x轴上的旋转速度，如速度为0，则禁用y轴旋转
@export var xSpeed:float=5
##控制y轴上的旋转速度，如速度为0，则禁用y轴旋转
@export var ySpeed:float=5
## Y轴最低
@export var yMinLimit:float = -0.8
## Y轴最高
@export var yMaxLimit:float = -0.6
## 当前视距
@export var distance:float = 70
## 视距滚动速度
@export var distanceSpeed:float = 100
## 最小视距
@export var minDistance:float = 20
## 最大视距
@export var maxDistance:float = 100
## 是否需要阻尼
@export var needDamping:bool = true
## 阻尼大小
@export var damping:float = 10
var mouseRightPress:bool=false
var mouseWheel:int = 0
var x:float = 0
var y:float = 0
var baseItems:Array[Node3D]

func _ready() -> void:
	x = transform.basis.get_euler().x;
	y = transform.basis.get_euler().y;
	print(x,y)
	var nodes = get_tree().get_nodes_in_group("faceCamera");
	for item in nodes:
		baseItems.append(item)

func _process(delta: float) -> void:
	UpdateMouseInput()
	UpdateItemRotation()
	var _rotation = Quaternion.from_euler(Vector3(y, x, 0))
	distance -= mouseWheel * distanceSpeed * delta
	distance = clamp(distance, minDistance, maxDistance)
	if needDamping:
		quaternion = quaternion.slerp(_rotation, delta * damping)
		position = position.lerp(transform.basis.z.normalized()*distance, delta * damping)
	else:
		print(_rotation)
		quaternion = _rotation
		position = transform.basis.z.normalized() * distance

func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and mouseRightPress:
		x += -event.relative.x * xSpeed * 0.0002
		y += -event.relative.y * ySpeed * 0.0002
		y = ClampAngle(y, yMinLimit, yMaxLimit)

func UpdateMouseInput():
	mouseRightPress = Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT)
	var isWheelUp = Input.is_action_just_released("mouseScrollUp")
	var isWheelDown = Input.is_action_just_released("mouseScrollDown")
	mouseWheel = 0 if !(isWheelUp || isWheelDown) else ( 1 if isWheelUp else -1 )

func UpdateItemRotation():
	for item in baseItems:
		item.quaternion = quaternion

func ClampAngle(angle:float, min:float, max:float):
	if angle < -1:
		angle += 1
	if angle > 1:
		angle -= 1
	return clamp(angle, min, max)
