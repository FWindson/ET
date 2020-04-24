namespace ETModel
{
	[ObjectSystem]
	public class GlobalConfigComponentAwakeSystem : AwakeSystem<GlobalConfigComponent>
	{
		public override void Awake(GlobalConfigComponent t)
		{
			t.Awake();
		}
	}

	/// <summary>
	/// 服务器和资源服务器地址配置组件
	/// 用于加载Resources目录下的Prefab身上挂载的GlobalProto配置文件到内存，以便后续与服务器做交互，文件内行游戏服务器地址和资源服务器地址
	/// </summary>
	public class GlobalConfigComponent : Component
	{
		public static GlobalConfigComponent Instance;
		public GlobalProto GlobalProto;

		public void Awake()
		{
			Instance = this;
			string configStr = ConfigHelper.GetGlobal();
			this.GlobalProto = JsonHelper.FromJson<GlobalProto>(configStr);
		}
	}
}