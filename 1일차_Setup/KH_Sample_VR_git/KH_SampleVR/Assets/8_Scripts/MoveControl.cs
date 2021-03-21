using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    //트랜스폼 배열을 points에 저장
    public Transform[] points;

    //컴포넌트 저장 변수
    private Transform tr;
    private int nextIndex = 1;

    //움직이는 방법으로 
    //1. 웨이포인트를 따라간다
    //2. 쳐다보는 쪽으로 움직인다
    //3. 데이드림(No!)
    public enum MoveType
    {
        WAYPOINT,
        LOOKAT,
        DAYDREAM
    }

    //지금 움직이는 방식 = 웨이포인트 방식이다.
    public MoveType moveType = MoveType.WAYPOINT;

    //이동속도
    public float moveSpeed = 2.0f;

    //회전속도
    public float damping = 3.0f;

    public int lookatSpeed = 3;


    //바라보는 방향으로 움직임
    private CharacterController cc;

    //메인카메라의 트랜스폼을 저장할 변수
    private Transform mainCameraTransform;


    // Start is called before the first frame update
    void Start()
    {
        //기초 연결
        tr = GetComponent<Transform>();

        cc = GetComponent<CharacterController>();

        mainCameraTransform = Camera.main.GetComponent<Transform>();


        GameObject waypointGroup = GameObject.Find("WaypointGroup");

        if(waypointGroup != null)
        {
            points = waypointGroup.GetComponentsInChildren<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (moveType)
        {
            case MoveType.WAYPOINT:
                MoveWayPoint();
                break;

            case MoveType.LOOKAT:
                MoveLookat(lookatSpeed);
                break;

            case MoveType.DAYDREAM:
                break;
        }
    }

    public void MoveWayPoint()
    {
        Vector3 direction = points[nextIndex].position - tr.position;

        Quaternion userRotation = Quaternion.LookRotation(direction);

        tr.rotation = Quaternion.Slerp(tr.rotation, userRotation, Time.deltaTime * damping);

        tr.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
    }

    public void MoveLookat(int facing)
    {
        Vector3 heading = mainCameraTransform.forward;

        //지면 기준의 벡터로 전환(constraint)
        heading.y = 0f;

        Debug.DrawRay(tr.position, heading.normalized * 1.0f, Color.gray);

        cc.SimpleMove(heading * moveSpeed * facing);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WAYPOINT"))
        {
            //3항식
            nextIndex = (++nextIndex >= points.Length) ? 1 : nextIndex;
        }
    }
}
