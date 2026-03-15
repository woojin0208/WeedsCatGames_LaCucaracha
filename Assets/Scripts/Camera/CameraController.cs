using UnityEngine;

// 플레이어 위치를 기준으로 카메라 X 범위를 제한해 추적한다.
public class CameraController : MonoBehaviour
{
    private Transform target;

    [SerializeField]
    private float[] wallXPosition;
    private float offset = 4.17f;
    private void Update()
    {
        if (target == null) return;

        transform.position = new Vector3(Mathf.Clamp(target.position.x, (wallXPosition[0] + offset), (wallXPosition[1] - offset)), -1, -10);
    }
    public void SetPlayer(Transform player)
    {
        this.target = player;
    }
}