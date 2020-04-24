using System;
using ETModel;

namespace ETHotfix
{
    public static class MapHelper
    {
        /// <summary>
        /// 异步连接MapServer
        /// </summary>
        /// <returns></returns>
        public static async ETVoid EnterMapAsync()
        {
            try
            {
                // 加载Unit资源，创建一个骷髅兵单位
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                await resourcesComponent.LoadBundleAsync($"unit.unity3d");

                // 加载场景资源
                await ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundleAsync("map.unity3d");
                // 切换到map场景
                using (SceneChangeComponent sceneChangeComponent = ETModel.Game.Scene.AddComponent<SceneChangeComponent>())
                {
                    await sceneChangeComponent.ChangeSceneAsync(SceneType.Map);
                }
				
                // 加载完地图场景后，发送一个类型为C2G_EnterMap的消息到GateServer，表示加入地图内
                // 如果加入MapServer过程中没有异常，则会返回一个类型为G2C_EnterMap的消息，里面携带着MapServer上所有单位的信息：UnitInfo[] Units
                G2C_EnterMap g2CEnterMap = await ETModel.SessionComponent.Instance.Session.Call(new C2G_EnterMap()) as G2C_EnterMap;
                //将该单位的ID保存到PlayerComponent内的Player实例中
                PlayerComponent.Instance.MyPlayer.UnitId = g2CEnterMap.UnitId;
				
                //添加一个操作管理组件
                Game.Scene.AddComponent<OperaComponent>();
				
                Game.EventSystem.Run(EventIdType.EnterMapFinish);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }	
        }
    }
}