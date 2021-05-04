using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Profiling;

public class PerformanceGUI : MonoBehaviour {
    int tick;

    long a, b;

    private void FixedUpdate() {
        tick++;

        if (tick > 20) {
            tick = 0;
            Refresh();
        }
    }

    public void Refresh() {
        a = Profiler.GetTotalReservedMemoryLong() / 1000000;
        b = Profiler.GetTotalAllocatedMemoryLong() / 1000000;
    }

    private void OnGUI() {
        GUI.TextArea(new Rect(10, 30, 250, 70), a + " Mb - all");
        GUI.TextArea(new Rect(10, 100, 250, 70), b + " Mb - used");
    }
}
