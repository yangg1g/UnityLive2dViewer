using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCamera : MonoBehaviour
{
    public Toggle MoveCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!MoveCamera.isOn)
            return;
        //�����ֵ�Ч��
        //Camera.main.fieldOfView ���������Ұ
        //Camera.main.orthographicSize �����������ͶӰ
        //Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += 0.1f;
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.orthographicSize > 0)
                Camera.main.orthographicSize -= 0.1f;
        }

        Vector3 camFollowPos = this.transform.position;
        float edgeSize = 20f;
        float moveAmount = 0.5f;

        if (Input.mousePosition.x < Screen.width && Input.mousePosition.x > Screen.width - edgeSize && camFollowPos.x < 1)//������λ�����Ҳ�
        {
            camFollowPos.x += moveAmount * Time.deltaTime;//�������ƶ�
        }
        if (Input.mousePosition.x > 0 && Input.mousePosition.x < edgeSize && camFollowPos.x > -1)
        {
            camFollowPos.x -= moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.y < Screen.height && Input.mousePosition.y > Screen.height - edgeSize && camFollowPos.y < 1)
        {
            camFollowPos.y += moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.y > 0 && Input.mousePosition.y < edgeSize && camFollowPos.y > -1)
        {
            camFollowPos.y -= moveAmount * Time.deltaTime;
        }
        this.transform.position = camFollowPos;//ˢ�������λ��

    }
}
