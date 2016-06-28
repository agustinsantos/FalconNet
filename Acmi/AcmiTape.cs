using FalconNet.Campaign;
using FalconNet.Common;
using FalconNet.Common.Encoding;
using FalconNet.Common.Graphics;
using FalconNet.Common.Maths;
using FalconNet.FalcLib;
using FalconNet.Graphics;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Acmi
{

    // this structure will hold the in-memory, Sim representation for the
    // entity.  IOW base class, drawable objs, etc...
    public class SimTapeEntity
    {
        public SimBaseClass objBase;
        public DrawableTrail objTrail;

        public ACMITape.EntityFlag flags;

        // object orientation
        public float x;
        public float y;
        public float z;
        public float yaw;
        public float pitch;
        public float roll;

        // average speed between 2 positions
        public float aveSpeed;
        public float aveTurnRate;
        public float aveTurnRadius;

        // for trails, the start and end times
        public float trailStartTime;
        public float trailEndTime;

        // missiles need engine glow drawables
        public DrawableBSP objBsp1;
        public DrawableBSP objBsp2;

        // for flare need a glowing sphere
        public Drawable2D obj2d;

        // for wing tip trails
        public int wtLength;
        public DrawableTrail wlTrail;
        public DrawableTrail wrTrail;

        // for features we may need an index to the lead component and
        // the slot # that was in the camp component list (for bridges, bases...)
        public long leadIndex;
        public int slot;

    }


    /*
    ** This struct holds info necessary for handling active tracer events
    */
    public class TracerEventData
    {
        public float x;
        public float y;
        public float z;
        public float dx;
        public float dy;
        public float dz;
        public DrawableTracer objTracer;
    }


    /*
    ** Each active event will have one of these in a chain
    */
    public class ActiveEvent
    {
        public RecordTypes eventType;
        public long index;
        public float time;
        public float timeEnd;
        public object eventData;
        public ActiveEvent next;
        public ActiveEvent prev;
    }


    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    //
    // These are the headers and data that are used internally for the .vhs format.
    // These use offsets instead of pointers so that we can memory map them.
    // All offsets are from the start of the file!!!.

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Header for the tape file.
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMITapeHeader
    {
        public const int FILEID = 0x54415045; // TAPE
        public const int Size = sizeof(int) * 17 + sizeof(float) * 3;

        public int fileID;
        public int fileSize;
        public int numEntities;
        public int numFeat;
        public int entityBlockOffset;
        public int featBlockOffset;
        public int numEntityPositions;
        public int timelineBlockOffset;
        public int firstEntEventOffset;
        public int firstGeneralEventOffset;
        public int firstEventTrailerOffset;
        public int firstTextEventOffset;
        public int firstFeatEventOffset;
        public int numEvents;
        public int numEntEvents;
        public int numTextEvents;
        public int numFeatEvents;
        public float startTime;
        public float totPlayTime;
        public float todOffset;
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Entity data.
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIEntityData
    {
        public const int Size = sizeof(int) * 7 + sizeof(int) * 2;

        public int uniqueID;
        public int type;
        public int count;
        public ACMITape.EntityFlag flags;


        // for features we may need an index to the lead component and
        // the slot # that was in the camp component list (for bridges, bases...)
        public int leadIndex;
        public int slot;
        public int specialFlags;


        // Offset from the start of the file to the start of my positional data.
        public int firstPositionDataOffset;
        public int firstEventDataOffset;
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Entity position data.

    // enum types for position
    public enum PosType : byte
    {
        PosTypePos = 0,
        PosTypeSwitch,
        PosTypeDOF,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct posTag
    {
        public float x;
        public float y;
        public float z;
        public float pitch;
        public float roll;
        public float yaw;
        public long radarTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct dofTag
    {
        public int DOFNum;
        public float DOFVal;
        public float prevDOFVal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct switchTag
    {
        public int switchNum;
        public uint switchVal;
        public uint prevSwitchVal;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ACMIEntityPositionData
    {
        // Time stamp for the positional data
        [FieldOffset(0)]
        public float time;
        [FieldOffset(sizeof(float))]
        public PosType type;

        // dereference based on type
        // union
        //{
        // Positional data.        
        [FieldOffset(sizeof(float) + sizeof(byte))]
        public posTag posData;

        // switch change
        [FieldOffset(sizeof(float) + sizeof(byte))]
        public switchTag switchData;

        // DOF change
        [FieldOffset(sizeof(float) + sizeof(byte))]
        public dofTag dofData;
        // end union };

        // Although position data is a fixed size, we still want
        // this so that we can organize the data to be friendly for
        // paging.
        [FieldOffset(sizeof(byte) + sizeof(float) * 7 + sizeof(int))]
        public int nextPositionUpdateOffset;

        [FieldOffset(sizeof(byte) + sizeof(float) * 7 + sizeof(int) * 2)]
        public int prevPositionUpdateOffset;
    }

    //
    // This raw format is used by the position/event/sfx bundler to
    // create a .vhs file (dig that extension), which is the ACMITape playback format.
    // This is the format stored in the flight file.

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIRawPositionData
    {
        public int type; // type of object
        public int uniqueID; // A unique ID for the object. Many to One correlation to Falcon Entities
        public int flags; // side

        // for features we may need an index to the lead component and
        // the slot # that was in the camp component list (for bridges, bases...)
        public int leadIndex;
        public int slot;
        public int specialFlags;
        public ACMIEntityPositionData entityPosData;
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Header for event data.

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIEventHeader
    {
        public const int Size = 1 + sizeof(int) * 4 + sizeof(int) * 6;

        // type of event this is
        public RecordTypes eventType;
        public int index;

        // Time stamp for this event.
        public float time;
        public float timeEnd;

        // data specific to type of event
        public int type;
        public int user;
        public int flags;
        public float scale;
        public float x, y, z;
        public float dx, dy, dz;
        public float roll, pitch, yaw;
    }

    //
    // Trailer for event data.
    //

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIEventTrailer
    {
        public const int Size = sizeof(float) + sizeof(int);

        public float timeEnd;
        public int index; // into EventHeader
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Feature Status Event

    [StructLayout(LayoutKind.Sequential)]
    public struct ACMIFeatEvent
    {
        public const int Size = sizeof(float) + sizeof(int) * 3;

        // Time stamp for this event.
        public float time;

        // index of feature on tape
        public int index;

        // data specific to type of event
        public int newStatus;
        public int prevStatus;

    }

    public class ACMIFeatEventImportData
    {
        public long uniqueID;
        public ACMIFeatEvent data;
    }

    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    //
    // .vhs file format:
    //
    // |                        |                 |                 |
    // |        header          | entity block    | timeline block  |
    // | sizeof(ACMITapeHeader) | (variable size) | (variable size) |
    //
    ////////////////////////////////////////////////////////////////////////////////
    //
    // entity block:
    //
    // |                    |                                       |
    // | number of entities |              entities                 |
    // |  sizeof(long)      | num entities * sizeof(ACMIEntityData) |
    //
    // entity:
    //
    // |                        |
    // |      ACMIEntityData    |
    // | sizeof(ACMIEntityData) |
    //
    ////////////////////////////////////////////////////////////////////////////////
    //
    // timeline block:
    //
    // |                              |                    |                     |
    // | entity position update block | entity event block | general event block |
    // |     (variable size)          |  (variable size)   |    (variable size)  |
    //
    // The entity position update block contains all entity position updates.
    // The position updates are threaded on a per-entity basis, with a separate doubly linked list
    // for each entity.
    // The position updates should be chronologically sorted.
    // There should be a position update read-head for each entity for traversing its linked list
    // of position updates.
    //
    // The entity event block contains all events which are relevant to entities.
    // The events are threaded on a per-entity basis, with a separate doubly linked list
    // for each entity.
    // The events should be chronologically sorted.
    // There should be an event read-head for each entity for traversing its linked list of events.
    //
    // The general event block contains all events which are not relevant to a specific entity.
    // The events are threaded in doubly linked list.
    // The events should be chronologically sorted.
    // There should be an event read-head for traversing the linked list of events.
    //
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////

    public static class ACMIStatics
    {
        public static void DestroyACMIRawPositionDataList(List list)
        {
            // LIST_DESTROY (list, (PFV)DeleteACMIRawPositionData);
            DestroyTheList(list);
        }

        public static void DeleteACMIRawPositionData(ACMIRawPositionData rawPositionData)
        {
            //delete rawPositionData;
        }

        public static void DeleteACMIEntityPositionData(ACMIEntityPositionData data)
        {
            //delete data;
        }
        public static void DeleteACMIEntityData(ACMIEntityData data)
        {
            //delete data;
        }

        public static void DeleteACMIEventHeader(ACMIEventHeader data)
        {
            //delete data;
        }
        public static void DeleteACMIFeatEventImportData(ACMIFeatEventImportData data)
        {
            //delete data;
        }

        public static int CompareEventTrailer(object p1, object p2)
        {
            ACMIEventTrailer t1 = (ACMIEventTrailer)p1;
            ACMIEventTrailer t2 = (ACMIEventTrailer)p2;

            if (t1.timeEnd < t2.timeEnd)
                return -1;
            else if (t1.timeEnd > t2.timeEnd)
                return 1;
            else
                return 0;
        }

        static void DestroyTheList(List list)
        {
#if TODO
            LIST* prev,
         *curr;

            if (!list)
                return;

            prev = list;
            curr = list.next;

            while (curr)
            {
                // if ( destructor )
                //    (*destructor)(prev . node);

                delete prev.node;

                prev.next = null;

                delete prev;

                prev = curr;
                curr = curr.next;
            }

            // if( destructor )
            //    (*destructor)( prev . node );

            delete prev.node;

            prev.next = null;

            delete prev;
#endif
            throw new NotImplementedException();
            //ListGlobalPack();
        }
    }


    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    //
    // Callback for events that are not entity-specific.
    // The EventIdData parameter is the event id.
    // The first void * parameter points to the event data, which
    // can be decoded with the event id.
    // The second void * parameter is for user data.

    public delegate void ACMI_GENERAL_EVENT_CALLBACK(ACMITape tape, EventIdData eid, object p1, object p2);

    public struct ACMIGeneralEventCallback
    {
        public ACMI_GENERAL_EVENT_CALLBACK forwardCallback;
        public ACMI_GENERAL_EVENT_CALLBACK reverseCallback;
        public object userData;
    }

    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////

    public struct ACMIEntityReadHead
    {
        public int positionDataOffset;
        public int eventDataOffset;
    }

    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////

    public class ACMITape
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Flags]
        public enum EntityFlag : int
        {
            ENTITY_FLAG_MISSILE = 0x00000001,
            ENTITY_FLAG_FEATURE = 0x00000002,
            ENTITY_FLAG_AIRCRAFT = 0x00000004,
            ENTITY_FLAG_CHAFF = 0x00000008,
            ENTITY_FLAG_FLARE = 0x00000010,
        }

        private static void DefaultForwardACMIGeneralEventCallback(ACMITape tape, EventIdData eventId, object p1, object p2)
        {
            log.DebugFormat("General event occured in forward ACMI Tape play --> event type: {0}.", eventId.type);
        }

        private static void DefaultReverseACMIGeneralEventCallback(ACMITape tape, EventIdData eventId, object p1, object p2)
        {
            log.DebugFormat("General event occured in reverse ACMI Tape play --> event type: {0}.", eventId.type);
        }

        private static void CleanupACMIImportPositionData(Stream flightFile, ACMIRawPositionData rawPositionData)
        {
#if TODO
            if (flightFile != null)
            {
                flightFile.Close();
            }

            if (rawPositionData != null)
            {
                //delete rawPositionData;
            }

            if (importEntityList != null)
            {
                DestroyTheList(importEntityList);
                importEntityList = null;
            }

            if (importFeatList != null)
            {
                DestroyTheList(importFeatList);
                importFeatList = null;
            }

            if (importPosList != null)
            {
                DestroyTheList(importPosList);
                importPosList = null;
            }

            if (importEntEventList != null)
            {
                DestroyTheList(importEntEventList);
                importEntEventList = null;
            }

            if (importEventList != null)
            {
                DestroyTheList(importEventList);
                importEventList = null;
            }

            if (importEventTrailerList != null)
            {
                //delete[] importEventTrailerList;
                importEventTrailerList = null;
            }

            if (importFeatEventList != null)
            {
                DestroyTheList(importFeatEventList);
                importFeatEventList = null;
            }

            if (Import_Callsigns != null)
            {
                //delete Import_Callsigns;
                Import_Callsigns = null;
                import_count = 0;
            }
#endif
            throw new NotImplementedException();
        }

        // Constructors.
        // Do not put the extension with name.
        // This should be the name of the desired .vcr file.
        public ACMITape(string name, RenderOTW renderer, RViewPoint viewPoint, string dir = null)
        {
            int numEntities;
            string fullName;
            ACMIEntityData e;
            long length = 0;


            // initialize storage for drawable poled objects
#if USE_SH_POOLS
            DrawablePoled.InitializeStorage();
#endif

            Debug.Assert(name != null);

            _tape = null;
            _entityReadHeads = null;
            _simTapeEntities = null;
            _simTapeFeatures = null;
            _activeEventHead = null;
            _eventList = null;
            _screenCapturing = false;
            _wingTrails = false;
            _tapeObjScale = 1.0f;

            // set our render and viewpoint
            _renderer = renderer;
            _viewPoint = viewPoint;

            Init();

            // Open up a map file with the given name.

            // edg note on hack: right now, ALWAYS do an import from the acmi.flt
            // file to convert to a tape file.  Later we'll probably want to import
            // right after an ACMIU record session to get into .vhs format
            //strcpy( fullName, "campaign\\save\\fltfiles\\" );
            if (string.IsNullOrWhiteSpace(dir))
                fullName = Path.Combine("acmibin ", name);
            else
                fullName = Path.Combine(dir, name);

            // commented out if statement for quick testing....
            // if ( Import( fullName ) )
            {
                // create the memory mapping
                length = OpenTapeFile(fullName);

                // just test
                if (IsLoaded())
                {
                    numEntities = NumEntities();

                    for (int i = 0; i < numEntities; i++)
                    {
                        e = EntityData(i);
                        log.DebugFormat("Entity {0}: Type = {1}, Id = {2}, Offset = {3}",
                                  i,
                                  e.type,
                                  e.uniqueID,
                                  e.firstPositionDataOffset);


                    }

                    // CloseTapeFile();
                }
                else
                {
                    log.Debug("Unable to test memory mapped tape file");
                }
            }


            // If it loaded, do any additional setup.
            if (IsLoaded())
            {
                int numcalls = 0;

                // Setup Callsigns...
                int callsigns = GetCallsignList(out numcalls);

                if (callsigns < length && numcalls > 0) // there are callsigns...
                {
                    ACMI_Callsigns = new ACMI_CallRec[numcalls];
                    var contentArray = new byte[ACMI_CallRec.Size];
                    for (int i = 0; i < numcalls; i++)
                    {
                        int pos = callsigns + i * ACMI_CallRec.Size;
                        _tape.ReadArray(pos, contentArray, 0, contentArray.Length);
                        ACMI_CallRec cr = StructEncoding<ACMI_CallRec>.Decode(contentArray);
                        // _tape.Read<ACMI_CallRec>(pos, out cr);
                        ACMI_Callsigns[i] = cr;
                    }
                }

                numEntities = NumEntities();

                // Setup entity event callbacks. and read heads
                _entityReadHeads = new ACMIEntityReadHead[numEntities];

                for (int i = 0; i < numEntities; i++)
                {
                    // set the read heads to the first position
                    e = EntityData(i);
                    _entityReadHeads[i].positionDataOffset = e.firstPositionDataOffset;
                    _entityReadHeads[i].eventDataOffset = e.firstEventDataOffset;
                }

                // Setup general event callbacks.
                SetGeneralEventCallbacks
                (
                    DefaultForwardACMIGeneralEventCallback,
                    DefaultReverseACMIGeneralEventCallback,
                    null
                );

                // setup the sim tape entities
                SetupSimTapeEntities();

                // create an array of ActiveEvent pointers -- 1 for every event
                _eventList = new ActiveEvent[_tapeHdr.numEvents];
                // make sure they're null
                //memset(_eventList, 0, sizeof(ActiveEvent) * _tapeHdr.numEvents);

                // set the first and last event trailer pointers
                if (_tapeHdr.numEvents != 0)
                {
                    _tape.Read<ACMIEventTrailer>(_tapeHdr.firstEventTrailerOffset, out _firstEventTrailer);
                    _tape.Read<ACMIEventTrailer>(_tapeHdr.firstEventTrailerOffset + (_tapeHdr.numEvents - 1) * ACMIEventTrailer.Size, out _lastEventTrailer);
                }

                _generalEventReadHeadTrailer = _firstEventTrailer;

                if (_tapeHdr.numFeatEvents != 0)
                {
                    _tape.Read<ACMIFeatEvent>(_tapeHdr.firstFeatEventOffset, out _firstFeatEvent);
                    _tape.Read<ACMIFeatEvent>(_tapeHdr.firstFeatEventOffset + (_tapeHdr.numFeatEvents - 1) * ACMIFeatEvent.Size, out _lastFeatEvent);
                }

                _featEventReadHead = _firstFeatEvent;
            }
        }

        // Destructor.
        ~ACMITape()
        {
#if TODO
            // Delete Callsigns
            if (ACMI_Callsigns != null)
            {
                // delete ACMI_Callsigns;
                ACMI_Callsigns = null;
            }

            Init();

#if USE_SH_POOLS
            DrawablePoled.ReleaseStorage();
#endif
#endif
        }


        // Import the current positional, event, and sfx data.
        // The filenames of these files will always be the same
        // so we don't have to pass them in.
        public static bool Import(string inFltFile, string outTapeFileName)
        {
#if TODO
            Stream flightFile;

            ACMIRawPositionData rawPositionData = null;

            ACMIEventHeader ehdr = new ACMIEventHeader();

            ACMIFeatEventImportData fedata = null;

            float
            begTime,
            endTime;

            ACMITapeHeader tapeHdr = new ACMITapeHeader();
            ACMIRecHeader hdr;
            ACMIGenPositionData genpos;
            ACMIFeaturePositionData featpos;
            ACMITracerStartData tracer;
            ACMIStationarySfxData sfx;
            ACMIMovingSfxData msfx;
            ACMISwitchData sd;
            ACMIDOFData dd;
            ACMIFeatureStatusData fs;


            // zero our counters
            importNumFeat = 0;
            importNumPos = 0;
            importNumEnt = 0;
            importNumEvents = 0;
            importNumFeatEvents = 0;
            importNumEntEvents = 0;

            // zero out position list
            importFeatList = null;
            importFeatEventList = null;
            importPosList = null;
            importEventList = null;
            importEntEventList = null;
            importEventTrailerList = null;

            // this value comes from tod type record
            tapeHdr.todOffset = 0.0f;


            // Load flight file for positional data.
            //flightFile = fopen("campaign\\save\\fltfiles\\acmi.flt", "rb");
            flightFile = new FileStream(inFltFile, FileMode.Open, FileAccess.Read);

            if (flightFile == null)
            {
                log.Debug("Error opening acmi flight file");
                return false;
            }

            begTime = -1.0f;
            endTime = 0.0f;
            log.Debug("ACMITape Import: Reading Raw Data ....\n");

            while (StructEncoding<ACMIRecHeader>.Decode(flightFile, out hdr))
            {
                // now read in the rest of the record depending on type
                switch (hdr.type)
                {
                    case RecordTypes.ACMIRecTodOffset:
                        tapeHdr.todOffset = hdr.time;
                        break;

                    case RecordTypes.ACMIRecGenPosition:
                    case RecordTypes.ACMIRecMissilePosition:
                    case RecordTypes.ACMIRecChaffPosition:
                    case RecordTypes.ACMIRecFlarePosition:
                    case RecordTypes.ACMIRecAircraftPosition:

                        // Read the data
                        if (!StructEncoding<ACMIGenPositionData>.Decode(flightFile, out genpos))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        if (hdr.type == RecordTypes.ACMIRecAircraftPosition)
                            fread(&tempTarget, sizeof(tempTarget), 1, flightFile);
                        else
                            tempTarget = -1;

                        // Allocate a new data node.
                        Debug.Assert(rawPositionData == null);
                        rawPositionData = new ACMIRawPositionData();
                        Debug.Assert(rawPositionData != null);

                        // fill in raw position data
                        rawPositionData.uniqueID = genpos.uniqueID;
                        rawPositionData.type = genpos.type;

                        if (hdr.type == RecordTypes.ACMIRecMissilePosition)
                            rawPositionData.flags = (int)EntityFlag.ENTITY_FLAG_MISSILE;
                        else if (hdr.type == RecordTypes.ACMIRecAircraftPosition)
                            rawPositionData.flags = (int)EntityFlag.ENTITY_FLAG_AIRCRAFT;
                        else if (hdr.type == RecordTypes.ACMIRecChaffPosition)
                            rawPositionData.flags = (int)EntityFlag.ENTITY_FLAG_CHAFF;
                        else if (hdr.type == RecordTypes.ACMIRecFlarePosition)
                            rawPositionData.flags = (int)EntityFlag.ENTITY_FLAG_FLARE;
                        else
                            rawPositionData.flags = 0;

                        rawPositionData.entityPosData.time = hdr.time;
                        rawPositionData.entityPosData.type = PosType.PosTypePos;
                        // remove rawPositionData.entityPosData.teamColor = genpos.teamColor;
                        // remove strcpy((char*)rawPositionData.entityPosData.label, (char*)genpos.label);
                        rawPositionData.entityPosData.posData.x = genpos.x;
                        rawPositionData.entityPosData.posData.y = genpos.y;
                        rawPositionData.entityPosData.posData.z = genpos.z;
                        rawPositionData.entityPosData.posData.roll = genpos.roll;
                        rawPositionData.entityPosData.posData.pitch = genpos.pitch;
                        rawPositionData.entityPosData.posData.yaw = genpos.yaw;
                        rawPositionData.entityPosData.posData.radarTarget = tempTarget;


                        // Append our new position data.
                        importPosList = AppendToEndOfList(importPosList, &importPosListEnd, rawPositionData);
                        rawPositionData = null;

                        // bump counter
                        importNumPos++;

                        break;

                    case RecordTypes.ACMIRecTracerStart:

                        // Read the data
                        if (!StructEncoding<ACMITracerStartData>.Decode(flightFile, out tracer))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        // Allocate a new data node.
                        Debug.Assert(ehdr == null);
                        ehdr = new ACMIEventHeader();
                        Debug.Assert(ehdr != null);

                        // fill in data
                        ehdr.eventType = hdr.type;
                        ehdr.time = hdr.time;
                        ehdr.timeEnd = hdr.time + 2.5F;
                        ehdr.index = importNumEvents;
                        ehdr.x = tracer.x;
                        ehdr.y = tracer.y;
                        ehdr.z = tracer.z;
                        ehdr.dx = tracer.dx;
                        ehdr.dy = tracer.dy;
                        ehdr.dz = tracer.dz;


                        // Append our new data.
                        importEventList = AppendToEndOfList(importEventList, &importEventListEnd, ehdr);
                        ehdr = null;

                        // bump counter
                        importNumEvents++;
                        break;

                    case RecordTypes.ACMIRecStationarySfx:

                        // Read the data
                        if (!StructEncoding<ACMIStationarySfxData>.Decode(flightFile, out sfx))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        // Allocate a new data node.
                        Debug.Assert(ehdr == null);
                        ehdr = new ACMIEventHeader();
                        Debug.Assert(ehdr != null);

                        // fill in data
                        ehdr.eventType = hdr.type;
                        ehdr.index = importNumEvents;
                        ehdr.time = hdr.time;
                        ehdr.timeEnd = hdr.time + sfx.timeToLive;
                        ehdr.x = sfx.x;
                        ehdr.y = sfx.y;
                        ehdr.z = sfx.z;
                        ehdr.type = sfx.type;
                        ehdr.scale = sfx.scale;


                        // Append our new data.
                        importEventList = AppendToEndOfList(importEventList, &importEventListEnd, ehdr);
                        ehdr = null;

                        // bump counter
                        importNumEvents++;
                        break;

                    case RecordTypes.ACMIRecFeatureStatus:

                        // Read the data
                        if (!StructEncoding<ACMIFeatureStatusData>.Decode(flightFile, out fs))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        // Allocate a new data node.
                        Debug.Assert(fedata == null);
                        fedata = new ACMIFeatEventImportData();
                        Debug.Assert(fedata != null);

                        // fill in data
                        fedata.uniqueID = fs.uniqueID;
                        fedata.data.index = -1; // will be filled in later
                        fedata.data.time = hdr.time;
                        fedata.data.newStatus = fs.newStatus;
                        fedata.data.prevStatus = fs.prevStatus;


                        // Append our new data.
                        importFeatEventList = AppendToEndOfList(importFeatEventList, &importFeatEventListEnd, fedata);
                        fedata = null;

                        // bump counter
                        importNumFeatEvents++;
                        break;

                    // not ready for these yet
                    case RecordTypes.ACMIRecMovingSfx:

                        // Read the data
                        if (!StructEncoding<ACMIMovingSfxData>.Decode(flightFile, out msfx))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        // Allocate a new data node.
                        Debug.Assert(ehdr == null);
                        ehdr = new ACMIEventHeader();
                        Debug.Assert(ehdr != null);

                        // fill in data
                        ehdr.eventType = hdr.type;
                        ehdr.index = importNumEvents;
                        ehdr.time = hdr.time;
                        ehdr.timeEnd = hdr.time + msfx.timeToLive;
                        ehdr.x = msfx.x;
                        ehdr.y = msfx.y;
                        ehdr.z = msfx.z;
                        ehdr.dx = msfx.dx;
                        ehdr.dy = msfx.dy;
                        ehdr.dz = msfx.dz;
                        ehdr.flags = msfx.flags;
                        ehdr.user = msfx.user;
                        ehdr.type = msfx.type;
                        ehdr.scale = msfx.scale;


                        // Append our new data.
                        importEventList = AppendToEndOfList(importEventList, &importEventListEnd, ehdr);
                        ehdr = null;

                        // bump counter
                        importNumEvents++;
                        break;

                    case RecordTypes.ACMIRecSwitch:

                        // Read the data
                        if (!StructEncoding<ACMISwitchData>.Decode(flightFile, out sd))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        // Allocate a new data node.
                        Debug.Assert(rawPositionData == null);
                        rawPositionData = new ACMIRawPositionData();
                        Debug.Assert(rawPositionData != null);

                        // fill in raw position data
                        rawPositionData.uniqueID = sd.uniqueID;
                        rawPositionData.type = sd.type;
                        rawPositionData.flags = 0;


                        rawPositionData.entityPosData.time = hdr.time;
                        rawPositionData.entityPosData.type = PosType.PosTypeSwitch;
                        rawPositionData.entityPosData.switchData.switchNum = sd.switchNum;
                        rawPositionData.entityPosData.switchData.switchVal = sd.switchVal;
                        rawPositionData.entityPosData.switchData.prevSwitchVal = sd.prevSwitchVal;

                        // Append our new position data.
                        importEntEventList = AppendToEndOfList(importEntEventList, &importEntEventListEnd, rawPositionData);
                        rawPositionData = null;

                        // bump counter
                        importNumEntEvents++;

                        break;

                    case RecordTypes.ACMIRecDOF:

                        // Read the data
                        if (!StructEncoding<ACMIDOFData>.Decode(flightFile, out dd))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        // Allocate a new data node.
                        Debug.Assert(rawPositionData == null);
                        rawPositionData = new ACMIRawPositionData;
                        Debug.Assert(rawPositionData != null);

                        // fill in raw position data
                        rawPositionData.uniqueID = dd.uniqueID;
                        rawPositionData.type = dd.type;
                        rawPositionData.flags = 0;


                        rawPositionData.entityPosData.time = hdr.time;
                        rawPositionData.entityPosData.type = PosType.PosTypeDOF;
                        rawPositionData.entityPosData.dofData.DOFNum = dd.DOFNum;
                        rawPositionData.entityPosData.dofData.DOFVal = dd.DOFVal;
                        rawPositionData.entityPosData.dofData.prevDOFVal = dd.prevDOFVal;

                        // Append our new position data.
                        importEntEventList = AppendToEndOfList(importEntEventList, &importEntEventListEnd, rawPositionData);
                        rawPositionData = null;

                        // bump counter
                        importNumEntEvents++;

                        break;

                    case RecordTypes.ACMIRecFeaturePosition:

                        // Read the data
                        if (!StructEncoding<ACMIFeaturePositionData>.Decode(flightFile, out featpos))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        // Allocate a new data node.
                        Debug.Assert(rawPositionData == null);
                        rawPositionData = new ACMIRawPositionData;
                        Debug.Assert(rawPositionData != null);

                        // fill in raw position data
                        rawPositionData.uniqueID = featpos.uniqueID;
                        rawPositionData.leadIndex = featpos.leadUniqueID;
                        rawPositionData.specialFlags = featpos.specialFlags;
                        rawPositionData.slot = featpos.slot;
                        rawPositionData.type = featpos.type;
                        rawPositionData.flags =  EntityFlag.ENTITY_FLAG_FEATURE;

                        rawPositionData.entityPosData.time = hdr.time;
                        rawPositionData.entityPosData.type = PosType.PosTypePos;
                        rawPositionData.entityPosData.posData.x = featpos.x;
                        rawPositionData.entityPosData.posData.y = featpos.y;
                        rawPositionData.entityPosData.posData.z = featpos.z;
                        rawPositionData.entityPosData.posData.roll = featpos.roll;
                        rawPositionData.entityPosData.posData.pitch = featpos.pitch;
                        rawPositionData.entityPosData.posData.yaw = featpos.yaw;

                        // Append our new position data.
                        importPosList = AppendToEndOfList(importPosList, &importPosListEnd, rawPositionData);
                        rawPositionData = null;

                        // bump counter
                        importNumPos++;

                        break;

                    case RecordTypes.ACMICallsignList:

                        // Read the data
                        if (!fread(&import_count, sizeof(long), 1, flightFile))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        Debug.Assert(Import_Callsigns == null);
                        Import_Callsigns = new ACMI_CallRec[import_count];
                        Debug.Assert(Import_Callsigns != null);

                        if (!StructEncoding<ACMIFeaturePositionData>.Decode(flightFile, out Import_Callsigns) ))
                        {
                            CleanupACMIImportPositionData(flightFile, rawPositionData);
                            return false;
                        }

                        break;

                    default:
                        // KCK: I was hitting this repeatidly.. So I'm making it a ShiAssert (and therefore ignorable)
                        // ShiAssert(0);
                        break;
                }

                // save begin and end times
                if (hdr.type != RecordTypes.ACMIRecTodOffset)
                {
                    if (begTime < 0.0)
                        begTime = hdr.time;

                    if (hdr.time > endTime)
                        endTime = hdr.time;
                }
            }

            // build the importEntityList
            log.Debug("ACMITape Import: Parsing Entities ....\n");
            ParseEntities();

            // setup the tape header
            tapeHdr.fileID = ACMITapeHeader.FILEID; // 'TAPE';
            tapeHdr.numEntities = importNumEnt;
            tapeHdr.numFeat = importNumFeat;
            tapeHdr.entityBlockOffset = sizeof(ACMITapeHeader);
            tapeHdr.featBlockOffset = tapeHdr.entityBlockOffset + sizeof(ACMIEntityData) * importNumEnt;
            tapeHdr.timelineBlockOffset = tapeHdr.featBlockOffset + sizeof(ACMIEntityData) * importNumFeat;
            tapeHdr.firstEntEventOffset = tapeHdr.timelineBlockOffset + sizeof(ACMIEntityPositionData) * importNumPos;
            tapeHdr.firstGeneralEventOffset = tapeHdr.firstEntEventOffset + sizeof(ACMIEntityPositionData) * importNumEntEvents;
            tapeHdr.firstEventTrailerOffset = tapeHdr.firstGeneralEventOffset + sizeof(ACMIEventHeader) * importNumEvents;
            tapeHdr.firstFeatEventOffset = tapeHdr.firstEventTrailerOffset + sizeof(ACMIEventTrailer) * importNumEvents;
            tapeHdr.firstTextEventOffset = tapeHdr.firstFeatEventOffset + sizeof(ACMIFeatEvent) * importNumFeatEvents;
            tapeHdr.numEntityPositions = importNumPos;
            tapeHdr.numEvents = importNumEvents;
            tapeHdr.numFeatEvents = importNumFeatEvents;
            tapeHdr.numEntEvents = importNumEntEvents;
            tapeHdr.totPlayTime = endTime - begTime;
            tapeHdr.startTime = begTime;


            // set up the chain offsets of entity positions
            log.Debug("ACMITape Import: Threading Positions ....\n");
            ThreadEntityPositions(tapeHdr);

            // set up the chain offsets of entity events
            log.Debug("ACMITape Import: Threading Entity Events ....\n");
            ThreadEntityEvents(tapeHdr);

            // Calculate size of .vhs file.
            tapeHdr.fileSize = tapeHdr.timelineBlockOffset +
                                   sizeof(ACMIEntityPositionData) * importNumPos +
                                   sizeof(ACMIEntityPositionData) * importNumEntEvents +
                                   sizeof(ACMIEventHeader) * importNumEvents +
                                   sizeof(ACMIFeatEvent) * importNumFeatEvents +
                                   sizeof(ACMIEventTrailer) * importNumEvents;

            // Open a writecopy file mapping.
            // Write out file in .vhs format.
            log.Debug("ACMITape Import: Writing Tape File ....\n");
            WriteTapeFile(outTapeFileName, tapeHdr);

            // Cleanup import data.
            CleanupACMIImportPositionData(flightFile, rawPositionData);

            // now delete the acmi.flt file
            //remove("campaign\\save\\fltfiles\\acmi.flt");
            remove(inFltFile);

            return true;
#endif
            throw new NotImplementedException();
        }

        public static void WriteTapeFile(string fname, ACMITapeHeader tapeHdr)
        {
#if TODO
            int i, j;
            List entityListPtr, posListPtr, eventListPtr;
            ACMIEntityData entityPtr;
            ACMIEventHeader eventPtr;
            ACMIRawPositionData posPtr;
            ACMIFeatEventImportData fePtr;
            Stream tapeFile;
            long ret;
            try
            {
                using (tapeFile = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write))
                {

                    if (tapeFile == null)
                    {
                        log.Debug("Error opening new tape file\n");
                        return;
                    }

                    // write the header
                    StructEncoding<ACMITapeHeader>.Encode(tapeFile, tapeHdr);

                    // write out the entities
                    entityListPtr = importEntityList;

                    for (i = 0; i < importNumEnt; i++)
                    {
                        // entityListPtr = LIST_NTH(importEntityList, i);
                        entityPtr = (ACMIEntityData)entityListPtr.node;

                        StructEncoding<ACMIEntityData>.Encode(tapeFile, entityPtr);

                        entityListPtr = entityListPtr.next;
                    } // end for entity loop

                    // write out the features
                    entityListPtr = importFeatList;

                    for (i = 0; i < importNumFeat; i++)
                    {
                        // entityListPtr = LIST_NTH(importEntityList, i);
                        entityPtr = (ACMIEntityData)entityListPtr.node;

                        StructEncoding<ACMIEntityData>.Encode(tapeFile, entityPtr);
 
                        entityListPtr = entityListPtr.next;
                    } // end for entity loop

                    // write out the entitiy positions
                    posListPtr = importPosList;

                    for (i = 0; i < importNumPos; i++)
                    {
                        // posListPtr = LIST_NTH(importPosList, i);
                        posPtr = (ACMIRawPositionData)posListPtr.node;

                        // we now want to do a "fixup" of the radar targets.  These are
                        // currently in "uniqueIDs" and we want to convert them into
                        // an index into the entity list
                        if (posPtr.entityPosData.posData.radarTarget != -1)
                        {
                            entityListPtr = importEntityList;

                            for (j = 0; j < importNumEnt; j++)
                            {
                                entityPtr = (ACMIEntityData)entityListPtr.node;

                                if (posPtr.entityPosData.posData.radarTarget == entityPtr.uniqueID)
                                {
                                    posPtr.entityPosData.posData.radarTarget = j;
                                    break;
                                }

                                entityListPtr = entityListPtr.next;
                            } // end for entity loop

                            // did we find it?
                            if (j == importNumEnt)
                            {
                                // nope
                                posPtr.entityPosData.posData.radarTarget = -1;
                            }
                        } // end if there's a radar target

                        StructEncoding<ACMIEntityPositionData>.Encode(tapeFile, posPtr.entityPosData);
 
                        posListPtr = posListPtr.next;
                    }

                    // write out the entitiy events
                    posListPtr = importEntEventList;

                    for (i = 0; i < importNumEntEvents; i++)
                    {
                        // posListPtr = LIST_NTH(importPosList, i);
                        posPtr = (ACMIRawPositionData)posListPtr.node;

                        StructEncoding<ACMIEntityPositionData>.Encode(tapeFile, posPtr.entityPosData);
 
                        posListPtr = posListPtr.next;
                    }


                    // allocate the trailer list
                    importEventTrailerList = new ACMIEventTrailer[importNumEvents];
                    Debug.Assert(importEventTrailerList != null);

                    // write out the events
                    eventListPtr = importEventList;

                    for (i = 0; i < importNumEvents; i++)
                    {
                        // eventListPtr = LIST_NTH(importEventList, i);
                        eventPtr = (ACMIEventHeader)eventListPtr.node;

                        // set the trailer data
                        importEventTrailerList[i].index = i;
                        importEventTrailerList[i].timeEnd = eventPtr.timeEnd;

                        StructEncoding<ACMIEventHeader>.Encode(tapeFile, eventPtr);
 
                        eventListPtr = eventListPtr.next;

                    } // end for events loop

                    // now sort the trailers in ascending order by endTime and
                    // write them out
                    qsort(importEventTrailerList,
                          importNumEvents,
                          sizeof(ACMIEventTrailer),
                          CompareEventTrailer);

                    for (i = 0; i < importNumEvents; i++)
                    {
                        StructEncoding<ACMIEventTrailer>.Encode(tapeFile, importEventTrailerList[i]);
                    } // end for events loop

                    // write out the feature events
                    posListPtr = importFeatEventList;

                    for (i = 0; i < importNumFeatEvents; i++)
                    {
                        // posListPtr = LIST_NTH(importPosList, i);
                        fePtr = (ACMIFeatEventImportData)posListPtr.node;

                        StructEncoding<ACMIFeatEvent>.Encode(tapeFile, fePtr.data);
 
                        posListPtr = posListPtr.next;
                    }

                    // finally import and write out the text events
                    ImportTextEventList(tapeFile, tapeHdr);

                    // normal exit
                    tapeFile.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                log.Debug("Error writing new tape file", ex);
            }

            return;
#endif
            throw new NotImplementedException();
        }

        // Time-independent entity access.
        public int NumEntities()
        {
            // Debug.Assert(_tape != null);

            return _tapeHdr.numEntities;
        }

        public int EntityId(int index)
        {
            return (int)(EntityData(index).uniqueID);
        }


        public ushort EntityType(int index)
        {
            return (ushort)(EntityData(index).type);
        }


        // Time-dependent entity access.
        public bool GetEntityPosition
        (
            int index,
            out float x,
            out float y,
            out float z,
            out float yaw,
            out float pitch,
            out float roll,
            out float speed,
            out float turnrate,
            out float turnradius
            )
        {
#if TODO
            float
            deltaTime;

            float dx, dy, dz;
            float dx1, dy1, dz1;

            ACMIEntityPositionData pos1, pos2, pos3;

            // init speed to 0.0
            speed = 0.0f;
            turnrate = 0.0f;
            turnradius = 0.0f;

            Debug.Assert(index >= 0 && index < NumEntities());

            pos1 = CurrentEntityPositionHead(index);

            // If there is not at least 1 positional update, the entity doesn't exist.
            Debug.Assert(pos1 != null);

            if (pos1.time > _simTime)
            {
                x = pos1.posData.x;
                y = pos1.posData.y;
                z = pos1.posData.z;
                yaw = pos1.posData.yaw;
                pitch = pos1.posData.pitch;
                roll = pos1.posData.roll;
                return false;
            }

            pos2 = HeadNext(pos1);

            if (pos2 == null)
            {
                x = pos1.posData.x;
                y = pos1.posData.y;
                z = pos1.posData.z;
                yaw = pos1.posData.yaw;
                pitch = pos1.posData.pitch;
                roll = pos1.posData.roll;
                return false;
            }
            else
            {
                pos3 = HeadPrev(pos1);
                Debug.Assert(pos1.time <= _simTime);
                Debug.Assert(pos2.time > _simTime);

                dx = pos2.posData.x - pos1.posData.x;
                dy = pos2.posData.y - pos1.posData.y;
                dz = pos2.posData.z - pos1.posData.z;

                // Interpolate.
                deltaTime =
                    (
                        (_simTime - pos1.time) /
                        (pos2.time - pos1.time)
                    );

                x =
                    (
                        pos1.posData.x + dx * deltaTime
                    );

                y =
                    (
                        pos1.posData.y + dy * deltaTime
                    );

                z =
                    (
                        pos1.posData.z + dz * deltaTime
                    );

                yaw = AngleInterp(pos1.posData.yaw, pos2.posData.yaw, deltaTime);
                pitch = AngleInterp(pos1.posData.pitch, pos2.posData.pitch, deltaTime);
                roll = AngleInterp(pos1.posData.roll, pos2.posData.roll, deltaTime);

                // get the average speed
                speed = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz) / (pos2.time - pos1.time);
                float dAng = pos2.posData.yaw - pos1.posData.yaw;

                if (Math.Abs(dAng) > 180.0f * Phyconst.DTR)
                {
                    if (dAng >= 0.0f)
                        dAng -= 360.0f * Phyconst.DTR;
                    else
                        dAng += 360.0f * Phyconst.DTR;

                }

                if (pos3 != null)
                {
                    dx1 = pos1.posData.x - pos3.posData.x;
                    dy1 = pos1.posData.y - pos3.posData.y;
                    dz1 = pos1.posData.z - pos3.posData.z;

                    // Turn rate = solid angle delta between velocity vectors
                    turnrate = (float)Math.Acos((dx * dx1 + dy * dy1 + dz * dz1) /
                                           (float)Math.Sqrt((dx * dx + dy * dy + dz * dz) * (dx1 * dx1 + dy1 * dy1 + dz1 * dz1)));
                    turnrate *= Phyconst.RTD / (pos2.time - pos1.time);
                    //    turnrate = RTD * fabs( dAng ) / ( pos2.time - pos1.time );

                    if (turnrate != 0.0f)
                    {
                        // sec to turn 360 deg
                        float secs = 360.0f / turnrate;

                        // get circumference
                        float circum = speed * secs;

                        // now we get turn radius ( circum = 2 * PI * R )
                        turnradius = circum / (2.0f * Phyconst.PI);
                    }
                }
                else
                {
                    turnrate = 0.0F;
                    turnradius = 0.0F;
                }
            }

            return true;
#endif
            throw new NotImplementedException();
        }


        // Prototype of an ACMI_GENERAL_EVENT_CALLBACK:
        // void PoopooCB(ACMITape *tape, EventIdData id, void *eventData, void *userData);
        public void SetGeneralEventCallbacks
        (
            ACMI_GENERAL_EVENT_CALLBACK forwardEventCallback,
            ACMI_GENERAL_EVENT_CALLBACK reverseEventCallback,
            object userData
        )
        {
            _generalEventCallbacks.forwardCallback = forwardEventCallback;
            _generalEventCallbacks.reverseCallback = reverseEventCallback;
            _generalEventCallbacks.userData = userData;
        }


        // Was the tape file found and opened successfully?
        public bool IsLoaded()
        {
            if (_tape == null)
                return false;

            return true;
        }

        // Is the tape paused?
        public bool IsPaused()
        {
            return _paused;
        }

        // Playback controls.
        public void Play()
        {
            _unpause = true;
        }

        public void Pause()
        {
            _paused = true;
            _unpause = false;
        }

        // Step in sim time.
        public void StepTime(float numSeconds)
        {
            _simTime += numSeconds;
            _stepTrail = numSeconds;
            AdvanceAllHeads();
        }

        // return a sim time based on pct into tape parm is
        public float GetNewSimTime(float t)
        {
            float newSimTime;

            // t should be normalized from 0 to 1
            newSimTime = _tapeHdr.startTime + (_tapeHdr.totPlayTime - 0.1f) * t;

            return newSimTime;
        }


        // Play speed controls.
        // This is a ratio of sim time / real time.
        public void SetPlayVelocity(float n)
        {
            _playVelocity = n;
        }

        public float PlayVelocity()
        {
            return _playVelocity;
        }

        // Increase in play velocity per second of real time.
        public void SetPlayAcceleration(float n)
        {
            _playAcceleration = n;
        }

        public float PlayAcceleration()
        {
            return _playAcceleration;
        }

        // This will be used to clamp play velocity.
        // It will be clamped to (-fabs(speed) <= velocity <= fabs(speed));
        public void SetMaxPlaySpeed(float n)
        {
            _maxPlaySpeed = (float)Math.Abs(n);
        }

        public float MaxPlaySpeed()
        {
            return _maxPlaySpeed;
        }

        // Set the read head position.  This should be a number
        // from 0 to 1 (0 = beginning of tape, 1 = end of tape).
        // The input value will be clamped to fit this range.
        public void SetHeadPosition(float t)
        {
            float newSimTime;

            // t should be normalized from 0 to 1
            newSimTime = _tapeHdr.startTime + (_tapeHdr.totPlayTime - 0.1f) * t;

            // run the update cycle until we've reached the new sim time
            while (_simTime != newSimTime)
                Update(newSimTime);

        }

        public float HeadPosition()
        {
            return _simTime;
        }

        // This gives the current simulation time.
        public float SimTime()
        {
            return _simTime;
        }

        public float GetTapePercent()
        {
            // Debug.Assert( _simTime >= _tapeHdr.startTime );
            // Debug.Assert( _tapeHdr.totPlayTime > 0.0f );

            return (_simTime - _tapeHdr.startTime) / _tapeHdr.totPlayTime;
        }

        public void Update(float newSimTime)
        {
            float realTime, deltaRealTime;
            float deltaLimit;

            if (_screenCapturing)
                deltaLimit = 0.0625f;
            else
                deltaLimit = 0.2f;

            // Update active events
            UpdateActiveEvents();

            // Calculate delta time and unpause us if necessary.
            realTime = timeGetTime() * 0.001F;

            // if new sim time is not negative, we are trying to
            // reach a new play position that the user has set with the slider
            // we'll be going in steps until _simTime = newSimTime
            _simulateOnly = false;

            if (newSimTime >= 0.0f)
            {
                _simulateOnly = true;
                deltaLimit = newSimTime - _simTime;
                deltaRealTime = 0.2f;

                if (deltaLimit > 0.0f)
                {
                    _playVelocity = 1.0f;
                }
                else if (deltaLimit < 0.0f)
                {
                    _playVelocity = -1.0f;
                }
                else
                {
                    _playVelocity = 0.0f;
                }

                if (deltaLimit < 0.0f)
                    deltaLimit = -deltaLimit;

                if (deltaRealTime > deltaLimit)
                    deltaRealTime = deltaLimit;

            }
            else if (_unpause && _paused)
            {
                deltaRealTime = 0.0f;
                _paused = false;
            }
            else if (_paused)
            {
                deltaRealTime = 0.0f;
            }
            else
            {
                deltaRealTime = realTime - _lastRealTime;

                // for debugger stops, make sure delta never is larger
                // than 1/5 second
                if (deltaRealTime < -deltaLimit)
                    deltaRealTime = -deltaLimit;

                if (deltaRealTime > deltaLimit)
                    deltaRealTime = deltaLimit;
            }

            _lastRealTime = realTime;
            _unpause = false;

            // Advance time.
            _deltaSimTime = _playVelocity * deltaRealTime;
            _simTime += _deltaSimTime;

            // sanity check -- don't allow the head to go beyond tape ends
            if (_simTime < _tapeHdr.startTime)
                _simTime = _tapeHdr.startTime;

            if (_simTime > _tapeHdr.startTime + _tapeHdr.totPlayTime - 0.1f)
                _simTime = _tapeHdr.startTime + _tapeHdr.totPlayTime - 0.1f;

            _playVelocity += _playAcceleration * deltaRealTime;
            MathUtil.Clamp(ref _playVelocity, -_maxPlaySpeed, _maxPlaySpeed);

            // Advance all entity read heads.
            AdvanceAllHeads();
        }

        /// <summary>
        /// The timeGetTime function retrieves the system time, in milliseconds. The system time is the time elapsed since Windows was started.
        /// </summary>
        /// <returns></returns>
        private long timeGetTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }


        // YPR interpolation
        public float AngleInterp(float begAng, float endAng, float dT)
        {
            float dAng;

            // get the delta angle
            dAng = endAng - begAng;

            // always rotate in shortest direction (ie when delta > 180 deg in mag)
            if (Math.Abs(dAng) > 180.0f * Phyconst.DTR)
            {
                if (dAng >= 0.0f)
                    endAng -= 360.0f * Phyconst.DTR;
                else
                    endAng += 360.0f * Phyconst.DTR;

                dAng = endAng - begAng;
            }


            /*
            if ( endAng < -0.5f * PI && begAng > 0.5f * PI )
             dAng = endAng + ( 2.0f * PI ) - begAng;
            else if ( endAng > 0.5f * PI && begAng < -0.5f * PI )
             dAng = endAng - ( 2.0f * PI ) - begAng;
            else
             dAng = endAng - begAng;
            */

            return (float)(begAng + dAng * dT);
        }

        // access function for sim tape entity
        public SimTapeEntity GetSimTapeEntity(int index)
        {
            Debug.Assert(_simTapeEntities != null);
            Debug.Assert(index < NumEntities());

            return _simTapeEntities[index];
        }



        // does entity exist at current read head
        public bool IsEntityInFrame(int index)
        {
#if TODO
            ACMIEntityPositionData pos1, pos2;

            Debug.Assert(index >= 0 && index < NumEntities());

            pos1 = CurrentEntityPositionHead(index);

            // If there is not at least 1 positional update, the entity doesn't exist.
            Debug.Assert(pos1 != null);

            if (pos1.time > _simTime)
            {
                return false;
            }

            pos2 = HeadNext(pos1);

            if (pos2 == null || pos2.time < _simTime)
            {
                return false;
            }

            return true;
#endif
            throw new NotImplementedException();
        }

        public void InsertEntityInFrame(int index)
        {
#if TODO
            Debug.Assert(_simTapeEntities != null);
            Debug.Assert(index < NumEntities());

            if (_simTapeEntities[index].objBase.drawPointer.InDisplayList())
                return;

            _viewPoint.InsertObject(_simTapeEntities[index].objBase.drawPointer);
#endif
            throw new NotImplementedException();
        }

        public void RemoveEntityFromFrame(int index)
        {
#if TODO
            Debug.Assert(_simTapeEntities != null);
            Debug.Assert(index < NumEntities());

            if (!_simTapeEntities[index].objBase.drawPointer.InDisplayList())
                return;

            _viewPoint.RemoveObject(_simTapeEntities[index].objBase.drawPointer);
#endif
            throw new NotImplementedException();
        }


        // get the entity's current radar target (entity index returned)
        public int GetEntityCurrentTarget(int index)
        {
#if TODO
            ACMIEntityPositionData pos1, pos2;

            Debug.Assert(index >= 0 && index < NumEntities());

            pos1 = CurrentEntityPositionHead(index);

            // If there is not at least 1 positional update, the entity doesn't exist.
            Debug.Assert(pos1 != null);

            if (pos1.time > _simTime)
            {
                return -1;
            }

            pos2 = HeadNext(pos1);

            if (pos2 == null || pos2.time < _simTime)
            {
                return -1;
            }

            return pos1.posData.radarTarget;
#endif
            throw new NotImplementedException();
        }

        // update sim tape Entities for this frame
        public void UpdateSimTapeEntities()
        {
#if TODO
            int
            i,
            numEntities;

            SimTapeEntity ep;

            Tpoint pos;
            Tpoint wtpos;
            Trotation rot;
            Tpoint newPoint;




            Debug.Assert(_simTapeEntities != null);

            // create array of SimTapeEntity
            numEntities = NumEntities();
            _renderer.SetColor(0xffff0000);

            // for each entity, create it's object stuff....
            for (i = 0; i < numEntities; i++)
            {
                // get pointer
                ep = _simTapeEntities[i];


                if (GetEntityPosition(i, out ep.x, out ep.y, out ep.z, out ep.yaw, out ep.pitch, out ep.roll, out ep.aveSpeed, out ep.aveTurnRate, out ep.aveTurnRadius) == false)
                {
                    // make sure we remove from draw list
                    if (ep.objBase.drawPointer.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBase.drawPointer);

                        // remove trail too
                        if (ep.objTrail && ep.objTrail.InDisplayList() && _simTime < ep.trailStartTime)
                        {
                            ep.objTrail.TrimTrail(0);
                            _viewPoint.RemoveObject(ep.objTrail);
                        }

                        // remove Bsp1 too
                        if (ep.objBsp1 != null && ep.objBsp1.InDisplayList() && _simTime < ep.trailStartTime)
                        {
                            _viewPoint.RemoveObject(ep.objBsp1);
                        }

                        // remove Bsp2 too
                        if (ep.objBsp2 != null && ep.objBsp2.InDisplayList() && _simTime < ep.trailStartTime)
                        {
                            _viewPoint.RemoveObject(ep.objBsp2);
                        }

                        // remove 2d too
                        if (ep.obj2d != null && ep.obj2d.InDisplayList())
                        {
                            _viewPoint.RemoveObject(ep.obj2d);
                        }
                    }

                    continue;
                }


                ////////////////////////////////////////////////////////////////////////



                ////////////////////////////////////////////////////////////


                // set the scalar value...// put check in if Isometric view
                ep.objBase.drawPointer.SetScale(_tapeObjScale);

                // if the entity is an aircraft we set its labels
                if (ep.flags & ENTITY_FLAG_AIRCRAFT)
                {
                    float tmp;
                    string tmpstr;

                    DrawablePoled dp = (DrawablePoled)ep.objBase.drawPointer;


                    // LOCK RANGES
                    // get the target entity
                    Tpoint posb;
                    int tgt = GetEntityCurrentTarget(i);
                    float distance;

                    if (tgt > 0)
                    {
                        SimTapeEntity targep = GetSimTapeEntity(tgt);
                        ep.objBase.drawPointer.GetPosition(&pos);
                        targep.objBase.drawPointer.GetPosition(&posb);
                        distance = (float)(Phyconst.FT_TO_NM * (float)Math.Sqrt(((pos.x - posb.x) * (pos.x - posb.x) + (pos.y - posb.y) * (pos.y - posb.y))));
                    }
                    else distance = 0;

                    tmp = distance;
                    tmpstr = String.Format("{0:0.0} Rng", tmp);
                    dp.SetDataLabel(DP_LABEL_LOCK_RANGE, tmpstr);



                    {
                        // heading
                        tmp = ep.yaw * Phyconst.RTD;

                        if (tmp < 0.0f)
                            tmp += 360.0f;

                        tmpstr = String.Format("{0:0.0} Deg", tmp);
                        dp.SetDataLabel(DP_LABEL_HEADING, tmpstr);
                    }
                    // alt
                    tmpstr = String.Format("{0:0.0} ft", -ep.z);
                    dp.SetDataLabel(DP_LABEL_ALT, tmpstr);

                    // speed
                    // tmp=ep.aveSpeed * FTPSEC_TO_KNOTS;
                    tmp = CalcKIAS(ep.aveSpeed, -ep.z);
                    tmpstr = String.Format("{0:0.0} Kts", tmp);
                    dp.SetDataLabel(DP_LABEL_SPEED, tmpstr);

                    // turn rate
                    tmp = ep.aveTurnRate;
                    tmpstr = String.Format("{0:0.0} deg/s", tmp);
                    dp.SetDataLabel(DP_LABEL_TURNRATE, tmpstr);

                    // turn radius
                    tmp = ep.aveTurnRadius;
                    tmpstr = String.Format("{0:0.0} ft", tmp);
                    dp.SetDataLabel(DP_LABEL_TURNRADIUS, tmpstr);
                }


                // update object's position
                ep.objBase.SetPosition
                (
                    ep.x,
                    ep.y,
                    ep.z
                );
                ep.objBase.SetYPR
                (
                    ep.yaw,
                    ep.pitch,
                    ep.roll
                );
                // just to make sure....
                ep.objBase.SetYPRDelta
                (
                    0.0f,
                    0.0f,
                    0.0f
                );

                // set the matrix
                CalcTransformMatrix(ep.objBase);

                // get position and matrix for drawable BSP
                ObjectSetData(ep.objBase, &pos, &rot);

                // update the BSP
                if (ep.flags & ENTITY_FLAG_FLARE)
                    ((Drawable2D)ep.objBase.drawPointer).SetPosition(&pos);
                else
                    ((DrawableBSP)ep.objBase.drawPointer).Update(&pos, &rot);

                ////////////////////////////////////////////////////////////////////////////////////

                // BING 3-20-98
                // TURN ON LABELS FOR ENTITIES.
                // acmiView.GetObjectName(acmiView.Tape().GetSimTapeEntity(i).objBase,tmpStr);
                // ((DrawableBSP *)ep.objBase.drawPointer).SetLabel(ep.name , labelColor );

                /////////////////////////////////////////////////////////////////////////////////////


                // entity is in the frame .....
                // make sure we tell draw loop to draw it
                if (!ep.objBase.drawPointer.InDisplayList())
                {
                    _viewPoint.InsertObject(ep.objBase.drawPointer);
                }

                // likewise for 2d portion
                if (ep.obj2d != null)
                {
                    if (!ep.obj2d.InDisplayList())
                        _viewPoint.InsertObject(ep.obj2d);

                    ep.obj2d.SetPosition(pos);
                }

                // do the wing trails if turned on
                // edg: partial hack.  For regen in dogfight the wingtrails are
                // continue from dead pos to new position.  Since we don't have the
                // info to detect a regen, if we see that the airspeed is too high
                // trim the trails back to 0
                if (_wingTrails && (ep.flags & ENTITY_FLAG_AIRCRAFT) && CalcKIAS(ep.aveSpeed, -ep.z) > 1100.0f)
                {
                    ep.wrTrail.TrimTrail(0);
                    ep.wlTrail.TrimTrail(0);
                    ep.wtLength = 0;
                }
                else if (_wingTrails && (ep.flags & ENTITY_FLAG_AIRCRAFT))
                {
                    if (_playVelocity < 0.0f && (!_paused || _simulateOnly))
                    {
                        ep.wtLength -= ep.wrTrail.RewindTrail((uint)(_simTime * 1000));
                        ep.wlTrail.RewindTrail((uint)(_simTime * 1000));
                    }
                    else if (_playVelocity > 0.0f && (!_paused || _simulateOnly))
                    {
                        ep.wtLength++;
                        wtpos.x = ep.objBase.dmx[1][0] * -20.0f * _tapeObjScale + ep.x;
                        wtpos.y = ep.objBase.dmx[1][1] * -20.0f * _tapeObjScale + ep.y;
                        wtpos.z = ep.objBase.dmx[1][2] * -20.0f * _tapeObjScale + ep.z;
                        ep.wlTrail.AddPointAtHead(&wtpos, (uint)(_simTime * 1000));
                        wtpos.x = ep.objBase.dmx[1][0] * 20.0f * _tapeObjScale + ep.x;
                        wtpos.y = ep.objBase.dmx[1][1] * 20.0f * _tapeObjScale + ep.y;
                        wtpos.z = ep.objBase.dmx[1][2] * 20.0f * _tapeObjScale + ep.z;
                        ep.wrTrail.AddPointAtHead(&wtpos, (uint)(_simTime * 1000));

                        /* ep.wtLength++;
                         wtpos.x = ep.objBase.dmx[1][0] * -40.0f + ep.x;
                         wtpos.y = ep.objBase.dmx[1][1] * -40.0f + ep.y;
                         wtpos.z = ep.objBase.dmx[1][2] * -40.0f + ep.z;
                         ep.wlTrail.AddPointAtHead( &wtpos, (DWORD)(_simTime * 1000) );
                         wtpos.x = ep.objBase.dmx[1][0] * 40.0f + ep.x;
                         wtpos.y = ep.objBase.dmx[1][1] * 40.0f + ep.y;
                         wtpos.z = ep.objBase.dmx[1][2] * 40.0f + ep.z;
                         ep.wrTrail.AddPointAtHead( &wtpos, (DWORD)(_simTime * 1000) );
                        */



                    }
                    else if (_stepTrail < 0.0f)
                    {
                        ep.wtLength -= ep.wrTrail.RewindTrail((uint)(_simTime * 1000));
                        ep.wlTrail.RewindTrail((uint)(_simTime * 1000));
                    }
                    else if (_stepTrail > 0.0f)
                    {
                        ep.wtLength++;
                        wtpos.x = ep.objBase.dmx[1][0] * -20.0f * _tapeObjScale + ep.x;
                        wtpos.y = ep.objBase.dmx[1][1] * -20.0f * _tapeObjScale + ep.y;
                        wtpos.z = ep.objBase.dmx[1][2] * -20.0f * _tapeObjScale + ep.z;
                        ep.wlTrail.AddPointAtHead(&wtpos, (uint)(_simTime * 1000));
                        wtpos.x = ep.objBase.dmx[1][0] * 20.0f * _tapeObjScale + ep.x;
                        wtpos.y = ep.objBase.dmx[1][1] * 20.0f * _tapeObjScale + ep.y;
                        wtpos.z = ep.objBase.dmx[1][2] * 20.0f * _tapeObjScale + ep.z;
                        ep.wrTrail.AddPointAtHead(&wtpos, (uint)(_simTime * 1000));


                    }

                    if (ep.wtLength != _wtMaxLength)   // MLR 12/14/2003 -
                    {
                        ep.wrTrail.TrimTrail(_wtMaxLength);
                        ep.wlTrail.TrimTrail(_wtMaxLength);
                        ep.wtLength = _wtMaxLength;
                    }
                }

                // check for trail
                if (!ep.objTrail)
                    continue;

                // we need to deal with the trail....

                // if the trail end time is before the read head, we do nothing
                if (_simTime > ep.trailEndTime)
                {
                    // remove Bsp1 too
                    if (ep.objBsp1 && ep.objBsp1.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBsp1);
                    }

                    // remove Bsp2 too
                    if (ep.objBsp2 && ep.objBsp2.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBsp2);
                    }

                    continue;
                }

                // if the trail start time is after the current read head,
                // make sure trail is no longer in display list
                if (_simTime < ep.trailStartTime)
                {
                    if (ep.objTrail.InDisplayList())
                    {
                        ep.objTrail.TrimTrail(0);
                        _viewPoint.RemoveObject(ep.objTrail);
                    }

                    // remove Bsp1 too
                    if (ep.objBsp1 && ep.objBsp1.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBsp1);
                    }

                    // remove Bsp2 too
                    if (ep.objBsp2 && ep.objBsp2.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBsp2);
                    }

                    continue;
                }

                // the read head is between trail start and end times
                // we need need to determine if we're moving forwards or
                // backwards in time, if back we rewind the trail, otherwise
                // add a new point
                if (!ep.objTrail.InDisplayList())
                {
                    _viewPoint.InsertObject(ep.objTrail);
                }

                // insert Bsp1 too
                if (ep.objBsp1 && !ep.objBsp1.InDisplayList())
                {
                    _viewPoint.InsertObject(ep.objBsp1);
                }

                // insert Bsp2 too
                if (ep.objBsp2 && !ep.objBsp2.InDisplayList())
                {
                    _viewPoint.InsertObject(ep.objBsp2);
                }

                // update the BSPs
                // placement a bit behind the missile
                newPoint = pos;
                newPoint.x += ep.objBase.dmx[0][0] * -7.0f;
                newPoint.y += ep.objBase.dmx[0][1] * -7.0f;
                newPoint.z += ep.objBase.dmx[0][2] * -7.0f;

                if (ep.objBsp1 != null)
                    ep.objBsp1.Update(&newPoint, &rot);

                if (ep.objBsp2 != null)
                    ep.objBsp2.Update(&newPoint, &rot);

                newPoint.x += ep.objBase.dmx[0][0] * -30.0f;
                newPoint.y += ep.objBase.dmx[0][1] * -30.0f;
                newPoint.z += ep.objBase.dmx[0][2] * -30.0f;



                if (_playVelocity < 0.0f && (!_paused || _simulateOnly))
                {
                    ep.objTrail.RewindTrail((uint)(_simTime * 1000));
                }
                else if (_playVelocity > 0.0f && (!_paused || _simulateOnly))
                {
                    ep.objTrail.AddPointAtHead(&newPoint, (uint)(_simTime * 1000));
                }
                else if (_stepTrail < 0.0f)
                {
                    ep.objTrail.RewindTrail((uint)(_simTime * 1000));
                }
                else if (_stepTrail > 0.0f)
                {
                    ep.objTrail.AddPointAtHead(&newPoint, (uint)(_simTime * 1000));
                }


            }

            _stepTrail = 0.0f;
#endif
            throw new NotImplementedException();
        }

        // sets the draw position and matrix for bsp update
        public void ObjectSetData(SimBaseClass obj, Tpoint simView, Trotation viewRotation)
        {
#if TODO
            viewRotation.M11 = obj.dmx[0][0];
            viewRotation.M21 = obj.dmx[0][1];
            viewRotation.M31 = obj.dmx[0][2];

            viewRotation.M12 = obj.dmx[1][0];
            viewRotation.M22 = obj.dmx[1][1];
            viewRotation.M32 = obj.dmx[1][2];

            viewRotation.M13 = obj.dmx[2][0];
            viewRotation.M23 = obj.dmx[2][1];
            viewRotation.M33 = obj.dmx[2][2];

            // Update object position
            simView.x = obj.XPos();
            simView.y = obj.YPos();
            simView.z = obj.ZPos();

#endif
            throw new NotImplementedException();
        }

        public void SetScreenCapturing(bool val)
        {
            _screenCapturing = val;
        }

        public void SetObjScale(float val)
        {
            _tapeObjScale = val;
        }

        public float GetObjScale()
        {
            return _tapeObjScale;
        }

        public float GetDeltaSimTime()
        {
            return _deltaSimTime;
        }

        public void SetWingTrails(bool turnOn)
        {
#if TODO
            int i, numEntities;

            SimTapeEntity ep;

            Debug.Assert(_simTapeEntities != null);
            Debug.Assert(_tape != null);

            // set flag
            _wingTrails = turnOn;

            // create array of SimTapeEntity
            numEntities = NumEntities();

            // for each entity, create it's object stuff....
            for (i = 0; i < numEntities; i++)
            {
                // get the tape entity data
                ep = _simTapeEntities[i];

                if ((ep.flags & ENTITY_FLAG_AIRCRAFT) == 0)
                {
                    continue;
                }

                if (turnOn)
                {
                    // turn trails on
                    if (!ep.wrTrail.InDisplayList())
                        _viewPoint.InsertObject(ep.wrTrail);

                    if (!ep.wlTrail.InDisplayList())
                        _viewPoint.InsertObject(ep.wlTrail);
                }
                else
                {
                    // turn trails off
                    if (ep.wrTrail.InDisplayList())
                        _viewPoint.RemoveObject(ep.wrTrail);

                    if (ep.wlTrail.InDisplayList())
                        _viewPoint.RemoveObject(ep.wlTrail);
                }

                ep.wrTrail.TrimTrail(0);
                ep.wlTrail.TrimTrail(0);
                ep.wtLength = 0;

            }
#endif 
            throw new NotImplementedException();
        }

        public void SetWingTrailLength(int val)
        {
            _wtMaxLength = val;
        }

        public object GetTextEvents(out int count)
        {
#if TODO
            if (_tapeHdr.numTextEvents > 1048576) // Sanity check
            {
                count = 0;
                return null;
            }

            count = _tapeHdr.numTextEvents;
            return (object)((char*)_tape + _tapeHdr.firstTextEventOffset);
#endif
            throw new NotImplementedException();
        }

        public int GetCallsignList(out int count)
        {
            if (_tapeHdr.numTextEvents > 1048576) // Sanity check
            {
                count = 0;
                return -1;
            }

            count = _tape.ReadInt32(_tapeHdr.firstTextEventOffset + _tapeHdr.numTextEvents * ACMITextEvent.Size);

            return _tapeHdr.firstTextEventOffset + _tapeHdr.numTextEvents * ACMITextEvent.Size + sizeof(int);
        }



        // list of sim entities from the tape that are manipulated and drawn
        public SimTapeEntity[] _simTapeEntities;
        public ACMIEntityData EntityData(int index)
        {
            // Debug.Assert(_tape != null);
            // Debug.Assert(index >= 0 && index < NumEntities());
            int pos = ACMITapeHeader.Size + index * ACMIEntityData.Size;
            ACMIEntityData rst;
            _tape.Read<ACMIEntityData>(pos, out rst);
            return rst;
        }


        public float GetTodOffset()
        {
            return _tapeHdr.todOffset;
        }



        private void Init()
        {
            if (_entityReadHeads != null)
            {
                //delete[] _entityReadHeads;
                _entityReadHeads = null;
            }

            if (_simTapeEntities != null)
            {
                CleanupSimTapeEntities();
            }

            if (_eventList != null)
            {
                CleanupEventList();
            }

            SetGeneralEventCallbacks
            (
                null,
                null,
                null
            );

            if (_tape != null)
            {
                // close file mapping.
                CloseTapeFile();
            }

            _playVelocity = 0.0f;
            _playAcceleration = 0.0f;
            _maxPlaySpeed = 4.0f;

            _simTime = 0.0f;
            _stepTrail = 0.0f;

            _lastRealTime = 0.0f;


            _unpause = false;
            _paused = true;
            _simulateOnly = false;

            _generalEventReadHeadHeader = 0;
            //_featEventReadHead = null;
            //_generalEventReadHeadTrailer = null;
        }

        // These are used for importation.
        private static void ParseEntities()
        {
#if TODO
            int
            i = 0,
            count = 0;

            List entityPtr, rawList;

            ACMIRawPositionData entityType;

            ACMIEntityData importEntityInfo;

            importEntityList = null;

            rawList = importPosList;

            for (count = 0; count < importNumPos; count++)
            {
                // rawList = LIST_NTH(importPosList, count);
                entityType = (ACMIRawPositionData)rawList.node;

                if (entityType.flags & EntityFlag.ENTITY_FLAG_FEATURE)
                {
                    // look for existing entity
                    entityPtr = importFeatList;

                    for (i = 0; i < importNumFeat; i++)
                    {
                        // entityPtr = LIST_NTH(importEntityList, i);
                        importEntityInfo = (ACMIEntityData)entityPtr.node;

                        if (entityType.uniqueID == importEntityInfo.uniqueID)
                        {
                            break;
                        }

                        entityPtr = entityPtr.next;
                    }

                    // create new import entity record
                    if (i == importNumFeat)
                    {
                        importEntityInfo = new ACMIEntityData;
                        importEntityInfo.count = 0;

                        Debug.Assert(importEntityInfo != null);
                        importEntityInfo.uniqueID = entityType.uniqueID;
                        importEntityInfo.type = entityType.type;
                        importEntityInfo.flags = entityType.flags;
                        importEntityInfo.leadIndex = entityType.leadIndex;
                        importEntityInfo.specialFlags = entityType.specialFlags;
                        importEntityInfo.slot = entityType.slot;
                        importFeatList = AppendToEndOfList(importFeatList, &importFeatListEnd, importEntityInfo);
                        importNumFeat++;
                    }
                }
                else
                {
                    // not a feature

                    // look for existing entity
                    entityPtr = importEntityList;

                    for (i = 0; i < importNumEnt; i++)
                    {
                        // entityPtr = LIST_NTH(importEntityList, i);
                        importEntityInfo = (ACMIEntityData)entityPtr.node;

                        if (entityType.uniqueID == importEntityInfo.uniqueID)
                        {
                            break;
                        }

                        entityPtr = entityPtr.next;
                    }

                    // create new import entity record
                    if (i == importNumEnt)
                    {
                        importEntityInfo = new ACMIEntityData;
                        importEntityInfo.count = 0;

                        Debug.Assert(importEntityInfo != null);
                        importEntityInfo.uniqueID = entityType.uniqueID;
                        importEntityInfo.type = entityType.type;
                        importEntityInfo.flags = entityType.flags;
                        // remove importEntityInfo.teamColor = entityType.entityPosData.teamColor;
                        // remove strcpy((importEntityInfo.label), (char*) entityType.entityPosData.label);

                        importEntityList = AppendToEndOfList(importEntityList, &importEntityListEnd, importEntityInfo);
                        importNumEnt++;
                    }
                }

                rawList = rawList.next;
            }

            // Count instances of each unique type
            List list1 = importEntityList;
            List list2;
            ACMIEntityData thing1;
            ACMIEntityData thing2;
            int objCount;

            while (list1 != null)
            {
                thing1 = (ACMIEntityData)list1.node;

                if (thing1.count == 0)
                {
                    thing1.count = 1;
                    objCount = 2;
                    list2 = list1.next;

                    while (list2 != null)
                    {
                        thing2 = (ACMIEntityData)list2.node;

                        if (thing2.type == thing1.type && thing2.count == 0)
                        {
                            thing2.count = objCount;
                            objCount++;
                        }

                        list2 = list2.next;
                    }
                }

                list1 = list1.next;
            }
#endif 
            throw new NotImplementedException();

        }

        private static void ThreadEntityPositions(ACMITapeHeader tapeHdr)
        {
#if TODO
            int i, j;
            long prevOffset;
            List entityListPtr, posListPtr, featListPtr;
            ACMIEntityData entityPtr, featPtr;
            ACMIRawPositionData posPtr;
            ACMIRawPositionData prevPosPtr;
            ACMIFeatEventImportData fePtr;
            bool foundFirst;
            long currOffset;

            // we run an outer and inner loop here.
            // the outer loops steps thru each entity
            // the inner loop searches each position update for one owned by the
            // entity and chains them together

            entityListPtr = importEntityList;

            for (i = 0; i < importNumEnt; i++)
            {
                // entityListPtr = LIST_NTH(importEntityList, i);
                entityPtr = (ACMIEntityData)entityListPtr.node;
                foundFirst = false;
                prevOffset = 0;
                prevPosPtr = null;
                entityPtr.firstPositionDataOffset = 0;

                posListPtr = importPosList;

                for (j = 0; j < importNumPos; j++)
                {
                    // posListPtr = LIST_NTH(importPosList, j);
                    posPtr = (ACMIRawPositionData)posListPtr.node;

                    // check the id to see if this position belongs to the entity
                    if (posPtr.uniqueID != entityPtr.uniqueID)
                    {
                        // nope
                        posListPtr = posListPtr.next;
                        continue;
                    }

                    // calculate the offset of this positional record
                    currOffset = tapeHdr.timelineBlockOffset +
                                 sizeof(ACMIEntityPositionData) * j;

                    // if it's the 1st in the chain, set the offset to it in
                    // the entity's record
                    if (foundFirst == false)
                    {
                        entityPtr.firstPositionDataOffset = currOffset;
                        foundFirst = true;
                    }

                    // thread current to previous
                    posPtr.entityPosData.prevPositionUpdateOffset = prevOffset;
                    posPtr.entityPosData.nextPositionUpdateOffset = 0;

                    // thread previous to current
                    if (prevPosPtr != null)
                    {
                        prevPosPtr.entityPosData.nextPositionUpdateOffset = currOffset;
                    }

                    // set vals for next time thru loop
                    prevOffset = currOffset;
                    prevPosPtr = posPtr;

                    // next in list
                    posListPtr = posListPtr.next;

                } // end for position loop

                entityListPtr = entityListPtr.next;
            } // end for entity loop

            entityListPtr = importFeatList;

            for (i = 0; i < importNumFeat; i++)
            {
                entityPtr = (ACMIEntityData)entityListPtr.node;
                foundFirst = false;
                prevOffset = 0;
                prevPosPtr = null;
                entityPtr.firstPositionDataOffset = 0;

                posListPtr = importPosList;

                for (j = 0; j < importNumPos; j++)
                {
                    // posListPtr = LIST_NTH(importPosList, j);
                    posPtr = (ACMIRawPositionData)posListPtr.node;

                    // check the id to see if this position belongs to the entity
                    if (posPtr.uniqueID != entityPtr.uniqueID)
                    {
                        // nope
                        posListPtr = posListPtr.next;
                        continue;
                    }

                    // calculate the offset of this positional record
                    currOffset = tapeHdr.timelineBlockOffset +
                                 sizeof(ACMIEntityPositionData) * j;

                    // if it's the 1st in the chain, set the offset to it in
                    // the entity's record
                    if (foundFirst == false)
                    {
                        entityPtr.firstPositionDataOffset = currOffset;
                        foundFirst = true;
                    }

                    // thread current to previous
                    posPtr.entityPosData.prevPositionUpdateOffset = prevOffset;
                    posPtr.entityPosData.nextPositionUpdateOffset = 0;

                    // thread previous to current
                    if (prevPosPtr != null)
                    {
                        prevPosPtr.entityPosData.nextPositionUpdateOffset = currOffset;
                    }

                    // set vals for next time thru loop
                    prevOffset = currOffset;
                    prevPosPtr = posPtr;

                    // next in list
                    posListPtr = posListPtr.next;

                } // end for position loop

                // while we're doing the features, for each one, go thru the
                // feature event list looking for our unique ID in the events
                // and setting the index value of our feature in the event
                posListPtr = importFeatEventList;

                for (j = 0; j < importNumFeatEvents; j++)
                {
                    // posListPtr = LIST_NTH(importPosList, j);
                    fePtr = (ACMIFeatEventImportData)posListPtr.node;

                    // check the id to see if this event belongs to the entity
                    if (fePtr.uniqueID == entityPtr.uniqueID)
                    {
                        fePtr.data.index = i;
                    }

                    // next in list
                    posListPtr = posListPtr.next;

                } // end for feature event loop

                // now go thru the feature list again and find lead unique ID's and
                // change them to indices into the list

                // actually NOW, go through and just make sure they exist... otherwise, clear
                if (entityPtr.leadIndex != -1)
                {
                    featListPtr = importFeatList;

                    for (j = 0; j < importNumFeat; j++)
                    {
                        // we don't compare ourselves
                        if (j != i)
                        {
                            featPtr = (ACMIEntityData)featListPtr.node;

                            if (entityPtr.leadIndex == featPtr.uniqueID)
                            {
                                entityPtr.leadIndex = j;
                                break;
                            }

                        }

                        // next in list
                        featListPtr = featListPtr.next;
                    }

                    // if we're gone thru the whole list and haven't found
                    // a lead index, we're in trouble.  To protect, set the
                    // lead index to -1
                    if (j == importNumFeat)
                    {
                        entityPtr.leadIndex = -1;
                    }
                }

                entityListPtr = entityListPtr.next;
            } // end for feature entity loop
#endif 
            throw new NotImplementedException();
        }

        private static void ThreadEntityEvents(ACMITapeHeader tapeHdr)
        {
#if TODO
            int i, j;
            long prevOffset;
            List entityListPtr, posListPtr;
            ACMIEntityData entityPtr;
            ACMIRawPositionData posPtr;
            ACMIRawPositionData prevPosPtr;
            bool foundFirst;
            long currOffset;

            // we run an outer and inner loop here.
            // the outer loops steps thru each entity
            // the inner loop searches each position update for one owned by the
            // entity and chains them together

            entityListPtr = importEntityList;

            for (i = 0; i < importNumEnt; i++)
            {
                // entityListPtr = LIST_NTH(importEntityList, i);
                entityPtr = (ACMIEntityData)entityListPtr.node;
                foundFirst = false;
                prevOffset = 0;
                prevPosPtr = null;
                entityPtr.firstEventDataOffset = 0;

                posListPtr = importEntEventList;

                for (j = 0; j < importNumEntEvents; j++)
                {
                    // posListPtr = LIST_NTH(importPosList, j);
                    posPtr = (ACMIRawPositionData)posListPtr.node;

                    // check the id to see if this position belongs to the entity
                    if (posPtr.uniqueID != entityPtr.uniqueID)
                    {
                        // nope
                        posListPtr = posListPtr.next;
                        continue;
                    }

                    // calculate the offset of this positional record
                    currOffset = tapeHdr.firstEntEventOffset +
                                 sizeof(ACMIEntityPositionData) * j;

                    // if it's the 1st in the chain, set the offset to it in
                    // the entity's record
                    if (foundFirst == false)
                    {
                        entityPtr.firstEventDataOffset = currOffset;
                        foundFirst = true;
                    }

                    // thread current to previous
                    posPtr.entityPosData.prevPositionUpdateOffset = prevOffset;
                    posPtr.entityPosData.nextPositionUpdateOffset = 0;

                    // thread previous to current
                    if (prevPosPtr != null)
                    {
                        prevPosPtr.entityPosData.nextPositionUpdateOffset = currOffset;
                    }

                    // set vals for next time thru loop
                    prevOffset = currOffset;
                    prevPosPtr = posPtr;

                    // next in list
                    posListPtr = posListPtr.next;

                } // end for position loop

                entityListPtr = entityListPtr.next;
            } // end for entity loop
#endif
            throw new NotImplementedException();
        }

        private static void ImportTextEventList(Stream fd, ACMITapeHeader tapeHdr)
        {
#if TODO
            EventElement cur;
            long ret;
            ACMITextEvent te;
            string timestr;//[20];

            tapeHdr.numTextEvents = 0;

            cur = ProcessEventListForACMI();

            memset(&te, 0, sizeof(ACMITextEvent));

            // PJW Totally rewrote event debriefing stuff... thus the new code
            while (cur != null)
            {
                te.intTime = cur.eventTime;
                GetTimeString(cur.eventTime, timestr);

                _tcscpy(te.timeStr, timestr + 3);
                _tcscpy(te.msgStr, cur.eventString);

                // KCK: Edit out some script info which is used in debreiefings
                _TCHAR* strptr = _tcschr(te.msgStr, '@');

                if (strptr)
                {
                    strptr[0] = ' ';
                    strptr[1] = '-';
                    strptr[2] = ' ';
                }

                ret = fwrite(&te, sizeof(ACMITextEvent), 1, fd);

                if (!ret)
                {
                    log.Debug("Error writing TAPE event element\n");
                    break;
                }

                tapeHdr.numTextEvents++;

                // next one
                cur = cur.next;

            } // end for events loop


            // write callsign list
            if (Import_Callsigns)
            {
                ret = fwrite(&import_count, sizeof(long), 1, fd);

                if (!ret)
                    goto error_exit;

                ret = fwrite(Import_Callsigns, import_count * sizeof(ACMI_CallRec), 1, fd);

                if (!ret)
                    goto error_exit;
            }

            // write the header again (bleck)
            ret = fseek(fd, 0, SEEK_SET);

            if (ret != 0)
            {
                log.Debug("Error seeking TAPE start\n");
                goto error_exit;
            }

            ret = fwrite(tapeHdr, sizeof(ACMITapeHeader), 1, fd);

            if (!ret)
            {
                log.Debug("Error writing TAPE header again\n");
            }

            error_exit:
            // free up mem
            // DisposeEventList(evList);
            ClearSortedEventList();
#endif 
            throw new NotImplementedException();
        }

        // Get at the entity data.
        private ACMIEntityData FeatureData(int index)
        {
            // Debug.Assert(_tape != null);
            // Debug.Assert(index >= 0 && index < _tapeHdr.numFeat);
            int pos = _tapeHdr.featBlockOffset + index * ACMIEntityData.Size;
            ACMIEntityData rst;
            _tape.Read<ACMIEntityData>(pos, out rst);
            return rst;
        }

        private ACMIEntityPositionData? CurrentFeaturePositionHead(int index)
        {
            Debug.Assert(_tape != null);
            Debug.Assert(index >= 0 && index < _tapeHdr.numFeat);

            ACMIEntityData e = FeatureData(index);
            int positionOffset = _tapeHdr.featBlockOffset;
            if (positionOffset == 0) return null;
            ACMIEntityPositionData pd;
            _tape.Read<ACMIEntityPositionData>(positionOffset, out pd);
            return pd;
        }

        // Traverse an entity's position update thread.
        private ACMIEntityPositionData? CurrentEntityPositionHead(int index)
        {
            Debug.Assert(_tape != null);
            Debug.Assert(_entityReadHeads != null);
            Debug.Assert(index >= 0 && index < NumEntities());

            int positionOffset = _entityReadHeads[index].positionDataOffset;
            if (positionOffset == 0) return null;
            ACMIEntityPositionData pd;
            _tape.Read<ACMIEntityPositionData>(positionOffset, out pd);
            return pd;
        }

        private ACMIEntityPositionData? CurrentEntityEventHead(int index)
        {
            Debug.Assert(_tape != null);
            Debug.Assert(_entityReadHeads != null);
            Debug.Assert(index >= 0 && index < NumEntities());

            int positionOffset = _entityReadHeads[index].eventDataOffset;
            if (positionOffset == 0) return null;
            ACMIEntityPositionData pd;
            _tape.Read<ACMIEntityPositionData>(positionOffset, out pd);
            return pd;
        }


        private ACMIEntityPositionData? HeadNext(ACMIEntityPositionData? current)
        {
            Debug.Assert(_tape != null);
            Debug.Assert(_entityReadHeads != null);

            if (current == null) return null;

            int positionOffset = current.Value.nextPositionUpdateOffset;
            if (positionOffset == 0) return null;
            ACMIEntityPositionData pd;
            _tape.Read<ACMIEntityPositionData>(positionOffset, out pd);
            return pd;
        }


        private ACMIEntityPositionData? HeadPrev(ACMIEntityPositionData? current)
        {
            Debug.Assert(_tape != null);
            Debug.Assert(_entityReadHeads != null);

            int positionOffset = current.Value.prevPositionUpdateOffset;
            if (positionOffset == 0) return null;
            ACMIEntityPositionData pd;
            _tape.Read<ACMIEntityPositionData>(positionOffset, out pd);
            return pd;
        }


        // Traverse an event thread.
        private ACMIEventHeader? GeneralEventData()
        {
            Debug.Assert(_tape != null);

            return GetGeneralEventData(_generalEventReadHeadHeader);
        }

        private ACMIEventHeader? GetGeneralEventData(int i)
        {
            Debug.Assert(_tape != null);

            if (i < 0 || i >= _tapeHdr.numEvents)
            {
                return null;
            }
            int positionOffset = _tapeHdr.firstGeneralEventOffset + ACMIEventHeader.Size * i;
            if (positionOffset == 0) return null;
            ACMIEventHeader result;
            _tape.Read<ACMIEventHeader>(positionOffset, out result);
            return result;
        }

        private ACMIEventHeader? Next(ACMIEventHeader? current)
        {
            Debug.Assert(_tape != null);
            if (current == null)
            {
                return null;
            }

            return GetGeneralEventData(current.Value.index + 1);
        }

        private ACMIEventHeader? Prev(ACMIEventHeader? current)
        {
            Debug.Assert(_tape != null);

            if (current == null)
            {
                return null;
            }

            return GetGeneralEventData(current.Value.index - 1);
        }

        private ACMIEventTrailer GeneralEventTrailer()
        {
            Debug.Assert(_tape != null);

            return _generalEventReadHeadTrailer;

        }

        private ACMIEventTrailer Next(ACMIEventTrailer current)
        {
#if TODO
             Debug.Assert(_tape != null);
             if (current == null || current == _lastEventTrailer)
            {
                return null;
            }


            return current + 1;
#endif
            throw new NotImplementedException();
        }

        private ACMIEventTrailer Prev(ACMIEventTrailer current)
        {
#if TODO
           // Debug.Assert(_tape != null);
             if (current == null || current == _firstEventTrailer)
            {
                return null;
            }

            return current - 1;
#endif
            throw new NotImplementedException();
        }

        private ACMIFeatEvent CurrFeatEvent()
        {
            // Debug.Assert(_tape != null);

            return _featEventReadHead;

        }

        private ACMIFeatEvent Next(ACMIFeatEvent current)
        {
            // Debug.Assert(_tape != null);
#if TODO
            if (current == null || current == _lastFeatEvent)
            {
                return null;
            }


            return current + 1;
#endif 
            throw new NotImplementedException();
        }
        private ACMIFeatEvent Prev(ACMIFeatEvent current)
        {
            // Debug.Assert(_tape != null);
#if TODO
            if (current == null || current == _firstFeatEvent)
            {
                return null;
            }

            return current - 1;
#endif 
            throw new NotImplementedException();
        }

        // Advance heads to current sim time.
        private void AdvanceEntityPositionHead(int index)
        {
            ACMIEntityPositionData? curr, next, prev;

            Debug.Assert(index >= 0 && index < NumEntities());

            // Backward.
            curr = CurrentEntityPositionHead(index);

            if (curr == null) return;

            while (_simTime < curr.Value.time)
            {
                prev = HeadPrev(curr);

                if (prev == null) return;

                // Advance the head.
                _entityReadHeads[index].positionDataOffset = curr.Value.prevPositionUpdateOffset;
                curr = prev;
            }

            // Forward.
            next = HeadNext(curr);

            if (next == null) return;

            while (_simTime >= next.Value.time)
            {
                // Advance the head.
                _entityReadHeads[index].positionDataOffset = curr.Value.nextPositionUpdateOffset;
                curr = next;

                next = HeadNext(curr);

                if (next == null) return;
            }
        }

        private void AdvanceEntityEventHead(int index)
        {
            ACMIEntityPositionData? curr, next, prev;
            SimTapeEntity e;

            Debug.Assert(index >= 0 && index < NumEntities());

            // get the entity if we need to change switch settings
            e = _simTapeEntities[index];

            // Backward.
            curr = CurrentEntityEventHead(index);

            if (curr == null) return;

            while (_simTime < curr.Value.time)
            {
                prev = HeadPrev(curr);

                if (prev == null) return;

                // handle switch settings
                if (curr.Value.type == PosType.PosTypeSwitch)
                {
                    ((DrawableBSP)e.objBase.drawPointer).SetSwitchMask(curr.Value.switchData.switchNum, curr.Value.switchData.prevSwitchVal);
                }
                else if (curr.Value.type == PosType.PosTypeDOF)
                {
                    ((DrawableBSP)e.objBase.drawPointer).SetDOFangle(curr.Value.dofData.DOFNum, curr.Value.dofData.prevDOFVal);
                }

                // Advance the head.
                _entityReadHeads[index].eventDataOffset = curr.Value.prevPositionUpdateOffset;
                curr = prev;
            }

            // Forward.
            next = HeadNext(curr);

            if (next == null) return;

            while (_simTime >= next.Value.time)
            {
                // Advance the head.
                _entityReadHeads[index].eventDataOffset = curr.Value.nextPositionUpdateOffset;

                // handle switch settings
                if (curr.Value.type == PosType.PosTypeSwitch)
                {
                    ((DrawableBSP)e.objBase.drawPointer).SetSwitchMask(curr.Value.switchData.switchNum, curr.Value.switchData.switchVal);
                }
                else if (curr.Value.type == PosType.PosTypeDOF)
                {
                    ((DrawableBSP)e.objBase.drawPointer).SetDOFangle(curr.Value.dofData.DOFNum, curr.Value.dofData.DOFVal);
                }

                curr = next;
                next = HeadNext(curr);

                if (next == null) return;
            }
        }

        private void AdvanceGeneralEventHead()
        {
            ACMIEventHeader? curr, next, prev;

            // Reverse.
            curr = GeneralEventData();

            if (curr == null) return;

            while (_simTime < curr.Value.time)
            {
                prev = Prev(curr);

                if (prev == null) return;

                if (_eventList[curr.Value.index] != null)
                    RemoveActiveEvent(ref _eventList[curr.Value.index]);


                // Advance the head.
                curr = prev;
                _generalEventReadHeadHeader = curr.Value.index;
            }

            // Forward.
            next = Next(curr);

            if (next == null) return;

            while (_simTime >= next.Value.time)
            {

                // Advance the head.
                curr = next;
                _generalEventReadHeadHeader = curr.Value.index;

                if (_eventList[curr.Value.index] == null)
                {
                    _eventList[curr.Value.index] = InsertActiveEvent(curr.Value, _simTime - curr.Value.time);
                }


                next = Next(curr);

                if (next == null) return;
            }
        }

        private void AdvanceGeneralEventHeadHeader()
        {
#if TODO
           ACMIEventHeader curr, next, prev;

            // Reverse.
            curr = GeneralEventData();

            if (curr == null) return;

            while (_simTime < curr.time)
            {
                prev = Prev(curr);

                if (prev == null) return;

                if (_eventList[curr.index])
                    RemoveActiveEvent(&_eventList[curr.index]);


                // Advance the head.
                curr = prev;
                _generalEventReadHeadHeader = curr.index;
            }

            // Forward.
            next = Next(curr);

            if (next == null) return;

            while (_simTime >= next.time)
            {

                // Advance the head.
                curr = next;
                _generalEventReadHeadHeader = curr.index;

                if (!_eventList[curr.index])
                {
                    _eventList[curr.index] = InsertActiveEvent(curr, _simTime - curr.time);
                }


                next = Next(curr);

                if (next == null) return;
            }
#endif
            throw new NotImplementedException();
        }

        private void AdvanceGeneralEventHeadTrailer()
        {
#if TODO
           ACMIEventHeader e;
            ACMIEventTrailer curr, next, prev;

            // Reverse.
            curr = GeneralEventTrailer();

            if (curr == null) return;

            while (_simTime < curr.timeEnd)
            {
                prev = Prev(curr);

                if (prev == null) return;

                if (!_eventList[curr.index])
                {
                    e = GetGeneralEventData(curr.index);
                    _eventList[curr.index] = InsertActiveEvent(e, _simTime - e.time);
                }


                // Advance the head.
                curr = prev;
                _generalEventReadHeadTrailer = curr;
            }

            // Forward.
            next = Next(curr);

            if (next == null) return;

            while (_simTime >= next.timeEnd)
            {

                // Advance the head.
                curr = next;
                _generalEventReadHeadTrailer = curr;

                if (_eventList[curr.index])
                    RemoveActiveEvent(&_eventList[curr.index]);


                next = Next(curr);

                if (next == null) return;
            }
#endif
            throw new NotImplementedException();
        }

        private void AdvanceFeatEventHead()
        {
#if TODO
            ACMIFeatEvent curr, next, prev;
            SimTapeEntity feat;

            // Reverse.
            curr = CurrFeatEvent();

            if (curr == null) return;

            while (_simTime < curr.time)
            {
                prev = Prev(curr);

                if (prev == null) return;

                // do stuff

                // sanity check that we have the right index
                if (curr.index >= 0)
                {
                    // get the feature entity
                    feat = &_simTapeFeatures[curr.index];

                    // create the new drawable object
                    // set new status
                    feat.objBase.ClearStatusBit((int)STATUS_ENUM.VIS_TYPE_MASK);
                    feat.objBase.SetStatusBit(curr.prevStatus);

                    // this function now handles inserts and removes from
                    // drawlist
                    CreateFeatureDrawable(feat);
                    Debug.Assert(feat.objBase.drawPointer != null);

                    if (curr.prevStatus == (int)VIS_TYPES.VIS_DAMAGED)
                    {
                        ((DrawableBSP)feat.objBase.drawPointer).SetTextureSet(1);
                    }

                    // remove old from display and delete
                    /*
                    if(feat.objBase.drawPointer.InDisplayList())
                    {
                     _viewPoint.RemoveObject(feat.objBase.drawPointer);
                    }
                    delete feat.objBase.drawPointer;
                    feat.objBase.drawPointer = null;

                    // set new status
                    feat.objBase.ClearStatusBit( VIS_TYPE_MASK );
                    feat.objBase.SetStatusBit( curr.prevStatus );

                    CreateDrawable ( feat.objBase, 1.0F);
                    Debug.Assert(feat.objBase.drawPointer != null);

                    // set damaged texture set if needed
                    if ( curr.prevStatus == VIS_DAMAGED )
                    {
                     ((DrawableBSP *)feat.objBase.drawPointer).SetTextureSet( 1 );
                    }

                    // features get put into draw list and positioned here.
                    _viewPoint.InsertObject( feat.objBase.drawPointer );
                    */
                }

                // Advance the head.
                curr = prev;
                _featEventReadHead = curr;
            }

            // Forward.
            next = Next(curr);

            if (next == null) return;

            if (F4IsBadReadPtr(curr, sizeof(ACMIFeatEvent)))
                return;

            while (_simTime >= next.time)
            {
                if (F4IsBadReadPtr(next, sizeof(ACMIFeatEvent)))
                    return;

                // Advance the head.
                curr = next;
                _featEventReadHead = curr;

                // do stuff

                // sanity check that we have the right index
                if (curr.index >= 0)
                {
                    // get the feature entity
                    feat = _simTapeFeatures[curr.index];

                    // create the new drawable object
                    // set new status
                    feat.objBase.ClearStatusBit((int)STATUS_ENUM.VIS_TYPE_MASK);
                    feat.objBase.SetStatusBit(curr.newStatus);

                    CreateFeatureDrawable(feat);
                    Debug.Assert(feat.objBase.drawPointer != null);

                    // set damaged texture set if needed
                    if (curr.newStatus == (int)VIS_TYPES.VIS_DAMAGED)
                    {
                        ((DrawableBSP)feat.objBase.drawPointer).SetTextureSet(1);
                    }

                    /*
                    // remove old from display and delete
                    if(feat.objBase.drawPointer.InDisplayList())
                    {
                     _viewPoint.RemoveObject(feat.objBase.drawPointer);
                    }
                    delete feat.objBase.drawPointer;
                    feat.objBase.drawPointer = null;

                    // set new status
                    feat.objBase.ClearStatusBit( VIS_TYPE_MASK );
                    feat.objBase.SetStatusBit( curr.newStatus );

                    CreateDrawable ( feat.objBase, 1.0F);
                    Debug.Assert(feat.objBase.drawPointer != null);

                    // set damaged texture set if needed
                    if ( curr.newStatus == VIS_DAMAGED )
                    {
                     ((DrawableBSP *)feat.objBase.drawPointer).SetTextureSet( 1 );
                    }

                    // features get put into draw list and positioned here.
                    _viewPoint.InsertObject( feat.objBase.drawPointer );
                    */
                }

                next = Next(curr);

                if (next == null) return;
            }
#endif
            throw new NotImplementedException();
        }

        private void AdvanceAllHeads()
        {
            // Advance all entity read heads.
            int numEntities = NumEntities();

            for (int i = 0; i < numEntities; i++)
            {
                // Advance entity position head.
                AdvanceEntityPositionHead(i);
                AdvanceEntityEventHead(i);
            }

            // Advance general event head, and apply events that are encountered.
            AdvanceGeneralEventHeadHeader();
            AdvanceGeneralEventHeadTrailer();

            // advance head for any feature events
            AdvanceFeatEventHead();
        }

        // Entity setup and cleanup
        private void SetupSimTapeEntities()
        {
            int i, numEntities;
            Tpoint pos;
            ACMIEntityData e;
            ACMIEntityPositionData p;


            Tpoint origin = new Tpoint() { x = 0.0f, y = 0.0f, z = 0.0f };

            Debug.Assert(_simTapeEntities == null);
            Debug.Assert(_tape != null);

            // create array of SimTapeEntity
            numEntities = NumEntities();
            _simTapeEntities = new SimTapeEntity[numEntities];
            Debug.Assert(_simTapeEntities != null);
#if TODO
            // for each entity, create it's object stuff....
            for (i = 0; i < numEntities; i++)
            {

                // get the tape entity data
                e = EntityData(i);

                // get the 1st position data for the entity
                p = CurrentEntityPositionHead(i);

                _simTapeEntities[i].flags = e.flags;

                if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_FEATURE))
                {
                    // edg: I believe this code path should no longer be in use
                    // since features are in a different list
                    _simTapeEntities[i].objBase = new SimFeatureClass(EntityType(i));
                    _simTapeEntities[i].objBase.SetDelta(0.0f, 0.0f, 0.0f);
                    _simTapeEntities[i].objBase.SetYPRDelta(0.0f, 0.0f, 0.0f);
                }
                else
                {
                    // create the base class
                    _simTapeEntities[i].objBase = new SimStaticClass(EntityType(i));// new SimBaseClass(EntityType(i));
                    GetEntityPosition(i,
                                     out _simTapeEntities[i].x,
                                     out _simTapeEntities[i].y,
                                     out _simTapeEntities[i].z,
                                     out _simTapeEntities[i].yaw,
                                     out _simTapeEntities[i].pitch,
                                     out _simTapeEntities[i].roll,
                                     out _simTapeEntities[i].aveSpeed,
                                     out _simTapeEntities[i].aveTurnRate,
                                     out _simTapeEntities[i].aveTurnRadius);
                }

                // set the matrix
                _simTapeEntities[i].objBase.SetYPR(p.posData.yaw, p.posData.pitch, p.posData.roll);
                _simTapeEntities[i].objBase.SetPosition(p.posData.x, p.posData.y, p.posData.z);
                CalcTransformMatrix(_simTapeEntities[i].objBase);

                // create thge drawable object
                // special case chaff and flares
                if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_CHAFF))
                {
                    pos.x = p.posData.x;
                    pos.y = p.posData.y;
                    pos.z = p.posData.z;
                    _simTapeEntities[i].objBase.drawPointer = new DrawableBSP(EntityDB.MapVisId(Vis_Types.VIS_CHAFF), &pos, &IMatrix, 1.0f);
                    ((DrawableBSP)_simTapeEntities[i].objBase.drawPointer).SetLabel("", 0);
                }
                else if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_FLARE))
                {

                    pos.x = p.posData.x;
                    pos.y = p.posData.y;
                    pos.z = p.posData.z;
                    _simTapeEntities[i].objBase.drawPointer = new Drawable2D(DRAW2D_FLARE, 2.0f, pos);
                }
                // aircraft use special Drawable Poled class
                else if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_AIRCRAFT))
                {
                    short visType;

                    SimBaseClass theObject = _simTapeEntities[i].objBase;
                    // get the class pointer
                    FalconNet.FalcLib.Falcon4EntityClassType classPtr = (FalconNet.FalcLib.Falcon4EntityClassType)theObject.EntityType();
                    pos.x = p.posData.x;
                    pos.y = p.posData.y;
                    pos.z = p.posData.z;
                    visType = classPtr.visType[theObject.Status() & (int)STATUS_ENUM.VIS_TYPE_MASK];
                    theObject.drawPointer = new DrawablePoled(visType, &pos, &IMatrix, 1.0f);

                    Debug.Assert(theObject.drawPointer != null);

                    if (ACMI_Callsigns != null) // we have callsigns
                        theObject.drawPointer.SetLabel(ACMI_Callsigns[e.uniqueID].label, TeamSimColorList[ACMI_Callsigns[e.uniqueID].teamColor]);
                }
                else
                {
                    CreateDrawable(_simTapeEntities[i].objBase, 1.0F);

                    if (ACMI_Callsigns != null)
                        ((DrawableBSP)_simTapeEntities[i].objBase.drawPointer).SetLabel(((DrawableBSP)_simTapeEntities[i].objBase.drawPointer).Label(), TeamSimColorList[ACMI_Callsigns[e.uniqueID].teamColor]);
                }


                // sigh.  hack for ejections
                if (_simTapeEntities[i].objBase.drawPointer == null)
                {
                    short visType;

                    SimBaseClass theObject = _simTapeEntities[i].objBase;
                    // get the class pointer
                    FalconNet.FalcLib.Falcon4EntityClassType classPtr = (FalconNet.FalcLib.Falcon4EntityClassType)theObject.EntityType();

                    if (classPtr.vuClassData.classInfo_[(int)VuClassHierarchy.VU_TYPE] == (byte)ClassTypes.TYPE_EJECT)
                    {
                        pos.x = p.posData.x;
                        pos.y = p.posData.y;
                        pos.z = p.posData.z;
                        visType = classPtr.visType[theObject.Status() & (int)STATUS_ENUM.VIS_TYPE_MASK];
                        theObject.drawPointer = new DrawableBSP(EntityDB.MapVisId(Vis_Types.VIS_EJECT1), &pos, &IMatrix, 1.0f);
                        ((DrawableBSP)theObject.drawPointer).SetLabel("Ejected Pilot", 0x0000FF00);
                    }

                    Debug.Assert(theObject.drawPointer != null);
                }

                // features get put into draw list and positioned here.
                if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_FEATURE))
                {
                    // edg: I believe this code path should no longer be in use
                    // since features are in a different list
                    _viewPoint.InsertObject(_simTapeEntities[i].objBase.drawPointer);
                }


                // create other stuff as needed by the object
                // (ie, if this is missile, create the drawable trail for it...)
                _simTapeEntities[i].objTrail = null;
                _simTapeEntities[i].objBsp1 = null;
                _simTapeEntities[i].objBsp2 = null;
                _simTapeEntities[i].obj2d = null;
                _simTapeEntities[i].wlTrail = null;
                _simTapeEntities[i].wrTrail = null;
                _simTapeEntities[i].wtLength = 0;


                // a missile -- it needs drawable trail set up
                if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_MISSILE))
                {
                    _simTapeEntities[i].objTrail = new DrawableTrail(TrailType.TRAIL_SAM);
                    _simTapeEntities[i].objBsp1 = new DrawableBSP(EntityDB.MapVisId(Vis_Types.VIS_MFLAME_L), origin, (Trotation) & IMatrix, 1.0f);
                    _simTapeEntities[i].objTrail.KeepStaleSegs(true);
                    _simTapeEntities[i].trailStartTime = p.time;
                    _simTapeEntities[i].trailEndTime = p.time + 120.0F; //me123 to 30 we wanna see the trials in acmi// trail lasts 3 sec
                }
                // a flare -- it needs drawable trail set up and a glow sphere
                else if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_FLARE))
                {
                    _simTapeEntities[i].objTrail = new DrawableTrail(TrailType.TRAIL_SAM);
                    _simTapeEntities[i].objTrail.KeepStaleSegs(true);
                    _simTapeEntities[i].trailStartTime = p.time;
                    _simTapeEntities[i].trailEndTime = p.time + 3.0F; // trail lasts 3 sec
                    _simTapeEntities[i].obj2d =
                        new Drawable2D(DRAW2D_EXPLCIRC_GLOW, 8.0f, &origin);
                }

                // a aircraft -- it needs drawable wing trails set up
                else if (e.flags.HasFlag(EntityFlag.ENTITY_FLAG_AIRCRAFT))
                {
                    _simTapeEntities[i].wlTrail = new DrawableTrail(TrailType.TRAIL_LWING);
                    _simTapeEntities[i].wlTrail.KeepStaleSegs(true);   // MLR 12/14/2003 -
                    _simTapeEntities[i].wrTrail = new DrawableTrail(TrailType.TRAIL_RWING);
                    _simTapeEntities[i].wrTrail.KeepStaleSegs(true);   // MLR 12/14/2003 -
                                                                       //_simTapeEntities[i].wlTrail = new DrawableTrail(TRAIL_AIM120);
                                                                       //_simTapeEntities[i].wrTrail = new DrawableTrail(TRAIL_MAVERICK);

                }

            }

            Debug.Assert(_simTapeFeatures == null);


            // create array of SimTapeEntity
            if (_tapeHdr.numFeat == 0)
                return;

            _simTapeFeatures = new SimTapeEntity[_tapeHdr.numFeat];
            Debug.Assert(_simTapeFeatures != null);

            // for each feature, create it's object stuff....
            for (i = 0; i < _tapeHdr.numFeat; i++)
            {
                // get the tape entity data
                e = FeatureData(i);

                // get the 1st position data for the entity
                p = CurrentFeaturePositionHead(i);

                _simTapeFeatures[i].flags = e.flags;

                // protect against bad indices(!?)
                // edg: I'm not sure what's going on but it seems like we
                // occasionally get a bad lead VU ID.  The imported can't find
                // it.  This is protected for in Import.  Also protect here...
                if (e.leadIndex >= _tapeHdr.numFeat)
                    _simTapeFeatures[i].leadIndex = -1;
                else
                    _simTapeFeatures[i].leadIndex = e.leadIndex;

                _simTapeFeatures[i].slot = e.slot;

                _simTapeFeatures[i].objBase = new SimFeatureClass((ushort)e.type);
                Debug.Assert(_simTapeFeatures[i].objBase != null);
                _simTapeFeatures[i].objBase.SetDelta(0.0f, 0.0f, 0.0f);
                _simTapeFeatures[i].objBase.SetYPRDelta(0.0f, 0.0f, 0.0f);
                _simTapeFeatures[i].objBase.SetYPR(p.posData.yaw, p.posData.pitch, p.posData.roll);
                _simTapeFeatures[i].objBase.SetPosition(p.posData.x, p.posData.y, p.posData.z);
                ((SimFeatureClass)_simTapeFeatures[i].objBase).featureFlags = e.specialFlags;

                // get the class pointer
                FalconNet.FalcLib.Falcon4EntityClassType classPtr = (FalconNet.FalcLib.Falcon4EntityClassType)_simTapeFeatures[i].objBase.EntityType();
                // get the feature class data
                FeatureClassDataType fc = (FeatureClassDataType)classPtr.dataPtr;
                _simTapeFeatures[i].objBase.SetCampaignFlag(fc.Flags);
                CalcTransformMatrix(_simTapeFeatures[i].objBase);



                // create other stuff as needed by the object
                // (ie, if this is missile, create the drawable trail for it...)
                _simTapeFeatures[i].objTrail = null;
                _simTapeFeatures[i].objBsp1 = null;
                _simTapeFeatures[i].objBsp2 = null;
                _simTapeFeatures[i].obj2d = null;

            }

            // for each feature, create it's drawable object
            // we do this in a different loop because we want to make sure all the
            // objbase's are created first since some feature objects are actually
            // subcomponents of others and have thier drawpointer inserted into
            // the drawpointer list of another rather than the main drawlist
            for (i = 0; i < _tapeHdr.numFeat; i++)
            {
                // create the drawable object
                CreateFeatureDrawable(_simTapeFeatures[i]);
                Debug.Assert(_simTapeFeatures[i].objBase.drawPointer != null);

                // NOTE: insertion into draw list should now be done in
                // CreateFeatureDrawable
                // features get put into draw list and positioned here.
                // _viewPoint.InsertObject( _simTapeFeatures[i].objBase.drawPointer );
            }
#endif
        }

        private void CleanupSimTapeEntities()
        {
#if TODO
            int
            i,
            numEntities;
            SimTapeEntity ep;

            Debug.Assert(_simTapeEntities != null);

            // create array of SimTapeEntity
            numEntities = NumEntities();

            // for each entity, create it's object stuff....
            for (i = 0; i < numEntities; i++)
            {
                // get pointer
                ep = _simTapeEntities[i];

                // if there's a base object...
                if (ep.objBase != null)
                {
                    // remove from display
                    if (ep.objBase.drawPointer.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBase.drawPointer);
                    }

                    //delete ep.objBase.drawPointer;
                    ep.objBase.drawPointer = null;
                    //delete ep.objBase;
                    ep.objBase = null;
                }

                // if there's a trail object...
                if (ep.objTrail != null)
                {
                    // remove from display
                    if (ep.objTrail.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objTrail);
                    }

                    //delete ep.objTrail;
                    ep.objTrail = null;
                }

                // if there's a 2d object...
                if (ep.obj2d != null)
                {
                    // remove from display
                    if (ep.obj2d.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.obj2d);
                    }

                    //delete ep.obj2d;
                    ep.obj2d = null;
                }

                // if there's a bsp object...
                if (ep.objBsp1 != null)
                {
                    // remove from display
                    if (ep.objBsp1.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBsp1);
                    }

                    //delete ep.objBsp1;
                    ep.objBsp1 = null;
                }

                // if there's a bsp object...
                if (ep.objBsp2 != null)
                {
                    // remove from display
                    if (ep.objBsp2.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBsp2);
                    }

                    //delete ep.objBsp2;
                    ep.objBsp2 = null;
                }

                // if there's a trail object...
                if (ep.wlTrail != null)
                {
                    // remove from display
                    if (ep.wlTrail.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.wlTrail);
                    }

                    //delete ep.wlTrail;
                    ep.wlTrail = null;
                }

                // if there's a trail object...
                if (ep.wrTrail != null)
                {
                    // remove from display
                    if (ep.wrTrail.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.wrTrail);
                    }

                    //delete ep.wrTrail;
                    ep.wrTrail = null;
                }
            }

            //delete[] _simTapeEntities;
            _simTapeEntities = null;

            // for each feature, remove its object stuff
            // 1st pass do drawpointer
            for (i = 0; i < _tapeHdr.numFeat; i++)
            {
                // get pointer
                ep = _simTapeFeatures[i];

                // if there's a base object...
                if (ep.objBase != null)
                {
                    // remove from display
                    if (ep.objBase.drawPointer.InDisplayList())
                    {
                        _viewPoint.RemoveObject(ep.objBase.drawPointer);
                    }

                    //delete ep.objBase.drawPointer;
                    ep.objBase.drawPointer = null;
                }

            }

            // 2nd pass delete baseObj pointer and objBase
            for (i = 0; i < _tapeHdr.numFeat; i++)
            {
                // get pointer
                ep = _simTapeFeatures[i];

                // if there's a base object...
                if (ep.objBase != null)
                {
                    // remove from display
                    if (((SimFeatureClass)ep.objBase).baseObject)
                    {
                        if (((SimFeatureClass)ep.objBase).baseObject.InDisplayList())
                        {
                            _viewPoint.RemoveObject(((SimFeatureClass)ep.objBase).baseObject);
                        }

                        //delete((SimFeatureClass)ep.objBase).baseObject;
                        ((SimFeatureClass)ep.objBase).baseObject = null;
                    }

                    //delete ep.objBase;
                    ep.objBase = null;
                }

            }

            //delete[] _simTapeFeatures;
            _simTapeFeatures = null;
#endif
            throw new NotImplementedException();
        }

        // open the tape file and setup memory mapping
        private long OpenTapeFile(string fname)
        {
            long length = 0;
            try
            {
                // 1st get the header info and check it out
                using (Stream fd = new FileStream(fname, FileMode.Open, FileAccess.Read))
                {

                    if (fd == null)
                    {
                    }

                    length = fd.Length;

                    // read in the tape header
                    if (!StructEncoding<ACMITapeHeader>.Decode(fd, out _tapeHdr))
                    {
                        log.Error("Unable to to read tape header.");
                        fd.Close();
                        return (0);
                    }

                    // close the file
                    fd.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to Open Tape File:", ex);
                return (0);
            }

            // check that we've got a valid file
            if (_tapeHdr.fileID != ACMITapeHeader.FILEID) // 'TAPE')
            {
                log.Error("Invalid Tape File.");
                return (0);
            }

            // Set up memory mapping
            // system info for tape file access
            MemoryMappedFile _tapeFileHandle;
            //MemoryMappedViewAccessor _tapeMapHandle;

            // open the tape file
            try
            {
                _tapeFileHandle = MemoryMappedFile.CreateFromFile(fname, FileMode.Open);
            }
            catch (Exception ex)
            {
                log.Debug("CreateFile failed on tape open.", ex);
                return (0);
            }


            // create map view of file mapping
            try
            {
                _tape = _tapeFileHandle.CreateViewAccessor();
            }
            catch (Exception ex)
            {
                log.Debug("MapViewOfFile failed on tape open", ex);
                _tapeFileHandle.Dispose();
                return (0);
            }

            // hunky dory
            return (length);
        }

        private void CloseTapeFile()
        {
#if TODO
            UnmapViewOfFile(_tape);
            CloseHandle(_tapeMapHandle);
            CloseHandle(_tapeFileHandle);
            _tape = null;
            return;
#endif
            throw new NotImplementedException();
        }


        // event list related functions
        private void CleanupEventList()
        {
#if TODO
            int i;

            Debug.Assert(_eventList != null);

            for (i = 0; i < _tapeHdr.numEvents; i++)
            {
                if (_eventList[i] != null)
                {
                    RemoveActiveEvent(ref _eventList[i]);
                }
            }

            //delete[] _eventList;
            _eventList = null;
#endif
            throw new NotImplementedException();
        }

        private ActiveEvent InsertActiveEvent(ACMIEventHeader eh, float dT)
        {
#if TODO
           ActiveEvent evnt = null;
            TracerEventData td = null;
            SfxClass sfx = null;
            SimBaseClass simBase;
            Tpoint pos;
            Tpoint vec;

            // don't insert if passed end time
            if (eh.time + dT > eh.timeEnd)
                return null;

            // creation based on type
            switch (eh.eventType)
            {
                case RecordTypes.ACMIRecTracerStart:
                    // create new event record
                    evnt = new ActiveEvent();
                    Debug.Assert(evnt != null);
                    evnt.eventType = eh.eventType;
                    evnt.index = eh.index;
                    evnt.time = eh.time;
                    evnt.timeEnd = eh.timeEnd;

                    // create new tracer event record
                    td = new TracerEventData();
                    Debug.Assert(td != null);
                    evnt.eventData = td;

                    // init tracer data
                    td.x = eh.x;
                    td.y = eh.y;
                    td.z = eh.z;
                    td.dx = eh.dx;
                    td.dy = eh.dy;
                    td.dz = eh.dz;
                    // create tracer
                    td.objTracer = new DrawableTracer(1.3f);
                    td.objTracer.SetAlpha(0.8f);
                    td.objTracer.SetRGB(1.0f, 1.0f, 0.2f);

                    UpdateTracerEvent(td, dT);

                    // put it into the draw list
                    _viewPoint.InsertObject(td.objTracer);

                    break;

                case RecordTypes.ACMIRecStationarySfx:
                    // create new event record
                    evnt = new ActiveEvent();
                    Debug.Assert(evnt);
                    evnt.eventType = eh.eventType;
                    evnt.index = eh.index;
                    evnt.time = eh.time;
                    evnt.timeEnd = eh.timeEnd;

                    pos.x = eh.x;
                    pos.y = eh.y;
                    pos.z = eh.z;

                    // create new tracer event record
                    sfx = new SfxClass(eh.type,
                                           &pos,
                                           (float)(eh.timeEnd - eh.time),
                                           eh.scale);

                    Debug.Assert(sfx);
                    evnt.eventData = sfx;

                    sfx.ACMIStart(_viewPoint, evnt.time, _simTime);

                    break;

                case RecordTypes.ACMIRecMovingSfx:
                    // create new event record
                    evnt = new ActiveEvent();
                    Debug.Assert(evnt != null);
                    evnt.eventType = eh.eventType;
                    evnt.index = eh.index;
                    evnt.time = eh.time;
                    evnt.timeEnd = eh.timeEnd;

                    pos.x = eh.x;
                    pos.y = eh.y;
                    pos.z = eh.z;
                    vec.x = eh.dx;
                    vec.y = eh.dy;
                    vec.z = eh.dz;

                    // create new sfx
                    if (eh.user < 0)
                    {
                        sfx = new SfxClass(eh.type,
                                           eh.flags,
                                           &pos,
                                           &vec,
                                           (float)(eh.timeEnd - eh.time),
                                           eh.scale);
                    }
                    else
                    {
                        // we need to build a base obj first
                        simBase = new SimStaticClass(0);// SimBaseClass( 0 );
                        simBase.drawPointer = new DrawableBSP(eh.user, &pos, &IMatrix, 1.0f);
                        simBase.SetPosition(pos.x, pos.y, pos.z);
                        simBase.SetDelta(vec.x, vec.y, vec.z);
                        simBase.SetYPR(0.0f, 0.0f, 0.0f);
                        simBase.SetYPRDelta(0.0f, 0.0f, 0.0f);
                        sfx = new SfxClass(eh.type, eh.flags, simBase, (float)(eh.timeEnd - eh.time), eh.scale);
                    }

                    Debug.Assert(sfx);
                    evnt.eventData = sfx;

                    sfx.ACMIStart(_viewPoint, evnt.time, _simTime);

                    break;

                // current don't handle anything else
                default:
                    return null;
            }

            // now insert it into the active list
            evnt.prev = null;

            if (_activeEventHead != null)
            {
                _activeEventHead.prev = evnt;
            }

            evnt.next = _activeEventHead;
            _activeEventHead = evnt;

            return evnt;
#endif
            throw new NotImplementedException();
        }

        private void RemoveActiveEvent(ref ActiveEvent evnt)
        {
#if TODO
           //ActiveEvent  evnt =  eptrptr;
            TracerEventData td = null;
            SfxClass sfx;

            // deletion based on type
            switch (evnt.eventType)
            {
                case RecordTypes.ACMIRecTracerStart:

                    // cast eventData to appropriate type
                    td = (TracerEventData)evnt.eventData;

                    // remove from draw list
                    if (td.objTracer.InDisplayList())
                        _viewPoint.RemoveObject(td.objTracer);

                    // free data memory
                    //delete td.objTracer;
                    //delete td;

                    break;

                case RecordTypes.ACMIRecMovingSfx:
                case RecordTypes.ACMIRecStationarySfx:

                    // cast eventData to appropriate type
                    sfx = (SfxClass)evnt.eventData;

                    // free data memory
                    //delete sfx;

                    break;

                // current don't handle anything else
                default:
                    return;
            }

            // take event out of active Event List
            if (evnt.prev != null)
                evnt.prev.next = evnt.next;
            else
                _activeEventHead = evnt.next;

            if (evnt.next != null)
                evnt.next.prev = evnt.prev;

            // delete event data and set caller's pointer to null
            //delete event;
            *eptrptr = null;
#endif
            throw new NotImplementedException();
        }

        private void UpdateActiveEvents()
        {
            ActiveEvent evnt = null;
            TracerEventData td = null;
            SfxClass sfx;

            evnt = _activeEventHead;

            while (evnt != null)
            {

                // handle based on type
                switch (evnt.eventType)
                {
                    case RecordTypes.ACMIRecTracerStart:

                        // deref eventData
                        td = (TracerEventData)evnt.eventData;
                        UpdateTracerEvent(td, _simTime - evnt.time);

                        // remove from display list if event no longer exists
                        // blech, this is a very less than optimal solution
                        // the active event list is going to bloat over time
                        /*
                        if ( _simTime > event.timeEnd || event.time > _simTime )
                        {
                         if ( td.objTracer.InDisplayList() )
                         _viewPoint.RemoveObject( td.objTracer );
                        }
                        else
                        {
                         // the event is active....
                         if ( !td.objTracer.InDisplayList() )
                         _viewPoint.InsertObject( td.objTracer );
                         UpdateTracerEvent( td, _simTime - event.time );
                        }
                        */
                        break;

                    case RecordTypes.ACMIRecMovingSfx:
                    case RecordTypes.ACMIRecStationarySfx:

                        // deref eventData
                        sfx = (SfxClass)evnt.eventData;
                        sfx.ACMIExec(_simTime);

                        break;

                    // currently don't handle anything else
                    default:
                        break;
                }

                evnt = evnt.next;
            }
        }

        // create/update feature drawables
        private void CreateFeatureDrawable(SimTapeEntity feat)
        {
#if TODO
           short visType = -1;
            Tpoint simView;
            Trotation viewRotation;
            SimBaseClass baseObject;
            DrawableObject lastPointer = null;


            // get the object and pointer to its classtbl entry
            SimBaseClass theObject = feat.objBase;
            Falcon4EntityClassType classPtr = (Falcon4EntityClassType)theObject.EntityType();

            // Set position and orientations
            viewRotation.M11 = theObject.dmx[0][0];
            viewRotation.M21 = theObject.dmx[0][1];
            viewRotation.M31 = theObject.dmx[0][2];

            viewRotation.M12 = theObject.dmx[1][0];
            viewRotation.M22 = theObject.dmx[1][1];
            viewRotation.M32 = theObject.dmx[1][2];

            viewRotation.M13 = theObject.dmx[2][0];
            viewRotation.M23 = theObject.dmx[2][1];
            viewRotation.M33 = theObject.dmx[2][2];

            // Update object position
            simView.x = theObject.XPos();
            simView.y = theObject.YPos();
            simView.z = theObject.ZPos();

            visType = classPtr.visType[theObject.Status() & (int)STATUS_ENUM.VIS_TYPE_MASK];

            // make sure things are sane
            Debug.Assert(visType >= 0 || theObject.drawPointer != null);
            Debug.Assert(classPtr.vuClassData.classInfo_[(int)VuClassHierarchy.VU_DOMAIN] == DOMAIN_LAND);
            Debug.Assert(classPtr.vuClassData.classInfo_[(int)VuClassHierarchy.VU_CLASS] == CLASS_FEATURE);

            // A feature thingy..
            SimBaseClass prevObj = null, nextObj = null;

            // In many cases, our visType should be modified by our neighbors.
            if ((theObject.Status() & (int)STATUS_ENUM.VIS_TYPE_MASK) != (int)VIS_TYPES.VIS_DESTROYED &&
                (((SimFeatureClass)theObject).featureFlags & FEAT_NEXT_NORM ||
                 ((SimFeatureClass)theObject).featureFlags & FEAT_PREV_NORM))
            {
                int idx = feat.slot;

                prevObj = FindComponentFeature(feat.leadIndex, idx - 1);
                nextObj = FindComponentFeature(feat.leadIndex, idx + 1);

                if (prevObj &&
                    (((SimFeatureClass)theObject).featureFlags & FEAT_PREV_NORM) &&
                    (prevObj.Status() & (int)STATUS_ENUM.VIS_TYPE_MASK) == (int)VIS_TYPES.VIS_DESTROYED)
                {
                    if (nextObj &&
                        (((SimFeatureClass)theObject).featureFlags & FEAT_NEXT_NORM) &&
                        (nextObj.Status() & (int)STATUS_ENUM.VIS_TYPE_MASK) == (int)VIS_TYPES.VIS_DESTROYED)
                    {
                        visType = classPtr.visType[VIS_BOTH_DEST];
                    }
                    else
                    {
                        visType = classPtr.visType[VIS_LEFT_DEST];
                    }
                }
                else if (nextObj &&
                         (((SimFeatureClass)theObject).featureFlags & FEAT_NEXT_NORM) &&
                         (nextObj.Status() & (int)STATUS_ENUM.VIS_TYPE_MASK) == (int)VIS_TYPES.VIS_DESTROYED)
                {
                    visType = classPtr.visType[VIS_RIGHT_DEST];
                }
            }

            // Check for change - and don't bother if there is none.
            if (theObject.drawPointer &&
                ((DrawableBSP)theObject.drawPointer).GetID() == visType)
                return;

            if (theObject.drawPointer && theObject.drawPointer.InDisplayList())
            {
                // KCK: In some cases we still need this pointer (specifically
                // when we replace bridge segments), so let's save it here - we'll
                // toss it out after we're done.
                lastPointer = theObject.drawPointer;
                theObject.drawPointer = null;
            }

            // get the lead baseobject if any
            // otherwise set base object to ourself
            if (feat.leadIndex >= 0)
                baseObject = _simTapeFeatures[feat.leadIndex].objBase;
            else
                baseObject = theObject;

            // Some things require Base Objects (like bridges and airbases)
            if (!((SimFeatureClass)baseObject).baseObject)
            {
                // Is this a bridge?
                if (theObject.IsSetCampaignFlag(FEAT_ELEV_CONTAINER))
                {
                    // baseObject is the "container" object for all parts of the bridge
                    // There is only one container for the entire bridge, stored in the lead element
                    ((SimFeatureClass)baseObject).baseObject = new DrawableBridge(1.0F);

                    // Insert only the bridge drawable.
                    _viewPoint.InsertObject(((SimFeatureClass)baseObject).baseObject);
                }
                // Is this a big flat thing with things on it (like an airbase?)
                else if (theObject.IsSetCampaignFlag(FEAT_FLAT_CONTAINER))
                {
                    // baseObject is the "container" object for all parts of the platform
                    // There is only one container for the entire platform, stored in the
                    // lead element.
                    ((SimFeatureClass)baseObject).baseObject = new DrawablePlatform(1.0F);

                    // Insert only the platform drawable.
                    _viewPoint.InsertObject(((SimFeatureClass)baseObject).baseObject);
                }
            }

            // Add another building to this grouping of buildings, or replace the drawable
            // of one which is here.
            // Is the container a bridge?
            if (baseObject.IsSetCampaignFlag(FEAT_ELEV_CONTAINER))
            {
                // Make the new BRIDGE object
                if (visType != 0)
                {
                    if (theObject.IsSetCampaignFlag(FEAT_NEXT_IS_TOP) && theObject.Status() != (int)VIS_TYPES.VIS_DESTROYED)
                        theObject.drawPointer = new DrawableRoadbed(visType, visType + 1, &simView, theObject.Yaw(), 10.0f, (float)Math.Atan(20.0f / 280.0f));
                    else
                        theObject.drawPointer = new DrawableRoadbed(visType, -1, &simView, theObject.Yaw(), 10.0f, (float)Math.Atan(20.0f / 280.0f));
                }
                else
                    theObject.drawPointer = null;

                // Check for replacement
                if (lastPointer)
                {
                    Debug.Assert(lastPointer.GetClass() == DrawableObject.Roadbed);
                    Debug.Assert(theObject.drawPointer.GetClass() == DrawableObject.Roadbed);
                    ((DrawableBridge)(((SimFeatureClass)baseObject).baseObject)).ReplacePiece((DrawableRoadbed)(lastPointer), (DrawableRoadbed*)(theObject.drawPointer));
                }
                else if (theObject.drawPointer)
                {
                    Debug.Assert(theObject.drawPointer.GetClass() == DrawableObject.Roadbed);
                    ((DrawableBridge)(((SimFeatureClass)baseObject).baseObject)).AddSegment((DrawableRoadbed)(theObject.drawPointer));
                }
            }
            // Is the container a big flat thing (airbase)?
            else if (baseObject.IsSetCampaignFlag(FEAT_FLAT_CONTAINER))
            {
                // Everything on a platform is a Building
                // That means it sticks straight up the -Z axis
                theObject.drawPointer = new DrawableBuilding(visType, &simView, theObject.Yaw(), 1.0F);

                // Am I Flat (can things drive across it)?
                if (theObject.IsSetCampaignFlag((FEAT_FLAT_CONTAINER | FEAT_ELEV_CONTAINER)))
                    ((DrawablePlatform)((SimFeatureClass)baseObject).baseObject).InsertStaticSurface(((DrawableBuilding)theObject.drawPointer));
                else
                    ((DrawablePlatform)((SimFeatureClass)baseObject).baseObject).InsertStaticObject(theObject.drawPointer);
            }
            else
            {
                // if we get here then this is just a loose collection of buildings, like a
                // village or city, with no big flat objects between them
                theObject.drawPointer = new DrawableBuilding(visType, &simView, theObject.Yaw(), 1.0F);

                // Insert the object
                _viewPoint.InsertObject(((SimFeatureClass)theObject).drawPointer);
            }

            // KCK: Remove any previous drawable object
            if (lastPointer)
            {
                if (lastPointer.InDisplayList())
                    _viewPoint.RemoveObject(lastPointer);

                //delete lastPointer;
            }
#endif
            throw new NotImplementedException();
        }

        private SimBaseClass FindComponentFeature(long leadIndex, int slot)
        {
#if TODO
           int i;

            if (leadIndex < 0 || slot < 0)
                return null;

            for (i = 0; i < _tapeHdr.numFeat; i++)
            {
                if (_simTapeFeatures[i].leadIndex == leadIndex &&
                    _simTapeFeatures[i].slot == slot)
                {
                    return _simTapeFeatures[i].objBase;
                }
            }

#endif
            throw new NotImplementedException();
            return null;
        }

        // update tracer data
        private void UpdateTracerEvent(TracerEventData td, float dT)
        {
#if TODO
            Tpoint pos, end;

            pos.x = td.x + td.dx * dT;
            pos.y = td.y + td.dy * dT;
            pos.z = td.z + td.dz * dT;

            end.x = pos.x - td.dx * 0.05f;
            end.y = pos.y - td.dy * 0.05f;
            end.z = pos.z - td.dz * 0.05f;

            td.objTracer.Update(&pos, &end);
#endif
            throw new NotImplementedException();
        }

        // tape header
        private ACMITapeHeader _tapeHdr;

        private bool _screenCapturing;


        // sim time / real time
        private float _playVelocity;
        private float _playAcceleration;
        private float _maxPlaySpeed;

        // Current sim time
        private float _simTime;
        private float _stepTrail;
        private float _deltaSimTime;

        // list of sim entities from the tape that are manipulated and drawn
        private SimTapeEntity[] _simTapeFeatures;

        // viewpoint and renderer objs from acmiview
        private RViewPoint _viewPoint;
        private RenderOTW _renderer;

        // Current real time
        private float _lastRealTime;

        private bool _simulateOnly;
        private bool _paused;
        private bool _unpause;

        private bool _wingTrails;
        private int _wtMaxLength;

        // Base memory address of the file mapping
        // for the tape data.
        private MemoryMappedViewAccessor _tape;
        private ACMIEntityReadHead[] _entityReadHeads;
        private int _generalEventReadHeadHeader;
        private ACMIEventTrailer _generalEventReadHeadTrailer;
        private ACMIGeneralEventCallback _generalEventCallbacks;
        private ACMIFeatEvent _featEventReadHead;

        // events
        private ActiveEvent[] _eventList;
        private ActiveEvent _activeEventHead;
        private ACMIEventTrailer _firstEventTrailer;
        private ACMIEventTrailer _lastEventTrailer;
        private ACMIFeatEvent _firstFeatEvent;
        private ACMIFeatEvent _lastFeatEvent;

        // for scaling objects
        private float _tapeObjScale;

        private static long tempTarget; // for missile lock.


        //////////////////////////////////////////////////////////////////////////


        //TODO private static void CalcTransformMatrix(SimBaseClass theObject);
        //TODO private static void CreateDrawable(SimBaseClass theObject, float objectScale);

        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////

        // these are for raw data import
        private static List importEntityList;
        private static List importFeatList;
        private static List importPosList;
        private static List importEventList;
        private static List importEntEventList;
        private static List importEntityListEnd;
        private static List importFeatListEnd;
        private static List importPosListEnd;
        private static List importEventListEnd;
        private static List importEntEventListEnd;
        private static List importFeatEventList;
        private static List importFeatEventListEnd;
        private static int importNumPos;
        private static int importNumEnt;
        private static int importNumFeat;
        private static int importNumFeatEvents;
        private static int importNumEvents;
        private static int importNumEntEvents;
        private static int importEntOffset;
        private static int importFeatOffset;
        private static int importFeatEventOffset;
        private static int importPosOffset;
        private static int importEventOffset;
        private static int importEntEventOffset;
        private static ACMIEventTrailer[] importEventTrailerList;

        //TODO private static extern long[] TeamSimColorList = new long[NUM_TEAMS];

        private static List AppendToEndOfList(List list, ref List end, object node)
        {
            List newnode;

            newnode = new List();

            newnode.node = node;
            newnode.next = null;

            /* list was null */
            if (list == null)
            {
                list = newnode;
                end = list;
            }
            else
            {
                /* chain in at end */
                end.next = newnode;
                end = newnode;
            }

            return (list);
        }

        private static void DestroyTheList(List list)
        {
            List prev, curr;

            if (list == null)
                return;

            prev = list;
            curr = list.next;

            while (curr != null)
            {
                // if ( destructor )
                //    (*destructor)(prev -> node);

                //delete prev->node;

                prev.next = null;

                //delete prev;

                prev = curr;
                curr = curr.next;
            }

            // if( destructor )
            //    (*destructor)( prev -> node );

            //delete prev->node;

            prev.next = null;

            //delete prev;

            //ListGlobalPack();
        }

        //TODO private static extern float CalcKIAS(float , float);

        private static ACMI_CallRec[] ACMI_Callsigns = null;
        private static ACMI_CallRec[] Import_Callsigns = null;
        private static long import_count = 0;
    }
}
