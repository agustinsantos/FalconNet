using FalconNet.Common.Encoding;
using FalconNet.SimBase;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{

    public class SimMoverClass : SimBaseClass
    {
#if TODO
        private int* switchData;
        private int* switchChange;


        protected enum DOFTypes { AngleDof, TranslateDof, NoDof };
        protected DOFS numDofs;
        protected Switches numSwitches;
        protected Vertices numVertices;
        protected float* DOFData;
        protected float* VertexData;
        protected int[] DOFType;
        protected int dataRequested;
        protected int requestCount;
        protected SimMoverDefinition mvrDefinition;
        protected virtual void InitData()
        {
            /*   DOFData = null;
               DOFType = null;			M.N. these initialisations don't belong here
               VertexData = null;		Dogfight calls this when recreating a shot down aircraft
               switchData = null;	    switchData crashes first
               switchChange = null;*/
            targetList = null;
            numSensors = 0;
            sensorArray = null;
            requestCount = 0;
            dataRequested = 0;
            nonLocalData = null;
            pilotSlot = 255;
            vehicleInUnit = 0;
            onFlatFeature = FALSE;
            vt = 0.0f;
            kias = 0.0f;
        }

        protected virtual void Cleanup()
        {
#if TODO
	if (numSensors > 0 && (!sensorArray || F4IsBadReadPtr(sensorArray, sizeof(SensorClass*)))) // JB 010223 CTD
		return; // JB 010223 CTD

   for (int i = 0; i<numSensors; i++)
   {
		if (F4IsBadWritePtr(sensorArray[i], sizeof(SensorClass))) // JB 010223 CTD
			continue; // JB 010223 CTD

	  sensorArray[i].SetPower( FALSE );
      delete (sensorArray[i]);
	  sensorArray[i] = null;
	  numSensors = 0; // 2002-02-01 ADDED BY S.G. Say we don't have any sensors
   }
   //delete [] sensorArray;
   sensorArray = null;
   SimVuDriver *oldd = (SimVuDriver *)SetDriver(null);
   //if ( oldd )
      //delete oldd;

   // tear down nonlocal data
   if ( nonLocalData )
   {
	   // check for active smoketrail and send it to sfx for
	   // later removal
	   if ( nonLocalData.smokeTrail )
	   {
	      OTWDriver.AddSfxRequest(
			 new SfxClass ( 2.0f,		// time to live
			 nonLocalData.smokeTrail ) );

	      nonLocalData.smokeTrail = null;
	   }

	   delete nonLocalData;
	   nonLocalData = null;
   }
#endif
            throw new NotImplementedException();
        }

        protected float vt, kias;
        protected void MakeComplex()
        {
            numDofs = DOFS.COMP_MAX_DOF;
            numSwitches = COMP_MAX_SWITCH;
            numVertices = AIRCRAFT_MAX_DVERTEX;
            AllocateSwitchAndDof();
            for (int i = 6; i < 9; i++)
            {
                DOFType[i] = TranslateDof;
            }
        }
        protected void MakeSimple()
        {
            int i;
            numDofs = SIMP_MAX_DOF;
            numSwitches = SIMP_MAX_SWITCH;
            numVertices = AIRCRAFT_MAX_DVERTEX;
            AllocateSwitchAndDof();
            for (i = 2; i < 6 && i < numDofs; i++)
                DOFType[i] = NoDof;
        }

        protected void AllocateSwitchAndDof()
        {
            int i;
            if (DOFData)
                MemFreePtr(DOFData);
            if (DOFType)
                MemFreePtr(DOFType);
            if (switchData)
                MemFreePtr(switchData);
            if (switchChange)
                MemFreePtr(switchChange);

            if (VertexData)
                MemFreePtr(VertexData);

            if (numVertices)
            {
                VertexData = (float*)MemAllocPtr(graphicsDOFDataPool, sizeof(float) * numVertices, 0);
                for (i = 0; i < numVertices; i++)
                {
                    VertexData[i] = 0.0F;
                }
            }
            else
                VertexData = null;

            if (numDofs)
            {
                DOFData = (float*)MemAllocPtr(graphicsDOFDataPool, sizeof(float) * numDofs, 0);
                DOFType = (int*)MemAllocPtr(graphicsDOFDataPool, sizeof(float) * numDofs, 0);
                for (i = 0; i < numDofs; i++)
                {
                    DOFData[i] = 0.0F;
                    DOFType[i] = AngleDof;
                }
            }
            else
            {
                DOFData = null;
                DOFType = null;
            }

            if (numSwitches)
            {
                switchData = (int*)MemAllocPtr(graphicsDOFDataPool, sizeof(int) * numSwitches, 0);
                switchChange = (int*)MemAllocPtr(graphicsDOFDataPool, sizeof(int) * numSwitches, 0);

                for (i = 0; i < numSwitches; i++)
                {
                    switchChange[i] = TRUE;
                    switchData[i] = 0;
                }
            }
            else
            {
                switchData = null;
                switchChange = null;
            }
        }


        // Sensors
        public SensorClass** sensorArray;
        public int numSensors;

        // Targeting
        public SimObjectType targetPtr;
        public SimObjectType targetList;

        // Other Data
        public byte vehicleInUnit;		// The (vehicleSlot)th vehicle in the unit
        public byte pilotSlot;			// The (pilotSlot)th pilot in the unit

        // if we've hit a flat feature (ie for landing)
        // use this pointer to record the fact
        public bool onFlatFeature;

        public virtual float GetP()
        {
            return (0.0F);
        }
        public virtual float GetQ()
        {
            return (0.0F);
        }
        public virtual float GetR()
        {
            return (0.0F);
        }
        public virtual float GetAlpha()
        {
            return (0.0F);
        }
        public virtual float GetBeta()
        {
            return (0.0F);
        }
        public virtual float GetNx()
        {
            return (0.0F);
        }
        public virtual float GetNy()
        {
            return (0.0F);
        }
        public virtual float GetNz()
        {
            return (0.0F);
        }
        public virtual float GetGamma()
        {
            return (0.0F);
        }
        public virtual float GetSigma()
        {
            return (0.0F);
        }
        public virtual float GetMu()
        {
            return (0.0F);
        }
        public virtual float Vt() { return vt; }
        public virtual float Kias() { return kias; }
        public virtual void SetVt(float new_vt)
        {
            vt = new_vt;
        }
        public virtual void SetKias(float new_kias)
        {
            kias = new_kias;
        }

#endif
        public SimMoverClass(int type) :base(type)
        { throw new NotImplementedException(); }
#if TODO
        public SimMoverClass(ByteWrapper buf)
            : base(buf)
        { throw new NotImplementedException(); }
        public SimMoverClass(FileStream stream)
            : base(stream)
        { throw new NotImplementedException(); }

        //TODO virtual ~SimMoverClass (void);
        public virtual void Init(SimInitDataClass* initData)
        {
            Falcon4EntityClassType* classPtr;
            SimVuDriver* oldd;

            if (initData)
            {
                SetCampaignObject(initData.campUnit);
                // pilotSlot is only different from vehicleInUnit in the
                // case of player owned aircraft
                pilotSlot = vehicleInUnit = static_cast<uchar>(initData.vehicleInUnit);
            }

            SimBaseClass.Init(initData);

            // Init mover data
            classPtr = (Falcon4EntityClassType*)EntityType();
            // 2002-03-02 ADDED BY S.G. If not enough vehicles in vehicle.lst compared to the value in vehicleDataIndex, it will overflow the array
            if (classPtr.vehicleDataIndex >= NumSimMoverDefinitions)
                mvrDefinition = null;
            else
                // END OF ADDED SECTION
                mvrDefinition = moverDefinitionData[classPtr.vehicleDataIndex];
            ShiAssert(mvrDefinition);

            // SetDelta(0.0F, 0.0F, 0.0F);
            // SetYPRDelta(0.0F, 0.0F, 0.0F);
            if (IsLocal())
            {
                oldd = (SimVuDriver*)SetDriver(new SimVuDriver(this));
            }
            else
            {
                oldd = (SimVuDriver*)SetDriver(new SimVuSlave(this));
            }
            //TODO if ( oldd )
            //TODO	delete oldd;
        }

        public virtual int Exec()
        {
            //FalconControlSurfaceMsg* newControlData;
            int i;

            // JB 011231 Set the last move so the campaign honors the movement of the unit while deaggregated.  
            // Without setting this, when the unit reaggregates, the unit will replay its mission from the time that
            // it deaggregated so that movement not following the waypoints of the unit's mission is lost.
            if (GetCampaignObject())
                ((UnitClass*)GetCampaignObject()).SetUnitLastMove(TheCampaign.CurrentTime);

            if (drawPointer && !IsExploding())
            {
                for (i = 0; i < numSwitches; i++)
                {
                    if (switchChange[i])
                    {
                        ((DrawableBSP*)drawPointer).SetSwitchMask(i, switchData[i]);
                        switchChange[i] = FALSE;
                    }
                }
                for (i = 0; i < numDofs; i++)
                {
                    if (DOFType[i] == AngleDof)
                        ((DrawableBSP*)drawPointer).SetDOFangle(i, DOFData[i]);
                    else if (DOFType[i] == TranslateDof)
                        ((DrawableBSP*)drawPointer).SetDOFoffset(i, DOFData[i]);
                }

                for (i = 0; i < numVertices; i++)
                {
                    ((DrawableBSP*)drawPointer).SetDynamicVertex(i, VertexData[i], 0.0F, 0.0F);
                }
            }

            if (IsLocal())
            {
                if ((requestCount > 0 && (SimLibFrameCount & 0x2F) == 0) || ((SimLibFrameCount & 0x1FF) == 0))
                {
                    //newControlData = new FalconControlSurfaceMsg(Id(), FalconLocalGame);
                    //newControlData.dataBlock.gameTime = SimLibElapsedTime;
                    //newControlData.dataBlock.entityID = Id();
                    //newControlData.dataBlock.numDofs = numDofs;
                    //newControlData.dataBlock.numSwitches = numSwitches;
                    //newControlData.dataBlock.DOFData = DOFData;
                    //newControlData.dataBlock.DOFType = DOFType;
                    //newControlData.dataBlock.switchData = switchData;
                    //newControlData.dataBlock.specialData = SpecialData();
                    // FalconSendMessage (newControlData,FALSE);
                }
            }
            else // Isn't local
            {
                // non local execution
                if (nonLocalData)
                {
                    Tpoint pos, vec;

                    // IsFiring will be set\unset when a message is received from
                    // machine controlling the entity locally
                    if (IsFiring())
                    {
                        if (nonLocalData.flags & NONLOCAL_GUNS_FIRING)
                        {
                            // already firing, is it time to stop or fire another?
                            if (nonLocalData.timer2 <= SimLibElapsedTime)
                            {
                                // stop firing
                                nonLocalData.flags &= ~NONLOCAL_GUNS_FIRING;

                                // check for active smoketrail and send it to sfx for
                                // later removal
                                if (nonLocalData.smokeTrail)
                                {
                                    OTWDriver.AddSfxRequest(
                                        new SfxClass(2.0f,		// time to live
                                        nonLocalData.smokeTrail));

                                    nonLocalData.smokeTrail = null;
                                }
                            }
                            else if (nonLocalData.timer1 >= SimLibElapsedTime)
                            {

                                // fire another....
                                nonLocalData.timer1 = SimLibElapsedTime + 500.0f;
                                pos.x = XPos();
                                pos.y = YPos();
                                pos.z = ZPos();
                                vec.x = XDelta() * ((900 + rand() % 200) / 1000);
                                vec.y = YDelta() * ((900 + rand() % 200) / 1000);
                                vec.z = ZDelta() * ((900 + rand() % 200) / 1000);
                                OTWDriver.AddTrailHead(nonLocalData.smokeTrail, pos.x, pos.y, pos.z);

                                // for the moment (at least), bullets only go in direction
                                // object is pointing
                                // vec.x += dmx[0][0]*3000.0f;
                                // vec.y += dmx[0][1]*3000.0f;
                                // vec.z += dmx[0][2]*3000.0f;
                                vec.x += nonLocalData.dx * ((900 + rand() % 200) / 1000);
                                vec.y += nonLocalData.dy * ((900 + rand() % 200) / 1000);
                                vec.z += nonLocalData.dz * ((900 + rand() % 200) / 1000);
                                pos.x += vec.x * SimLibMajorFrameTime;
                                pos.y += vec.y * SimLibMajorFrameTime;
                                pos.z += vec.z * SimLibMajorFrameTime;

                                // add a tracer
                                OTWDriver.AddSfxRequest(
                                    new SfxClass(SFX_GUN_TRACER,				// type
                                    SFX_MOVES | SFX_USES_GRAVITY,						// flags
                                    &pos,							// world pos
                                    &vec,							// vector
                                    3.0f,							// time to live
                                    1.0f));							// scale
                            }
                        }
                        else if (!nonLocalData.timer2)
                        {
                            // we haven't yet started firing....
                            // will be set when recieving a fire message nonLocalData.flags |= NONLOCAL_GUNS_FIRING;

                            // timer1 is the time to fire next tracer
                            // timer2 is the maximum time we'll allow firing to continue
                            // (we may not receive an end fire message)
                            nonLocalData.timer1 = SimLibElapsedTime + 500.0f;
                            nonLocalData.timer2 = SimLibElapsedTime + 20000.0f;

                            // set up the smoke trail
                            pos.x = XPos();
                            pos.y = YPos();
                            pos.z = ZPos();
                            vec.x = XDelta();
                            vec.y = YDelta();
                            vec.z = ZDelta();
                            nonLocalData.smokeTrail = new DrawableTrail(TRAIL_GUN);
                            OTWDriver.InsertObject(nonLocalData.smokeTrail);
                            OTWDriver.AddTrailHead(nonLocalData.smokeTrail, pos.x, pos.y, pos.z);

                            // for the moment (at least), bullets only go in direction
                            // object is pointing
                            // vec.x += dmx[0][0]*3000.0f;
                            // vec.y += dmx[0][1]*3000.0f;
                            // vec.z += dmx[0][2]*3000.0f;
                            vec.x += nonLocalData.dx;
                            vec.y += nonLocalData.dy;
                            vec.z += nonLocalData.dz;
                            pos.x += vec.x * SimLibMajorFrameTime;
                            pos.y += vec.y * SimLibMajorFrameTime;
                            pos.z += vec.z * SimLibMajorFrameTime;

                            // add a tracer
                            OTWDriver.AddSfxRequest(
                                new SfxClass(SFX_GUN_TRACER,				// type
                                SFX_MOVES | SFX_USES_GRAVITY,						// flags
                                &pos,							// world pos
                                &vec,							// vector
                                3.0f,							// time to live
                                1.0f));							// scale
                        }
                    }
                    else // not firing
                    {
                        if (nonLocalData.flags & NONLOCAL_GUNS_FIRING)
                        {
                            // stop firing
                            nonLocalData.flags &= ~NONLOCAL_GUNS_FIRING;
                            // check for active smoketrail and send it to sfx for
                            // later removal
                            if (nonLocalData.smokeTrail)
                            {
                                OTWDriver.AddSfxRequest(
                                    new SfxClass(2.0f,		// time to live
                                    nonLocalData.smokeTrail));

                                nonLocalData.smokeTrail = null;
                            }
                        }
                    }
                }
            } // end of non-local condition

            return (IsLocal());
        }

        public virtual void SetLead(int l) { }
        public virtual void SetDead(int flag)
        {
            base.SetDead(flag);
            /*
            ** edg: moved to sleep function
            if (flag)
            {
               while (targetList)
               {
                  tmpObject = targetList;
                  targetList = targetList.next;
                  tmpObject.prev = null;
                  tmpObject.next = null;
                  tmpObject.Release();
                  tmpObject = null;
               }
               targetList = null;
               ClearTarget();

               for (int i=0; i<numSensors; i++)
               {
                  if (sensorArray[i])
                     sensorArray[i].SetPower( FALSE );
               }
            }
            */
        }

        public virtual int Sleep()
        {
            int numRef, i;

            SimObjectType* tmpObject;

            SimDriver.RemoveFromObjectList(this);

            base.Sleep();

            while (targetList)
            {
                tmpObject = targetList;
                targetList = targetList.next;
                tmpObject.prev = null;
                tmpObject.next = null;

                // use for debugging
                numRef = tmpObject.IsReferenced();

                tmpObject.Release(SIM_OBJ_REF_ARGS);
                tmpObject = null;
            }
            ClearTarget();

            for (i = 0; i < numSensors; i++)
            {
                if (sensorArray[i])
                    sensorArray[i].SetPower(FALSE);
            }

            // SetDelta(0.0F, 0.0F, 0.0F);
            // SetYPRDelta(0.0F, 0.0F, 0.0F);
            return 0;
        }
        public virtual int Wake()
        {
            int
                i;

            base.Wake();

            if (EntityDriver())
            {
                EntityDriver().ResetLastUpdateTime(vuxGameTime);
            }

            // SetDelta(0.0F, 0.0F, 0.0F);
            // SetYPRDelta(0.0F, 0.0F, 0.0F);

            if (drawPointer && !IsExploding())
            {
                for (i = 0; i < numSwitches; i++)
                {
                    if (switchChange[i])
                    {
                        ((DrawableBSP*)drawPointer).SetSwitchMask(i, switchData[i]);
                        switchChange[i] = FALSE;
                    }
                }
                for (i = 0; i < numDofs; i++)
                {
                    if (DOFType[i] == AngleDof)
                        ((DrawableBSP*)drawPointer).SetDOFangle(i, DOFData[i]);
                    else if (DOFType[i] == TranslateDof)
                        ((DrawableBSP*)drawPointer).SetDOFoffset(i, DOFData[i]);
                }

                for (i = 0; i < numVertices; i++)
                {
                    ((DrawableBSP*)drawPointer).SetDynamicVertex(i, VertexData[i], 0.0F, 0.0F);
                }
            }

            SimDriver.AddToObjectList(this);

            return 0;
        }
        public virtual void MakeLocal()
            {
	// LEON TODO: Need to do all necessary shit to convert from a remote to a local entity.
	SimBaseClass.MakeLocal();
	
	// Clear all nonLocal data
	if (nonLocalData)
	{
		// check for active smoketrail and send it to sfx for later removal
		if (nonLocalData.smokeTrail)
		{
			OTWDriver.AddSfxRequest(new SfxClass ( 2.0f, nonLocalData.smokeTrail ) );
			nonLocalData.smokeTrail = null;
		}
		delete nonLocalData;
		nonLocalData = null;
	}
	
	// The local (Master) driver.
	// SetDelta(0.0F, 0.0F, 0.0F);
	// SetYPRDelta(0.0F, 0.0F, 0.0F);

	SimVuDriver *drive = new SimVuDriver(this);
	drive.ExecDR(vuxGameTime);
	SimVuDriver *oldd = (SimVuDriver *)SetDriver (drive);
    //if ( oldd )
    //    delete oldd;
}

        public virtual void MakeRemote(){
	// LEON TODO: Need to do all necessary shit to convert from a remote to a local entity.
	SimBaseClass.MakeRemote();
	
	// Allocate and init nonLocal Data
	nonLocalData = new SimBaseNonLocalData();
	nonLocalData.flags = 0;
	nonLocalData.smokeTrail = null;
	nonLocalData.timer3 = (float)SimLibElapsedTime / SEC_TO_MSEC;
	
	// The remote (slave) driver
	// SetDelta(0.0F, 0.0F, 0.0F);
	// SetYPRDelta(0.0F, 0.0F, 0.0F);

	SimVuDriver *oldd = (SimVuDriver *)SetDriver(new SimVuSlave(this));
	if ( oldd )
		delete oldd;
}


        // this function can be called for entities which aren't necessarily
        // exec'd in a frame (ie ground units), but need to have their
        // gun tracers and (possibly other) weapons serviced
        public virtual void WeaponKeepAlive() { }

        // virtual function interface
        // serialization functions
        public virtual int SaveSize()
        {
            return SimBaseClass.SaveSize() +
               numDofs * (sizeof(float) + sizeof(int)) + // DOFData and DofType
               numSwitches * sizeof(int) + // SwitchData
               numVertices * sizeof(float) + // VertexData
               3 * sizeof(int) + // numDofs, numSwitches, numVertices;
               2 * sizeof(uchar); // pilotSlot and aircraftSlot;
            //   return SimBaseClass.SaveSize() + numDofs * (sizeof (float) + sizeof (int)) +
            //      (numSwitches + 3) * sizeof (int) + + numVertices * sizeof(float);
        }

#if TODO
        public virtual int Save(VU_BYTE** stream)	// returns bytes written
        {
            SimBaseClass.Save(stream);

            memcpy(*stream, &numDofs, sizeof(int));
            *stream += sizeof(int);
            memcpy(*stream, DOFData, sizeof(float) * numDofs);
            *stream += sizeof(float) * numDofs;

            memcpy(*stream, DOFType, sizeof(int) * numDofs);
            *stream += sizeof(int) * numDofs;

            memcpy(*stream, &numVertices, sizeof(int));
            *stream += sizeof(int);
            if (numVertices)
            {
                memcpy(*stream, VertexData, sizeof(float) * numVertices);
                *stream += sizeof(float) * numVertices;
            }

            memcpy(*stream, &numSwitches, sizeof(int));
            *stream += sizeof(int);
            memcpy(*stream, switchData, sizeof(int) * numSwitches);
            *stream += sizeof(int) * numSwitches;

            memcpy(*stream, &vehicleInUnit, sizeof(uchar));
            *stream += sizeof(uchar);
            memcpy(*stream, &pilotSlot, sizeof(uchar));
            *stream += sizeof(uchar);

            return numDofs * sizeof(float) + numSwitches * sizeof(int) + 2 * sizeof(int) + 2 * sizeof(uchar);
        }
#endif
        public virtual int Save(FILE file)		// returns bytes written
        {
            int retval;

            retval = base.Save(file);

            retval += fwrite(&numDofs, sizeof(int), 1, file);
            retval += fwrite(DOFData, sizeof(float), numDofs, file);
            retval += fwrite(DOFType, sizeof(int), numDofs, file);

            retval += fwrite(&numVertices, sizeof(int), 1, file);
            if (numVertices)
            {
                retval += fwrite(VertexData, sizeof(float), numVertices, file);
            }

            retval += fwrite(&numSwitches, sizeof(int), 1, file);
            retval += fwrite(switchData, sizeof(int), numSwitches, file);

            return (retval);
        }



        // event handlers
        public virtual int Handle(VuFullUpdateEvent* evnt)
        {
            SimMoverClass* tmpMover = (SimMoverClass*)(evnt.expandedData_);
            int i;

            // Copy Switch and DOF data
            memcpy(&numDofs, &tmpMover.numDofs, sizeof(int));
            memcpy(DOFData, tmpMover.DOFData, sizeof(float) * numDofs);
            memcpy(DOFType, tmpMover.DOFType, sizeof(int) * numDofs);

            memcpy(&numSwitches, &tmpMover.numSwitches, sizeof(int));

            for (i = 0; i < numSwitches; i++)
            {
                if (switchData[i] != tmpMover.switchData[i])
                {
                    switchData[i] = tmpMover.switchData[i];
                    switchChange[i] = TRUE;
                }
            }

            return (SimBaseClass.Handle(evnt));
        }


        public virtual int Handle(VuPositionUpdateEvent evnt)
        {
            UnitClass* campObj = (UnitClass*)GetCampaignObject();

            if (campObj && campObj.IsLocal() && campObj.GetComponentLead() == this)
            {
                campObj.SimSetLocation(evnt.x_, evnt.y_, evnt.z_);
                if (campObj.IsFlight())
                    campObj.SimSetOrientation(evnt.yaw_, evnt.pitch_, evnt.roll_);
            }

            return (SimBaseClass.Handle(evnt));
        }

        public virtual int Handle(VuTransferEvent evnt)
        {
            return (SimBaseClass.Handle(evnt));
        }



        // collision with feature
        public virtual SimBaseClass FeatureCollision(float groundZ)
        {
            CampBaseClass* objective;
#if VU_GRID_TREE_Y_MAJOR
VuGridIterator gridIt(ObjProxList, YPos(), XPos(), 3.0F * NM_TO_FT);
#else
            VuGridIterator gridIt = new VuGridIterator(ObjProxList, XPos(), YPos(), 3.0F * NM_TO_FT);
#endif
            SimBaseClass* foundFeature = null;
            SimBaseClass* testFeature;
            float radius;
            Tpoint pos, fpos, vec, p3, collide;
            BOOL firstFeature;
            WeaponClassDataType* wc = null;
            const float deltat = 1.0f; // JPO time to look ahead for flat surfaces - arrived at by experiment.
            // this required to land on highway strips - that are in forest or similar.

            if (IsWeapon())
            {
                wc = (WeaponClassDataType*)Falcon4ClassTable[Type() - VU_LAST_ENTITY_TYPE].dataPtr;
            }

            onFlatFeature = FALSE;

            // get the 1st objective that contains the bomb
            objective = (CampBaseClass*)gridIt.GetFirst();
            while (objective)
            {
                if (objective.GetComponents())
                {
                    pos.x = XPos();
                    pos.y = YPos();
                    pos.z = ZPos();

                    // check out some time in the future (was without multiplier JPO)
                    vec.x = XDelta() * SimLibMajorFrameTime * deltat;
                    vec.y = YDelta() * SimLibMajorFrameTime * deltat;
                    vec.z = ZDelta() * SimLibMajorFrameTime * deltat;

                    p3.x = (float)fabs(vec.x);
                    p3.y = (float)fabs(vec.y);
                    p3.z = (float)fabs(vec.z);

                    // loop thru each element in the objective
                    VuListIterator featureWalker = new VuListIterator(objective.GetComponents());
                    testFeature = (SimBaseClass)featureWalker.GetFirst();
                    firstFeature = TRUE;
                    while (testFeature)
                    {
                        if (testFeature.drawPointer)
                        {
                            // get feature's radius and position
                            radius = testFeature.drawPointer.Radius();
                            if (drawPointer)
                                radius += drawPointer.Radius();
                            testFeature.drawPointer.GetPosition(&fpos);

                            // test with gross level bounds of object
                            if (fabs(pos.x - fpos.x) < radius + p3.x &&
                                fabs(pos.y - fpos.y) < radius + p3.y &&
                                fabs(pos.z - fpos.z) < radius + p3.z)
                            {
                                // if we're on the ground make sure we have a downward vector if
                                // we're testing a flat container so we detect a collision
                                if (OnGround())
                                {
                                    if (testFeature.IsSetCampaignFlag(FEAT_FLAT_CONTAINER))
                                    {
                                        vec.z = 1500.0f;
                                        pos.z = groundZ - 50.0f;
                                    }
                                    else
                                    {
                                        vec.z = 0.0f;
                                        pos.z = groundZ - 0.5f;
                                    }
                                }
                                else
                                {
                                    vec.z = ZDelta() * SimLibMajorFrameTime * deltat;
                                    pos.z = ZPos();
                                }

                                if (testFeature.drawPointer.GetRayHit(&pos, &vec, &collide, 1.0f))
                                {
                                    if (IsWeapon())
                                    {
                                        FeatureClassDataType* fc = GetFeatureClassData(testFeature.Type() - VU_LAST_ENTITY_TYPE);

                                        if (fc.DamageMod[wc.DamageType])
                                            return testFeature;

                                        foundFeature = testFeature;
                                    }
                                    else
                                    {
                                        // if we're a bomb and we've hit a flat thingy, it's OK to
                                        // return detect a hit on the feature so return it....
                                        // other wise we just note if we're on top of a flat feature
                                        if (testFeature.IsSetCampaignFlag(FEAT_FLAT_CONTAINER))
                                        {
                                            onFlatFeature = TRUE;
                                        }
                                        else
                                        {
                                            if (FalconLocalSession && this == FalconLocalSession.GetPlayerEntity())
                                                g_intellivibeData.CollisionCounter++;

                                            return testFeature;
                                        }
                                    }
                                }
                            }
                        }
                        testFeature = (SimBaseClass*)featureWalker.GetNext();
                        firstFeature = FALSE;
                    }
                }

                objective = (CampBaseClass*)gridIt.GetNext();
            }

            if (foundFeature && FalconLocalSession && this == FalconLocalSession.GetPlayerEntity())
                g_intellivibeData.CollisionCounter++;

            return foundFeature;
        }

        public virtual int CheckLOS(SimObjectType* obj)
        {
            if (!obj || !obj.BaseData())
                return FALSE;

            if (SimLibElapsedTime > obj.localData.nextLOSCheck)
                UpdateLOS(obj);

            return obj.localData.TerrainLOS();
        }

        public virtual int CheckCompositeLOS(SimObjectType* obj)
        {
            if (!obj || !obj.BaseData())
                return FALSE;

            if (SimLibElapsedTime > obj.localData.nextLOSCheck)
                UpdateLOS(obj);

            if (obj.localData.CloudLOS())
                return obj.localData.TerrainLOS();
            else
                return FALSE;
        }

        public void UpdateLOS(SimObjectType* obj)
        {
            float top, bottom;

            // See if the target is near the ground
            OTWDriver.GetAreaFloorAndCeiling(&bottom, &top);
            if (ZPos() < top && obj.BaseData().ZPos() < top)
                obj.localData.SetTerrainLOS(TRUE);
            else if (OTWDriver.CheckLOS(this, obj.BaseData()))
                obj.localData.SetTerrainLOS(TRUE);
            else
                obj.localData.SetTerrainLOS(FALSE);

            if (OTWDriver.CheckCloudLOS(this, obj.BaseData()))
                obj.localData.SetCloudLOS(TRUE);
            else
                obj.localData.SetCloudLOS(FALSE);

            if (!OnGround())
                obj.localData.nextLOSCheck = SimLibElapsedTime + 200;
            else if (!obj.BaseData().OnGround())
                obj.localData.nextLOSCheck = SimLibElapsedTime + 1000;
            else
                obj.localData.nextLOSCheck = SimLibElapsedTime + 5000;
        }


        public void SetDOFs(float* newData)
        {
            memcpy(DOFData, newData, sizeof(float) * numDofs);
            memcpy(DOFType, newData, sizeof(float) * numDofs);
        }

        public void SetSwitches(int* newData)
        {
            memcpy(switchData, newData, sizeof(int) * numSwitches);
        }
        public void SetDOF(int dof, float val) { ShiAssert(dof < numDofs); if (dof < numDofs) { DOFData[dof] = val; } }
        public void SetDOFInc(int dof, float val) { ShiAssert(dof < numDofs); if (dof < numDofs) { DOFData[dof] += val; } }
        public float GetDOFValue(int dof) { ShiAssert(dof < numDofs); return dof < numDofs ? DOFData[dof] : 0; }
        public void SetDOFType(int dof, int type) { ShiAssert(dof < numDofs); DOFType[dof] = type; }
        public void SetSwitch(int num, int val) { ShiAssert(num < numSwitches); if (num < numSwitches) { switchData[num] = val; switchChange[num] = TRUE; } }
        public int GetSwitch(int num) { return num < numSwitches ? switchData[num] : 0; }
        public void AddDataRequest(int flag)
        {
            requestCount += flag;
        }

        public int DataRequested() { return dataRequested; }
        public int GetNumSwitches() { return numSwitches; }
        public int GetNumDOFs() { return numDofs; }
        public void SetDataRequested(int flag) { dataRequested = flag; }
        public void SetTarget(SimObjectType* newTarget)
        {
            if (newTarget == targetPtr)
                return;

            if (targetPtr)
                targetPtr.Release(SIM_OBJ_REF_ARGS);

            if (newTarget)
            {
                ShiAssert(newTarget.BaseData() != (FalconEntity*)0xDDDDDDDD);
                newTarget.Reference(SIM_OBJ_REF_ARGS);
            }

            targetPtr = newTarget;
        }

        public void ClearTarget()
        {
            if (targetPtr)
            {
                targetPtr.Release(SIM_OBJ_REF_ARGS);
                targetPtr = null;
            }
        }

        public virtual VU_ERRCODE InsertionCallback();
        public virtual VU_ERRCODE RemovalCallback();
        public virtual bool IsMover() { return true; }
#endif
    }
}
