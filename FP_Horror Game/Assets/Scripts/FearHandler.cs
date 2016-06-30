using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class FearHandler : MonoBehaviour
{
    public enum Fear { None, Low, Normal, High }
    public Fear fear = Fear.None;
    public float fearMeter;

    [Header("Camera Effects")]
    public Fisheye fishEye;
    [Range(0, 1.5f)]
    public float maxFishEye = .75f;
    public MotionBlur motionBlur;
    [Range(0, 1f)]
    public float maxMotionBlur = .5f;


    void Update()
    {
        fearMeter = Mathf.Clamp(fearMeter, 0, 100); //får bara vara mellan 0-100 och det kommer aldrig överstiga det!

        if (Input.GetKeyDown(KeyCode.F))
            fear = Fear.None;
        if (Input.GetKeyDown(KeyCode.G))
            fear = Fear.Low;
        if (Input.GetKeyDown(KeyCode.H))
            fear = Fear.Normal;
        if (Input.GetKeyDown(KeyCode.J))
            fear = Fear.High;

        CameraEffects(); //Update the effects there. Cleaner in the update function!

        switch (fear)
        {
            case Fear.None:

                break;
            case Fear.Low:

                break;
            case Fear.Normal:

                break;
            case Fear.High:

                break;
        }
    }

    void CameraEffects()
    {
        switch (fear)
        {
            case Fear.None:
                fishEye.strengthX = Mathf.Lerp(fishEye.strengthX, 0, Time.deltaTime);
                fishEye.strengthY = Mathf.Lerp(fishEye.strengthY, 0, Time.deltaTime);
                motionBlur.blurAmount = Mathf.Lerp(motionBlur.blurAmount, 0, Time.deltaTime);
                break;
            case Fear.Low:
                fishEye.strengthX = Mathf.Lerp(fishEye.strengthX, maxFishEye / 3, Time.deltaTime);
                fishEye.strengthY = Mathf.Lerp(fishEye.strengthY, maxFishEye / 3, Time.deltaTime);
                motionBlur.blurAmount = Mathf.Lerp(motionBlur.blurAmount, maxMotionBlur / 3, Time.deltaTime);
                break;
            case Fear.Normal:
                fishEye.strengthX = Mathf.Lerp(fishEye.strengthX, maxFishEye / 2, Time.deltaTime);
                fishEye.strengthY = Mathf.Lerp(fishEye.strengthY, maxFishEye / 2, Time.deltaTime);
                motionBlur.blurAmount = Mathf.Lerp(motionBlur.blurAmount, maxMotionBlur / 2, Time.deltaTime);
                break;
            case Fear.High:
                fishEye.strengthX = Mathf.Lerp(fishEye.strengthX, maxFishEye, Time.deltaTime);
                fishEye.strengthY = Mathf.Lerp(fishEye.strengthY, maxFishEye, Time.deltaTime);
                motionBlur.blurAmount = Mathf.Lerp(motionBlur.blurAmount, maxMotionBlur, Time.deltaTime);
                break;
        }
    }
}
