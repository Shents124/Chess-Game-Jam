using UnityEngine;

namespace Gameplay.Runtime.GameplayCamera
{
    [ExecuteAlways]
    public class AdjustOrthographicCamera : MonoBehaviour
    {
        public float targetAspectRatio = 9f / 16f; // thiết kế gốc
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float originOrthographic = 12.88889f;

        void Start()
        {
            AdjustCamera();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (!Application.isPlaying)
                AdjustCamera(); // Cho phép thấy thay đổi khi chỉnh trong editor
        }
#endif

        void AdjustCamera()
        {
            float currentAspect = (float)Screen.width / Screen.height;
            
            if (currentAspect >= targetAspectRatio)
            {
                // Màn hình rộng hơn => Giữ chiều cao, thêm vùng ngang
                mainCamera.orthographicSize = originOrthographic; // ví dụ: chiều cao chuẩn là 10 units
            }
            else
            {
                // Màn hình hẹp hơn => mở rộng chiều cao để giữ chiều ngang
                float scale = targetAspectRatio / currentAspect;
                mainCamera.orthographicSize = originOrthographic * scale;
            }
        }
    }
}