using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessController : MonoBehaviour
{
    private float VIGNETTE_DEFAULT_INTENSITY = 0.12f;
    private float VIGNETTE_DEFAULT_SMOOTH = .35f;
    private float VIGNETTE_SCARED_INTENSITY = 0.18f;
    private float VIGNETTE_SCARED_SMOOTH = 1f;

    private float GRAIN_DEFAULT_INTENSITY = 0f;
    private float GRAIN_SCARED_INTENSITY = 0.4f;

    private float DOF_DEFAULT_LENGTH = 1f;
    private float DOF_SCARED_LENGTH = 60f;

    private float GRADING_DEFAULT_SAT = 10;
    private float GRADING_DEFAULT_CONTRAST=25;
    private float GRADING_SCARED_SAT=-85;
    private float GRADING_SCARED_CONTRAST=55;
    private ColorGrading grading;
    private Vignette vignette;
    private Grain grain;

    void Awake(){
        Messenger<int>.AddListener(GameEvent.FEAR_CHANGED, FearValueChnaged);

        var pp = GetComponent<PostProcessVolume>();
        //dof = pp.profile.GetSetting<DepthOfField>();
        grading =  pp.profile.GetSetting<ColorGrading>();
        vignette =  pp.profile.GetSetting<Vignette>();
        grain =  pp.profile.GetSetting<Grain>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //grain.active = false;
    }

    void OnDestroy(){
        Messenger<int>.RemoveListener(GameEvent.FEAR_CHANGED, FearValueChnaged);
    }
    // private IEnumarator ChangeToScared(){

    private void FearValueChnaged(int actualFear){
            float fear = actualFear/100f;
            vignette.intensity.value  = Mathf.Lerp(VIGNETTE_DEFAULT_INTENSITY,   VIGNETTE_SCARED_INTENSITY,  fear);
            vignette.smoothness.value = Mathf.Lerp(VIGNETTE_DEFAULT_SMOOTH, VIGNETTE_SCARED_SMOOTH,         fear);
            grading.contrast.value    = Mathf.Lerp(GRADING_DEFAULT_CONTRAST,   GRADING_SCARED_CONTRAST,        fear);
            grading.saturation.value  = Mathf.Lerp(GRADING_DEFAULT_SAT,      GRADING_SCARED_SAT,             fear);
            grain.intensity.value     = Mathf.Lerp(GRAIN_DEFAULT_INTENSITY,     GRAIN_SCARED_INTENSITY,         fear);
    }
    // Update is called once per frame
    void Update()
    {
        // if(GameEvent.chasingSet.Count > 0){
        //     //dof.active = true;
        //     //grain.active= true;
        //     vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, VIGNETTE_SCARED_INTENSITY, Time.deltaTime*speed);
        //     vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, VIGNETTE_SCARED_SMOOTH, Time.deltaTime*speed);
        //     grading.contrast.value = Mathf.Lerp(grading.contrast.value, GRADING_SCARED_CONTRAST, Time.deltaTime*speed);
        //     grading.saturation.value = Mathf.Lerp(grading.saturation.value, GRADING_SCARED_SAT, Time.deltaTime*speed);
        //     //dof.focalLength.value = Mathf.Lerp(dof.focalLength.value, DOF_SCARED_LENGTH, Time.deltaTime*speed);
        //     grain.intensity.value = Mathf.Lerp(grain.intensity.value, GRAIN_SCARED_INTENSITY, Time.deltaTime*speed);

        // }else{
        //     //dof.active = false;
        //     //grain.active = false;
        //     //dof.focalLength.value = Mathf.Lerp(dof.focalLength.value, DOF_DEFAULT_LENGTH, Time.deltaTime*speed);
        //     vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, VIGNETTE_DEFAULT_INTENSITY, Time.deltaTime*speed);
        //     vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, VIGNETTE_DEFAULT_SMOOTH, Time.deltaTime*speed);
        //     grading.contrast.value = Mathf.Lerp(grading.contrast.value, GRADING_DEFAULT_CONTRAST, Time.deltaTime*speed);
        //     grading.saturation.value = Mathf.Lerp(grading.saturation.value, GRADING_DEFAULT_SAT, Time.deltaTime*speed);
        //     grain.intensity.value = Mathf.Lerp(grain.intensity.value,GRAIN_DEFAULT_INTENSITY, Time.deltaTime*speed);
        // }
    }
}
