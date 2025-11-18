using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldInstructionCache
{
    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    private static readonly Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();
    private static readonly Dictionary<float, WaitForSecondsRealtime> waitForSecondsRealtime = new Dictionary<float, WaitForSecondsRealtime>();

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!waitForSeconds.TryGetValue(seconds, out wfs))
            waitForSeconds.Add(seconds, wfs = new WaitForSeconds(seconds));

        return wfs;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime wfs;
        if (!waitForSecondsRealtime.TryGetValue(seconds, out wfs))
            waitForSecondsRealtime.Add(seconds, wfs = new WaitForSecondsRealtime(seconds));
        return wfs;
    }
}
