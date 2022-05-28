using Unity.Collections;
using Unity.Netcode;

public struct NetworkString : INetworkSerializeByMemcpy
{
	private ForceNetworkSerializeByMemcpy<FixedString64Bytes> data;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref data);
	}

	public override string ToString()
	{
		return data.ToString();
	}

	public static implicit operator string(NetworkString s) => s.ToString();
	public static implicit operator NetworkString(string s) => new NetworkString() { data = new FixedString64Bytes(s) };
}
