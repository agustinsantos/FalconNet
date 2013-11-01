
namespace FalconNet.Common
{
    public interface IPlugin
    {
        string Name { get; }
        bool Initialize();
        void Shutdown();
    }
}
