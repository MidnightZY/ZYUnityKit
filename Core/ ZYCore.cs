//------------------------------------------------------------
// ZYCell - Unity 的综合系统框架
// Copyright © 2019-2024 Alex Liu. All rights reserved.
// https://github.com/Skyrim07/ZYCell
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZYUnityKit
{
  // [DisallowMultipleComponent]
  [RequireComponent(typeof(ZYCommonTimer))]
  // [RequireComponent(typeof(ZYPoolManager))]
  [AddComponentMenu("ZYUnityKit/Core/ZYCore")]
  public sealed class ZYCore : ZYMonoSingleton<ZYCore>
  {
    public const string ZYCELL_VERSION = "v1.4.0";

    // 目的：统一生命周期、统一时间服务、减少散乱协程、让普通 C# 模块也能进入游戏循环
    public static Action Awake000 = new Action(EmptyAction), Awake100 = new Action(EmptyAction),
                          Start000 = new Action(EmptyAction), Start100 = new Action(EmptyAction), Start200 = new Action(EmptyAction),
                          Tick000 = new Action(EmptyAction), Tick100 = new Action(EmptyAction), Tick200 = new Action(EmptyAction), Tick300 = new Action(EmptyAction), Tick400 = new Action(EmptyAction), Tick500 = new Action(EmptyAction),
                          FixedTick000 = new Action(EmptyAction), FixedTick100 = new Action(EmptyAction), FixedTick200 = new Action(EmptyAction),
                          UnscaledTick000 = new Action(EmptyAction),
                          LateTick000 = new Action(EmptyAction), LateTick100 = new Action(EmptyAction), LateTick200 = new Action(EmptyAction),
                          OnSceneLoaded000 = new Action(EmptyAction), OnSceneLoaded100 = new Action(EmptyAction);

    // public static Dictionary<ZYModule, Type> moduleTypeDict = new Dictionary<ZYModule, Type>()
    //     {
    //         {ZYModule.CoreModule, null},
    //         {ZYModule.LocalizationModule, typeof(ZYLocalizationManager)},
    //     };

    /// <summary>
    /// Initialize ZYCell modules
    /// </summary>
    // private void InitializeZYCell()
    // {
    //   ZYInventory.Initialize();

    //   ZYUtils.EditorLogNormal("ZYCell Initialized!");
    // }

    #region Unity Lifecycle

    protected override void Awake()
    {
      base.Awake();
      // ZYUtils.ClearAllCustomMeshes();

      Awake000();
      Awake100();
    }
    private void Start()
    {
      // InitializeZYCell();
      Start000();
      Start100();
      Start200();
    }
    private void Update()
    {
      Tick000();
      Tick100();
      Tick200();
      Tick300();
      Tick400();
      Tick500();
      UnscaledTick000();
    }

    private void FixedUpdate()
    {
      FixedTick000();
      FixedTick100();
      FixedTick200();
    }

    private void LateUpdate()
    {
      LateTick000();
      LateTick100();
      LateTick200();
    }
    //private void OnLevelWasLoaded(int level)
    //{
    //    OnSceneLoaded000();
    //    OnSceneLoaded100();
    //}

    #endregion
    private static void EmptyAction() { }
  }

  /// <summary>
  /// Represents an ZY module manager class.
  /// </summary>
  public enum ZYModule
  {
    CoreModule,
    InputModule,
    MediaModule,
    EnvironmentModule,
    LocalizationModule,
    CSVModule,
    FSMModule,
    GridModule,
    ObjectPoolModule,
    TimeModule,
    PhysicsModule,
    UIModule,
    StructureModule
  }
}
