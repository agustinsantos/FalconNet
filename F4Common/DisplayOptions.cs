using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public enum TEXMODE
    {
        TEX_MODE_16 = 70159,
        TEX_MODE_32,
        TEX_MODE_DDS,
    }

    public class DisplayOptionsClass
    {

        public ushort DispWidth;
        public ushort DispHeight;
        public byte DispVideoCard;
        public byte DispVideoDriver;
        public int DispDepth;
        public bool bRender2Texture;
        public bool bAnisotropicFiltering;
        public bool bLinearMipFiltering;
        public bool bMipmapping;
        public bool bZBuffering;
        public bool bRender2DCockpit;
        public bool bFontTexelAlignment;
        public bool bSpecularLighting;
        public bool bScreenCoordinateBiasFix; //Wombat778 4-01-04


        public TEXMODE m_texMode;

        public DisplayOptionsClass()
        {
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public int LoadOptions(string filename = "display")
        {
            throw new NotImplementedException();
        }

        public int SaveOptions()
        {
            throw new NotImplementedException();
        }

        public static void SetDevCaps(uint devCaps)
        {
            throw new NotImplementedException();
        }

        public static uint GetDevCaps()
        {
            throw new NotImplementedException();
        }


        // sfr: used for enumerating only some drivers, not a player option but a command line switch
        // so static, it wont be saved
        private static uint iDeviceCaps;

        public static DisplayOptionsClass DisplayOptions = new DisplayOptionsClass();
    }
}
