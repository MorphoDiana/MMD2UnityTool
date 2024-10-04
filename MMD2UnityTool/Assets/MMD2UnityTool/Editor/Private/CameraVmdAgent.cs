using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class CameraVmdAgent
{
    string _vmdFile;
    MMD.VMD.VMDFormat format_;

	public CameraVmdAgent(string filePath)
    {
        _vmdFile = filePath;

    }

    public void CreateAnimationClip()
    {
		if (null == format_)
		{
			format_ = VMDLoaderScript.Import(_vmdFile);
		}

		AnimationClip animation_clip = VMDCameraConverter.CreateAnimationClip(format_);
		if (animation_clip == null)
		{
			throw new System.Exception("Cannot create AnimationClip");
		}

		string vmd_folder = Path.GetDirectoryName(_vmdFile);
		string anima_file = Path.Combine(vmd_folder, animation_clip.name + ".anim");

		if (File.Exists(anima_file))
			AssetDatabase.DeleteAsset(anima_file);

		AssetDatabase.CreateAsset(animation_clip, anima_file);
		
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animation_clip));



        GameObject mmdCamera = new GameObject("MMDCamera");

        Animator animator = mmdCamera.AddComponent<Animator>();
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(anima_file);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(Path.Combine(vmd_folder, animation_clip.name + ".controller"));
        AnimatorState state = controller.AddMotion(clip);
        animator.runtimeAnimatorController = controller;

        GameObject distance = new GameObject("Distance");
        distance.transform.parent = mmdCamera.transform;

        GameObject camera = new GameObject("Camera");
        camera.transform.parent = distance.transform;
        camera.AddComponent<Camera>();

        camera.transform.rotation = Quaternion.Euler(0, 180, 0);

        string prefabPath = Path.Combine(vmd_folder,"MMDCamera.prefab");
        PrefabUtility.SaveAsPrefabAsset(mmdCamera, prefabPath);

        GameObject.DestroyImmediate(mmdCamera);

        //AssetDatabase.Refresh();
    }
}
