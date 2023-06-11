using UnityEngine;
 
public class TPSCamera : MonoBehaviour
{
    float rotateSpeed = 4f;
    float smoothing = 5f;
    float maxY = 50f;
    float minY = -40f;
    float radius = 1.5f;            //离目标对象的距离
    float height = 2.5f;          //离目标对象的高度

    Camera MainCamera;            // 控制的相机
    Transform player;
    Vector3 rotate;
    RaycastHit hit;
 
    void Start()
    {
        MainCamera = this.GetComponent<Camera>();
        player = this.transform.parent;
    }
 
    void Update()
    {
        if(!GameSetting.isBegin)
            return;
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");
        rotate.x += inputX * rotateSpeed;
        rotate.y += inputY * rotateSpeed;
 
        if (rotate.x >= 360 || rotate.x <= -360)
            rotate.x = 0;
 
        // 仰角和俯角的角度限制
        if (rotate.y < minY)
            rotate.y = minY;
        else if (rotate.y > maxY)
            rotate.y = maxY;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
 
    void FixedUpdate()
    {
        Vector3 startPosition = transform.position;         // 相机开始位置
        Vector3 endPosition = Vector3.zero;                 // 相机最终位置
 
        Vector3 targetPos = player.position;
        targetPos.y += height;
 
        //旋转y轴，左右滑动
        float radian = -rotate.x * (Mathf.PI / 180);        // 弧度=角度*Math.PI/180
        float x = radius * Mathf.Cos(radian);
        float y = radius * Mathf.Sin(radian);
        endPosition = targetPos + new Vector3(x, 0, y);
        
        //更新角色旋转
        Quaternion q = Quaternion.LookRotation(targetPos - endPosition);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, q, Time.deltaTime * smoothing);
 
        // 防相机穿墙检测
        if (Physics.Linecast(targetPos, endPosition, out hit))
        {
            string name = hit.collider.gameObject.tag;
            if (name != "MainCamera" || name != "Player")
                endPosition = hit.point - (endPosition - hit.point).normalized * 0.2f;
        }

        endPosition.y = startPosition.y;
        transform.position = Vector3.Lerp(startPosition, endPosition, Time.deltaTime * smoothing);
        transform.eulerAngles = new Vector3(-rotate.y, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
