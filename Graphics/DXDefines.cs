using FalconNet.Common.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DWORD = System.UInt32;

namespace FalconNet.Graphics
{
    public enum NodeType { ROOT = 0, VERTEX, DOT, LINE, TRIANGLE, POLY, DOF, CLOSEDOF, SLOT, MODELEND }
    public enum DofType { NO_DOF = 0, ROTATE, XROTATE, TRANSLATE, SCALE, SWITCH, XSWITCH }
    public enum StencilModeType { STENCIL_OFF = 0, STENCIL_ON, STENCIL_WRITE, STENCIL_CHECK }


    // The scripts constants
    // WARNING : Change of this must be done carefully, synced with scripts variables...
    public enum ScriptType
    {
        SCRIPT_NONE = 0,
        SCRIPT_ANIMATE,
        SCRIPT_ROTATE,
        SCRIPT_HELY,
        SCRIPT_BEACON,
        SCRIPT_VASIF,
        SCRIPT_VASIN,
        SCRIPT_MEATBALL,
        SCRIPT_CHAFF,
        SCRIPT_CHUTEDIE,
    } ;

    // * MODEL SCRIPTS MANAGEMENT *
    public class DXScriptVariableType
    {
        public ScriptType Script;
        public DWORD[] Arguments = new DWORD[3];
    }

    // * DX Database Textures section *
    public class DxDbHeader
    {
        public const int MAX_SCRIPTS_X_MODEL = 2; // number of scripts available for a model
        public DWORD Version;
        public DWORD Id;
        public DWORD VBClass;
        public DWORD ModelSize;
        public DWORD dwNVertices;
        public DWORD dwPoolSize;
        public DWORD pVPool;
        public DWORD dwNodesNr;
        public DXScriptVariableType[] Scripts = new DXScriptVariableType[MAX_SCRIPTS_X_MODEL];
        public DWORD dwLightsNr;
        public DWORD pLightsPool;
        public DWORD dwTexNr;
    }

    #region Encoding
    public static class DXScriptVariableTypeEncodingLE
    {
        public static void Encode(Stream stream, DXScriptVariableType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, DXScriptVariableType rst)
        {
            rst.Script = (ScriptType)Int32EncodingLE.Decode(stream);
            for (int i = 0; i < 3; i++)
                rst.Arguments[i] = UInt32EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class DxDbHeaderEncodingLE
    {
        public static void Encode(Stream stream, DxDbHeader val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, DxDbHeader rst)
        {
            rst.Version = UInt32EncodingLE.Decode(stream);
            rst.Id = UInt32EncodingLE.Decode(stream);
            rst.VBClass = UInt32EncodingLE.Decode(stream);
            rst.ModelSize = UInt32EncodingLE.Decode(stream);
            rst.dwNVertices = UInt32EncodingLE.Decode(stream);
            rst.dwPoolSize = UInt32EncodingLE.Decode(stream);
            rst.pVPool = UInt32EncodingLE.Decode(stream);
            rst.dwNodesNr = UInt32EncodingLE.Decode(stream);
            for (int i = 0; i < DxDbHeader.MAX_SCRIPTS_X_MODEL; i++)
            {
                rst.Scripts[i] = new DXScriptVariableType();
                DXScriptVariableTypeEncodingLE.Decode(stream, rst.Scripts[i]);
            }
            rst.dwLightsNr = UInt32EncodingLE.Decode(stream);
            rst.pLightsPool = UInt32EncodingLE.Decode(stream);
            rst.dwTexNr = UInt32EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    #endregion
}
