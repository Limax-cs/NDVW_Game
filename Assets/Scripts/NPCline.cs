using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NPCline : MonoBehaviour {
    private LineRenderer locationLine;

    void Start() {
        locationLine = GetComponent<LineRenderer>();
        locationLine.positionCount = 2;  // Two points (start and end of the line)
        locationLine.startWidth = 0.1f;  // Or any other width you prefer
        locationLine.endWidth = 0.1f;
    }

    void Update() {
        Vector3 npcPosition = transform.position;
        Vector3 skyPoint = npcPosition + Vector3.up * 100;  // 10 units above the NPC

        locationLine.SetPosition(0, npcPosition);
        locationLine.SetPosition(1, skyPoint);
    }
}
