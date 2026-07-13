using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;

#endif

namespace ZYUnityKit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ZYButton))]
    public class ZYButtonEditor : Editor
    {
        static ZYButton zyButton;
        public override void OnInspectorGUI()
        {
            zyButton = (ZYButton)target; // 当前正在检查的对象
            // 生成标准结构按钮
            if ((!zyButton.initialized) && UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null)
                if (GUILayout.Button("<Generate Structure>", GUILayout.Height(30)))
                {
                    zyButton.GenerateStructure();
                }

            // 动画添加与删除
            if (zyButton.initialized || UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                if (zyButton.transitionMode == ZYButton.ZYButtonTransitionMode.Animation)
                {
                    GUILayout.Label("The animation template is automatically generated.");
                    if (!zyButton.hasAnimator && GUILayout.Button("<Generate Animator>", GUILayout.Height(30)))
                    {
                        CreateController();
                    }
                }
                else if (zyButton.hasAnimator)
                {
                    if (GUILayout.Button("<Delete Animation>", GUILayout.Height(30)))
                    {
                        DetachController();
                    }
                }

                // 显示真实序列化字段
                base.OnInspectorGUI();

                // 不在运行模式时，自动刷新按钮外观预览。
                if (!Application.isPlaying)
                    zyButton.DrawEditorPreview();
            }
        }

        private static void CreateController()
        {
            // 动画资源生成目录
            string path = ZYAssetLibrary.UI_ANIM_DIR_PATH + $"/{zyButton.name}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // 创建动画
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path + $"/{zyButton.name}.controller");
            AnimationClip normalClip = new AnimationClip();
            AssetDatabase.CreateAsset(normalClip, path + $"/Normal.anim");
            AnimationClip overClip = new AnimationClip();
            AssetDatabase.CreateAsset(overClip, path + $"/Over.anim");
            AnimationClip pressedClip = new AnimationClip();
            AssetDatabase.CreateAsset(pressedClip, path + $"/Pressed.anim");
            AnimationClip disabledClip = new AnimationClip();
            AssetDatabase.CreateAsset(disabledClip, path + $"/Disabled.anim");
            AnimationClip hold1Clip = new AnimationClip();
            AssetDatabase.CreateAsset(hold1Clip, path + $"/Hold1.anim");
            AnimationClip hold2Clip = new AnimationClip();
            AssetDatabase.CreateAsset(hold2Clip, path + $"/Hold2.anim");
            zyButton.AttachController(controller);

            // 添加参数
            controller.AddParameter("Normal", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Pressed", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Over", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Disabled", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("HoldForSeconds1", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("HoldForSeconds2", AnimatorControllerParameterType.Trigger);

            // 添加状态机
            var rootStateMachine = controller.layers[0].stateMachine;

            // Add States
            var normalState = rootStateMachine.AddState("Normal");
            normalState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Normal.anim");
            var overState = rootStateMachine.AddState("Over");
            overState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Over.anim");
            var pressedState = rootStateMachine.AddState("Pressed");
            pressedState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Pressed.anim");
            var disabledState = rootStateMachine.AddState("Disabled");
            disabledState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Disabled.anim");
            var hold1State = rootStateMachine.AddState("HoldForSeconds1");
            hold1State.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Hold1.anim");
            var hold2State = rootStateMachine.AddState("HoldForSeconds2");
            hold2State.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Hold2.anim");

            rootStateMachine.defaultState = normalState;

            // 添加过渡Transition
            var normalTransition = rootStateMachine.AddAnyStateTransition(normalState);
            normalTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Normal");
            normalTransition.duration = 0.1f;

            var overTransition = rootStateMachine.AddAnyStateTransition(overState);
            overTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Over");
            overTransition.duration = 0.1f;

            var pressedTransition = rootStateMachine.AddAnyStateTransition(pressedState);
            pressedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Pressed");
            pressedTransition.duration = 0.1f;

            var disabledTransition = rootStateMachine.AddAnyStateTransition(disabledState);
            disabledTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Disabled");
            disabledTransition.duration = 0.1f;

            var hold1Transition = rootStateMachine.AddAnyStateTransition(hold1State);
            hold1Transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "HoldForSeconds1");
            hold1Transition.duration = 0.1f;

            var hold2Transition = rootStateMachine.AddAnyStateTransition(hold2State);
            hold2Transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "HoldForSeconds2");
            hold2Transition.duration = 0.1f;

        }

        // 删除动画
        private void DetachController()
        {
            zyButton.DetachController();
            string path = ZYAssetLibrary.UI_ANIM_DIR_PATH + $"/{zyButton.name}";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
#endif
}
