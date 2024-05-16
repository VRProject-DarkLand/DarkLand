using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessController : MonoBehaviour
{
    private float VIGNETTE_DEFAULT_INTENSITY = 0.25f;
    private float VIGNETTE_DEFAULT_SMOOTH = .35f;
    private float VIGNETTE_SCARED_INTENSITY = 0.18f;
    private float VIGNETTE_SCARED_SMOOTH = 1f;

    private float GRAIN_DEFAULT_INTENSITY = 0f;
    private float GRAIN_SCARED_INTENSITY = 0.4f;

    private float DOF_DEFAULT_LENGTH = 1f;
    private float DOF_SCARED_LENGTH = 60f;

    private float GRADING_DEFAULT_SAT = 10;
    private float GRADING_DEFAULT_CONTRAST=35;
    private float GRADING_SCARED_SAT=-85;
    private float GRADING_SCARED_CONTRAST=65;

    private float speed = 3f;
    private PostProcessVolume volume;
    private DepthOfField dof;
    private ColorGrading grading;
    private Vignette vignette;
    private Grain grain;
    // Start is called before the first frame update
    void Start()
    {
        var pp = GetComponent<PostProcessVolume>();
        //dof = pp.profile.GetSetting<DepthOfField>();
        grading =  pp.profile.GetSetting<ColorGrading>();
        vignette =  pp.profile.GetSetting<Vignette>();
        grain =  pp.profile.GetSetting<Grain>();
        //grain.active = false;
    }

    // private IEnumarator ChangeToScared(){

    
    // Update is called once per frame
    void Update()
    {
        if(GameEvent.chasingSet.Count > 0){
            //dof.active = true;
            //grain.active= true;
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, VIGNETTE_SCARED_INTENSITY, Time.deltaTime*speed);
            vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, VIGNETTE_SCARED_SMOOTH, Time.deltaTime*speed);
            grading.contrast.value = Mathf.Lerp(grading.contrast.value, GRADING_SCARED_CONTRAST, Time.deltaTime*speed);
            grading.saturation.value = Mathf.Lerp(grading.saturation.value, GRADING_SCARED_SAT, Time.deltaTime*speed);
            //dof.focalLength.value = Mathf.Lerp(dof.focalLength.value, DOF_SCARED_LENGTH, Time.deltaTime*speed);
            grain.intensity.value = Mathf.Lerp(grain.intensity.value, GRAIN_SCARED_INTENSITY, Time.deltaTime*speed);

        }else{
            //dof.active = false;
            //grain.active = false;
            //dof.focalLength.value = Mathf.Lerp(dof.focalLength.value, DOF_DEFAULT_LENGTH, Time.deltaTime*speed);
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, VIGNETTE_DEFAULT_INTENSITY, Time.deltaTime*speed);
            vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, VIGNETTE_DEFAULT_SMOOTH, Time.deltaTime*speed);
            grading.contrast.value = Mathf.Lerp(grading.contrast.value, GRADING_DEFAULT_CONTRAST, Time.deltaTime*speed);
            grading.saturation.value = Mathf.Lerp(grading.saturation.value, GRADING_DEFAULT_SAT, Time.deltaTime*speed);
            grain.intensity.value = Mathf.Lerp(grain.intensity.value,GRAIN_DEFAULT_INTENSITY, Time.deltaTime*speed);
        }
    }
}
