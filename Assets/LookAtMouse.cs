using System.Linq;
using UnityEngine;

public enum InputMode {
    Touch,
    Mouse
}


public class LookAtMouse : MonoBehaviour
{
    public bool locked = false;
    public float rotSpeed = 10f;
    public float zoomFactor = 0.1f;
    public float markerSize = 10f;
    private int DebounceTimer = 0;
    public int DebounceLimit = 5;
    public float TapDebounceMoveThreshold = 10;
    private bool MarkerPlaced = false;
    public GameObject markerPrefab;
    public InputMode inputMode = InputMode.Mouse;
    public DynamicDict<int, Vector2> TouchStart = new DynamicDict<int, Vector2>();
    public DynamicDict<int, Vector2> TouchPos = new DynamicDict<int, Vector2>();

    private void AddMarker(RaycastHit? hit) {
        if(!hit.HasValue) return;

        var marker = Instantiate(markerPrefab, hit.Value.point, Quaternion.identity, hit.Value.transform);
        marker.gameObject.layer = LayerMask.NameToLayer("Markers");
        marker.transform.localScale = new Vector3(markerSize, markerSize, markerSize)
                                        .Mul(Vector3.one - marker.transform.localScale.Div(marker.transform.lossyScale));
    }

    private void AddMarker(Vector2 screeenPos) {
        AddMarker(screeenPos.RayCast("Default"));
    }

    public void Zoom(float delta) {
        var scale = transform.localScale;
        scale.x += delta * zoomFactor;
        scale.y += delta * zoomFactor;
        scale.z += delta * zoomFactor;
        transform.localScale = Vector3.Max(Vector3.one, scale) ;
    }

    public void Rotate(Quaternion? newAngle) {
        if (!newAngle.HasValue) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, newAngle.Value, Time.deltaTime * rotSpeed);
    }

    public void LookAt(Vector3? target) {
        if (!target.HasValue) return;
        Rotate(Quaternion.LookRotation(target.Value));
    }

    void HandleMouseInput() {
        if (Input.touches.Length > 0) {
            inputMode = InputMode.Touch;
        }

        if(Input.GetMouseButtonDown(1)) {
            locked = !locked;
        }

        if(locked) {
            if (Input.GetMouseButtonDown(0)){
                AddMarker(Input.mousePosition);
            }
        } else {
            LookAt(Input.mousePosition.RayCast("Controls")?.point);
        }

        Zoom(Input.mouseScrollDelta.y);
    }

    void HandleTouchInput() {
        for (int i = 0; i < Input.touchCount; i++) {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began) {
                TouchStart[touch.fingerId] = touch.position;
                TouchPos[touch.fingerId] = touch.position;
            } else if (touch.phase == TouchPhase.Moved) {
                TouchPos[touch.fingerId] = touch.position;
                if((touch.position - TouchStart[touch.fingerId]).magnitude > TapDebounceMoveThreshold) {
                    DebounceLimit = 0;
                }
            } else if (touch.phase == TouchPhase.Ended) {
                TouchStart.Remove(touch.fingerId);
                MarkerPlaced = false;
                DebounceTimer = 0;
            }
        }
        switch(Input.touchCount) {
            case 1:
                if (!MarkerPlaced) {
                    if (DebounceTimer > DebounceLimit) {
                        AddMarker(Input.touches[0].position);
                        MarkerPlaced = true;
                    }
                    else 
                        DebounceTimer ++;
                }
                break;
            case 2:
                DebounceTimer = 0;

                var startOffset = TouchStart[0] - TouchStart[1];
                var endOffset = TouchPos[0] - TouchPos[1];
                var deltaOffset = endOffset - startOffset;
                var startCenter = (TouchStart[0] + TouchStart[1]) / 2;
                var endCenter = (TouchPos[0] + TouchPos[1]) / 2;
                var deltaCenter = endCenter - startCenter;

                Zoom(((endOffset.magnitude - startOffset.magnitude) / new Vector2(Screen.width, Screen.height).magnitude));
                LookAt(Input.touches[0].position.RayCast("Controls")?.point);
                break;
        }
    }

    void Update()
    {
        switch (inputMode) {
            case InputMode.Mouse:
                HandleMouseInput();
                break;
            case InputMode.Touch:
                HandleTouchInput();
                break;
        }
    }
}
