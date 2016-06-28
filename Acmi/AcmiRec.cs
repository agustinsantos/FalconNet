using FalconNet.Campaign;
using FalconNet.Common.Encoding;
using FalconNet.F4Common;
using FalconNet.VU;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Acmi
{
    /// <summary>
    /// These are the enumerated record types
    /// </summary>
    public enum RecordTypes : byte
    {
        ACMIRecGenPosition = 0,
        ACMIRecMissilePosition,
        ACMIRecFeaturePosition,
        ACMIRecAircraftPosition,
        ACMIRecTracerStart,
        ACMIRecStationarySfx,
        ACMIRecMovingSfx,
        ACMIRecSwitch,
        ACMIRecDOF,
        ACMIRecChaffPosition,
        ACMIRecFlarePosition,
        ACMIRecTodOffset,
        ACMIRecFeatureStatus,
        ACMICallsignList,
        ACMIRecMaxTypes
    }

    /// <summary>
    /// this struct is common thru all record types as a record header
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIRecHeader
    {
        public RecordTypes type; // one of the ennumerated types
        public float time; // time stamp
    }

    /// <summary>
    /// General position data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIGenPositionData
    {
        public int type; // base type for creating simbase object
        public long uniqueID; // identifier of instance
        public float x;
        public float y;
        public float z;
        public float yaw;
        public float pitch;
        public float roll;
    }

    /// <summary>
    /// General position data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIFeaturePositionData
    {
        public int type; // base type for creating simbase object
        public long uniqueID; // identifier of instance
        public long leadUniqueID; // id of lead component (for bridges. bases etc)
        public int slot; // slot number in component list
        public int specialFlags;   // campaign feature flag
        public float x;
        public float y;
        public float z;
        public float yaw;
        public float pitch;
        public float roll;
    }

    /// <summary>
    /// ACMI Text event (strings parsed from event file)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMITextEvent
    {
        public const int Size = sizeof(int) + 120;

        public int intTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string timeStr;//[20];
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string msgStr;//[100];
    }

    /// <summary>
    /// General position data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMISwitchData
    {
        public int type; // base type for creating simbase object
        public long uniqueID; // identifier of instance
        public int switchNum;
        public int switchVal;
        public int prevSwitchVal;
    }

    /// <summary>
    /// Feature status change data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIFeatureStatusData
    {
        public long uniqueID; // identifier of instance
        public int newStatus;
        public int prevStatus;
    }

    /// <summary>
    /// General position data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIDOFData
    {
        public int type; // base type for creating simbase object
        public long uniqueID; // identifier of instance
        public int DOFNum;
        public float DOFVal;
        public float prevDOFVal;
    }

    /// <summary>
    /// Starting pos and velocity of tracer rounds
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMITracerStartData
    {
        // initial values
        public float x;
        public float y;
        public float z;
        public float dx;
        public float dy;
        public float dz;
    }

    /// <summary>
    /// Starting pos of a staionay special sfx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIStationarySfxData
    {
        public int type; // sfx type
        public float x; // position
        public float y;
        public float z;
        public float timeToLive;
        public float scale;
    }

    /// <summary>
    /// Starting pos of a staionay special sfx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIMovingSfxData
    {
        public int type; // sfx type
        public int user; // misc data
        public int flags;
        public float x; // position
        public float y;
        public float z;
        public float dx; // vector
        public float dy;
        public float dz;
        public float timeToLive;
        public float scale;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIMovingSfxRecord
    {
        public ACMIRecHeader hdr;
        public ACMIMovingSfxData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIStationarySfxRecord
    {
        public ACMIRecHeader hdr;
        public ACMIStationarySfxData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIGenPositionRecord
    {
        public ACMIRecHeader hdr;
        public ACMIGenPositionData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIMissilePositionRecord
    {
        public ACMIRecHeader hdr;
        public ACMIGenPositionData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMITodOffsetRecord
    {
        public ACMIRecHeader hdr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIChaffPositionRecord
    {
        public ACMIRecHeader hdr;
        public ACMIGenPositionData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIFlarePositionRecord
    {
        public ACMIRecHeader hdr;
        public ACMIGenPositionData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIAircraftPositionRecord
    {
        public ACMIRecHeader hdr;
        public ACMIGenPositionData data;
        public long RadarTarget;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIFeaturePositionRecord
    {
        public ACMIRecHeader hdr;
        public ACMIFeaturePositionData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIFeatureStatusRecord
    {
        public ACMIRecHeader hdr;
        public ACMIFeatureStatusData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMITracerStartRecord
    {
        public ACMIRecHeader hdr;
        public ACMITracerStartData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMISwitchRecord
    {
        public ACMIRecHeader hdr;
        public ACMISwitchData data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIDOFRecord
    {
        public ACMIRecHeader hdr;
        public ACMIDOFData data;
    }

    public class ACMIRecorder
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string AcmiPath = "acmibin";

        // the global recorder
        private static ACMIRecorder gACMIRec;

        // no more recording once an error is hit
        private static bool gACMIRecError = false;

        // visible display in otwdrive for pct tape full
        private static string gAcmiStr;//[11]; //Defined in otwloop.cpp, line 198

        private static ACMI_Hash ACMIIDTable = null;

        private static void InitACMIIDTable()
        {
            if (ACMIIDTable != null)
            {
                ACMIIDTable.Cleanup();
                //delete ACMIIDTable;
                ACMIIDTable = null;
            }

            ACMIIDTable = new ACMI_Hash();
            ACMIIDTable.Setup(100);
        }

        private static void CleanupACMIIDTable()
        {
            if (ACMIIDTable != null)
            {
                ACMIIDTable.Cleanup();
                //delete ACMIIDTable;
                ACMIIDTable = null;
            }
        }


        // Constructors.
        public ACMIRecorder()
        {
            _fd = null;
            _csect = F4CriticalSection.F4CreateCriticalSection("acmi");
            _recording = false;

            // edg: We Need to get this from player options!!!!
            // at the moment there doesn't seem to be a value for this in the class
            // default to 5 meg
            _maxbytesToWrite = (float)PlayerOptionsClass.PlayerOptions.AcmiFileSize() * 1024 * 1024;

            // OW BC
            string[] files;

            // Exception could occur due to insufficient permission.
            files = Directory.GetFiles(AcmiPath, "*.flt", SearchOption.TopDirectoryOnly);

            // If matching files have been found, return the first one.
            if (files.Length != 0)
            {
                // Iterate through each file and delete it.
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
        }

        // Destructor.
        ~ACMIRecorder()
        {
            if (_fd != null)
                _fd.Close();

            _fd = null;
            F4CriticalSection.F4DestroyCriticalSection(_csect);
            //_csect = null; // JB 010108
        }


        /// <summary>
        /// Start Recording
        /// </summary>
        public void StartRecording()
        {
            string fname = null;
            int y;
            Stream fp;

            InitACMIIDTable();

            // init the display string
            gAcmiStr = "----------";

            // if we're hit a write error, no more recording...
            if (gACMIRecError == true)
                return;

            // set our max file size now
            _maxbytesToWrite = 1000000.0f * PlayerOptionsClass.PlayerOptions.ACMIFileSize;

            // find a suitable name for flight file
            for (y = 0; y < 10000; y++)
            {
                fname = string.Format("acmibin\\acmi{%0:D4}.flt", y);

                if (!File.Exists(fname)) break;
            }

            _fd = new FileStream(fname, FileMode.CreateNew, FileAccess.Write);

            if (_fd != null)
            {
                // initialize the bytes written
                _bytesWritten = 0.0f;

                // this is where a call to simdriver needs to go to initialize
                // objects
                SimulationDriver.SimDriver.InitACMIRecord();

                _recording = true;

                log.DebugFormat("ACMI Recording, File Size = {0}", _maxbytesToWrite);
            }
        }

        /// <summary>
        /// Stop Recording
        /// </summary>
        public void StopRecording()
        {
            long i;
            int count;
            long idx = 0;
            ACMI_HASHNODE rec = null;
            ACMI_CallRec[] list;
            ACMIRecHeader hdr;


            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                _recording = false;
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            // Write out the Callsign/Color list

            count = (int)ACMIIDTable.GetLastID();

            if (count > 0)
            {
                list = new ACMI_CallRec[count];
                //memset(list, 0, sizeof(ACMI_CallRec) * count);

                hdr.type = RecordTypes.ACMICallsignList;
                hdr.time = 0.0f;
                StructEncoding<ACMIRecHeader>.Encode(_fd, hdr);

                Int64EncodingLE.Encode(_fd, count);

                i = ACMIIDTable.GetFirst(ref rec, ref idx);

                while (rec != null && i >= 0 && i < count)
                {
                    list[i].label = rec.label;
                    list[i].teamColor = rec.color;

                    i = ACMIIDTable.GetNext(ref rec, ref idx);
                }

                StructEncoding<ACMI_CallRec>.Encode(list, count);
            }

            _fd.Close();
            _fd = null;
            _recording = false;

            CleanupACMIIDTable();

            F4CriticalSection.F4LeaveCriticalSection(_csect);

            log.Debug("ACMI Stopped Recording\n");
        }

        /// <summary>
        /// Toggle Recording
        /// </summary>
        public void ToggleRecording()
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (IsRecording())
                StopRecording();
            else
                StartRecording();

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        public bool IsRecording()
        {
            return _recording;
        }

        /// <summary>
        /// Write a tracer start record
        /// </summary>
        /// <param name="recp"></param>
        public void TracerRecord(ACMITracerStartRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecTracerStart;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMITracerStartRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMITracerStartRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a General Position record
        /// </summary>
        /// <param name="recp"></param>
        public void GenPositionRecord(ACMIGenPositionRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecGenPosition;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            // FIX *(recp.data.label) = null;

            try
            {
                StructEncoding<ACMIGenPositionRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIGenPositionRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a General Position record
        /// </summary>
        /// <param name="recp"></param>
        public void AircraftPositionRecord(ACMIAircraftPositionRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecAircraftPosition;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMIAircraftPositionRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIAircraftPositionRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a General Position record
        /// </summary>
        /// <param name="recp"></param>
        public void MissilePositionRecord(ACMIMissilePositionRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecMissilePosition;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            // FIX *(recp.data.label) = null;
            try
            {
                StructEncoding<ACMIMissilePositionRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIMissilePositionRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a General Position record
        /// </summary>
        /// <param name="recp"></param>
        public void ChaffPositionRecord(ACMIChaffPositionRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecChaffPosition;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            // FIX *(recp.data.label) = null;
            // FIX recp.data.teamColor = 0x0;

            try
            {
                StructEncoding<ACMIChaffPositionRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIChaffPositionRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a General Position record
        /// </summary>
        /// <param name="recp"></param>
        public void FlarePositionRecord(ACMIFlarePositionRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecFlarePosition;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            // FIX *(recp.data.label) = null;
            // FIX recp.data.teamColor = 0x0;

            try
            {
                StructEncoding<ACMIFlarePositionRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIFlarePositionRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a General Position record
        /// </summary>
        /// <param name="recp"></param>
        public void FeaturePositionRecord(ACMIFeaturePositionRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecFeaturePosition;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMIFeaturePositionRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIFeaturePositionRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a Stationary Sfx record
        /// </summary>
        /// <param name="recp"></param>
        public void StationarySfxRecord(ACMIStationarySfxRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecStationarySfx;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMIStationarySfxRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIStationarySfxRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a Moving Sfx record
        /// </summary>
        /// <param name="recp"></param>
        public void MovingSfxRecord(ACMIMovingSfxRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecMovingSfx;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMIMovingSfxRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIMovingSfxRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a switch data record
        /// </summary>
        /// <param name="recp"></param>
        public void SwitchRecord(ACMISwitchRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecSwitch;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMISwitchRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMISwitchRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a DOF data record
        /// </summary>
        /// <param name="recp"></param>
        public void DOFRecord(ACMIDOFRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecDOF;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMIDOFRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIDOFRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a General Position record
        /// </summary>
        /// <param name="recp"></param>
        public void TodOffsetRecord(ACMITodOffsetRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecTodOffset;

            try
            {
                StructEncoding<ACMITodOffsetRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMITodOffsetRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Write a Feature Status record
        /// </summary>
        /// <param name="recp"></param>
        public void FeatureStatusRecord(ACMIFeatureStatusRecord recp)
        {
            F4CriticalSection.F4EnterCriticalSection(_csect);

            if (_fd == null)
            {
                F4CriticalSection.F4LeaveCriticalSection(_csect);
                return;
            }

            recp.hdr.type = RecordTypes.ACMIRecFeatureStatus;
            // recp.hdr.time = (float)(vuxGameTime/1000) + OTWDriver.todOffset;

            try
            {
                StructEncoding<ACMIFeatureStatusRecord>.Encode(_fd, recp);
            }
            catch
            {
                StopRecording();
                gACMIRecError = true;
            }

            _bytesWritten += StructEncoding<ACMIFeatureStatusRecord>.Size;

            // check our file size and automaticly start a new recording
            if (_bytesWritten >= _maxbytesToWrite)
            {
                StopRecording();
                // this tells simdrive to toggle recording at the appropriate time
                SimulationDriver.SimDriver.doFile = true;
                log.Debug("ACMI Recording starting new tape\n");
            }

            F4CriticalSection.F4LeaveCriticalSection(_csect);
        }

        /// <summary>
        /// Returns Percent Tape Full, a number in the 0 - 10 range
        /// </summary>
        /// <returns></returns>
        public int PercentTapeFull()
        {
            return (int)(10.0f * (_bytesWritten / _maxbytesToWrite));
        }


        private Stream _fd;

        // we need synchronization for writes
        private F4CSECTIONHANDLE _csect;

        private bool _recording;

        private float _bytesWritten;
        private float _maxbytesToWrite;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ACMI_CallRec
    {
        public const int Size = sizeof(int) + 16;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string label; //[16];
        public int teamColor;
    }

}
