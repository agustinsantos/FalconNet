using FalconNet.Common.Graphics;

namespace FalconNet.F4Common
{
    public interface IDrawableObject
    {
        void GetPosition(ref Tpoint pos);
        bool GetRayHit(Tpoint p, Tpoint p2, Tpoint p3, float f = 1);
        float GetScale();
        bool InDisplayList();
        float Radius();
        void SetInhibitFlag(bool p);
        void SetLabel(string label, uint p);
        void SetScale(float s);
        float X();
        float Y();
        float Z();
    }
}