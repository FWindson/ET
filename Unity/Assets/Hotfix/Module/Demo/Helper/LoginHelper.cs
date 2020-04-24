using System;
using ETModel;

namespace ETHotfix
{
    public static class LoginHelper
    {
        public static async ETVoid OnLoginAsync(string account)
        {
            try
            {
                // 创建一个ETModel层的Session
                // 该地址通过GlobalConfigComponent从Resources文件内加载挂载Prefab上的地址文件得来
                // 通过NetOuterComponent构建一个Session
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);
				
                // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
                // 通过ComponentFactory构建一个与RealmServer连接的Session
                //TODO:这里看不懂，为什么要先创建ETModel层的Session，再根据ETModel.Session创建一个ETHofix.Session，之后通过ETHotfix.Session来调用ETModel.Session发送消息
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                //发送一个类型为C2R_Login的消息到RealmServer进行登录验证，并异步等待
                R2C_Login r2CLogin = (R2C_Login) await realmSession.Call(new C2R_Login() { Account = account, Password = "111111" });
                //认证结束后释放与RealmServer的Session
                realmSession.Dispose();

                // 创建一个ETModel层的Session,并且保存到ETModel.SessionComponent中
                // 通过类型为R2C_Login的响应内携带的地址，创建一个与GateServer连接的Session
                //TODO:为什么Game都要分一个ETModel层和ETHotfix层？
                ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2CLogin.Address);
                ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;
				
                // 创建一个ETHotfix层的Session, 并且保存到ETHotfix.SessionComponent中
                Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
				
                //GateServer响应内携带了一个PlayerId，用于之后当前客户端与GateServer的通讯
                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

                Log.Info("登陆gate成功!");

                // 创建Player
                Player player = ETModel.ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
                //将Player实例存入PlayerComponent内
                PlayerComponent playerComponent = ETModel.Game.Scene.GetComponent<PlayerComponent>();
                playerComponent.MyPlayer = player;

                Game.EventSystem.Run(EventIdType.LoginFinish);

                // 测试消息有成员是class类型
                G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        } 
    }
}