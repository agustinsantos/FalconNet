using FalconNet.Common;
using System;
using System.Diagnostics;
using DWORD = System.UInt32;
using BOOL = System.Boolean;
using FalconNet.Common.Graphics;

namespace FalconNet.Graphics
{
    public class DrawableBSP : DrawableObject
    {

        public DrawableBSP(int ID, Tpoint pos, Trotation rot, float scale = 1.0f)
            : base(scale)
        {
            instance = new ObjectInstance(ID);
            // Initialize our member variables
            id = ID;
            label = null;
            labelLen = 0;
            drawClassID = DrawClass.BSP;
            inhibitDraw = false;

            // Record our position
            position = pos;

            // Ask the object library what size this object is
            radius = instance.Radius();

            // Store our rotation matrix
            orientation = rot;
        }

        //TODO public virtual ~DrawableBSP();

        // This constructor is used only by derived classes who do their own setup
        protected DrawableBSP(float s, int ID)
            : base(s)
        {
            instance = new ObjectInstance(ID);
            inhibitDraw = false;
            labelLen = 0;
            id = ID;
            radius = instance.Radius();
        }


        public void Update(Tpoint pos, Trotation rot)
        {
            Debug.Assert(id >= 0);

            Debug.Assert(!Single.IsNaN(position.x));
            // Update the location of this object
            position.x = pos.x;
            position.y = pos.y;

            if (GetClass() != DrawClass.GroundVehicle)
                position.z = pos.z;

            orientation = rot;
        }

        public bool IsLegalEmptySlot(int slotNumber)
        {
            return (slotNumber < instance.ParentObject.nSlots) && (instance.SlotChildren != null) && (instance.SlotChildren[slotNumber] == null);
        }

        public int GetNumSlots() { return instance.ParentObject.nSlots; }
        public int GetNumDOFs() { return instance.ParentObject.nDOFs; }
        public int GetNumSwitches() { return instance.ParentObject.nSwitches; }
        public int GetNumDynamicVertices() { return instance.ParentObject.nDynamicCoords; }

        /// <summary>
        ///    Attach an object as a child (no further updates need to be done on it)
        /// </summary>
        /// <param name="child"></param>
        /// <param name="slotNumber"></param>
        public void AttachChild(DrawableBSP child, int slotNumber)
        {
            Debug.Assert(id >= 0);
            Debug.Assert(child != null);
            Debug.Assert(slotNumber >= 0);
            Debug.Assert(slotNumber < instance.ParentObject.nSlots);
            Debug.Assert((instance.SlotChildren != null) && (instance.SlotChildren[slotNumber] == null));

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE SLOTS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (instance.SlotChildren == null) return;

            if (slotNumber >= instance.ParentObject.nSlots) return;

            if (child == null) return;

            instance.SetSlotChild(slotNumber, child.instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="slotNumber"></param>
        public void DetachChild(DrawableBSP child, int slotNumber)
        {
            Debug.Assert(id >= 0);
            Debug.Assert(child != null);
            Debug.Assert(slotNumber >= 0);
            Debug.Assert(slotNumber < instance.ParentObject.nSlots);
            Debug.Assert((instance.SlotChildren != null) && (instance.SlotChildren[slotNumber] == child.instance));

            Tpoint offset;
            Tpoint pos = new Tpoint();

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE SLOTS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (slotNumber >= instance.ParentObject.nSlots)
            {
                return;
            }

            if ((instance.SlotChildren == null) || (instance.SlotChildren[slotNumber] != child.instance))
            {
                //(*(int*)0) = 0;
                return;
            }

            // Get the childs offset from the parent in object space
            GetChildOffset(slotNumber, out offset);

            // Rotate the offset into world space and add the parents position
            pos.x = orientation.M11 * offset.x + orientation.M12 * offset.y + orientation.M13 * offset.z + position.x;
            pos.y = orientation.M21 * offset.x + orientation.M22 * offset.y + orientation.M23 * offset.z + position.y;
            pos.z = orientation.M31 * offset.x + orientation.M32 * offset.y + orientation.M33 * offset.z + position.z;

            // Update the child's location
            child.position = pos;

            // Give the child the same orientation as the parent
            // (We used to set scale as well, but that cause a few problems, so we'll leave
            //  that up to the application...)
            child.orientation = orientation;

            // Stop drawing the child object as an attachment
            instance.SetSlotChild(slotNumber, null);
        }

        /// <summary>
        ///    Return the object space location of the slot connect point indicated
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="offset"></param>
        public void GetChildOffset(int slotNumber, out Tpoint offset)
        {
            Debug.Assert(id >= 0);
            Debug.Assert(slotNumber >= 0);
            Debug.Assert(slotNumber < instance.ParentObject.nSlots);
            offset = new Tpoint();

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE SLOTS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (slotNumber >= instance.ParentObject.nSlots) return;

            offset = instance.ParentObject.pSlotAndDynamicPositions[slotNumber];
            offset.x *= scale;
            offset.y *= scale;
            offset.z *= scale;
        }

        /// <summary>
        ///    Set the angle control values for a degree of freedom in the model
        /// </summary>
        /// <param name="DOF"></param>
        /// <param name="radians"></param>
        public void SetDOFangle(int DOF, float radians)
        {
            Debug.Assert(id >= 0);

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DOFS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (DOF >= instance.ParentObject.nDOFs) return;

            Debug.Assert(DOF < instance.ParentObject.nDOFs);
            instance.DOFValues[DOF].rotation = radians;
        }

        /// <summary>
        /// Set the offset control values for a degree of freedom in the model
        /// </summary>
        /// <param name="DOF"></param>
        /// <param name="offset"></param>
        public void SetDOFoffset(int DOF, float offset)
        {
            Debug.Assert(id >= 0);

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DOFS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (DOF >= instance.ParentObject.nDOFs) return;

            Debug.Assert(DOF < instance.ParentObject.nDOFs);

            if (DOF < instance.ParentObject.nDOFs)
                instance.DOFValues[DOF].translation = offset;
        }

        public void SetDynamicVertex(int vertID, float dx, float dy, float dz)
        {
            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DYANAMIC VERTS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (vertID >= instance.ParentObject.nDynamicCoords) return;

            Debug.Assert(vertID < instance.ParentObject.nDynamicCoords);
            instance.SetDynamicVertex(vertID, dx, dy, dz);
        }

        public void SetSwitchMask(int switchNumber, UInt32 mask)
        {
            Debug.Assert(id >= 0);

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DOFS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (switchNumber >= instance.ParentObject.nSwitches) return;

            Debug.Assert(switchNumber < instance.ParentObject.nSwitches);

            if (switchNumber < instance.ParentObject.nSwitches)
                instance.SwitchValues[switchNumber] = mask;
        }

        public void SetTextureSet(int set) { instance.SetTextureSet(set); }
        public int GetNTextureSet() { return instance.GetNTextureSet(); }

        public float GetDOFangle(int DOF)
        {
            Debug.Assert(id >= 0);

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DOFS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (DOF >= instance.ParentObject.nDOFs) return 0.0f;

            Debug.Assert(DOF < instance.ParentObject.nDOFs);
            return instance.DOFValues[DOF].rotation;
        }

        public float GetDOFoffset(int DOF)
        {
            Debug.Assert(id >= 0);

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DOFS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (DOF >= instance.ParentObject.nDOFs) return 0.0f;

            Debug.Assert(DOF < instance.ParentObject.nDOFs);
            return instance.DOFValues[DOF].translation;
        }

        public void GetDynamicVertex(int vertID, ref float dx, ref float dy, ref float dz)
        {
            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DYANAMIC VERTS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (vertID >= instance.ParentObject.nDynamicCoords)
            {
                dx = dy = dz = 0.0f;
                return;
            }

            Debug.Assert(vertID < instance.ParentObject.nDynamicCoords);
            instance.GetDynamicVertex(vertID, ref dx, ref dy, ref dz);
        }

        public void GetDynamicCoords(int vertID, out float dx, out float dy, out float dz)
        {
#if TODO
            if (vertID >= instance.ParentObject.nDynamicCoords)
            {
                dx = dy = dz = 0.0f;
                return;
            }

            Debug.Assert(vertID < instance.ParentObject.nDynamicCoords);
            instance.GetDynamicCoords(vertID, ref dx, ref dy, ref dz);
#endif
            throw new NotImplementedException();
        }

        public UInt32 GetSwitchMask(int switchNumber)
        {
            Debug.Assert(id >= 0);

            // THIS IS A HACK TO TOLERATE OBJECTS WHICH DON'T YET HAVE DOFS
            // THIS SHOULD BE REMOVED IN THE LATE BETA AND SHIPPING VERSIONS
            if (switchNumber >= instance.ParentObject.nSwitches) return 0;

            Debug.Assert(switchNumber < instance.ParentObject.nSwitches);

            if (switchNumber < instance.ParentObject.nSwitches)
                return instance.SwitchValues[switchNumber];
            else return 0;
        }

        public Int32 GetTextureSet() { return instance.TextureSet; }

        public string Label() { return label; }
        public DWORD LabelColor() { return labelColor; }
        public override void SetLabel(string labelString, DWORD color)
        {
            //Debug.Assert(strlen(labelString) < sizeof(label));

            label = labelString;
            //label[31] = 0;
            labelColor = color;
            labelLen = VirtualDisplay.ScreenTextWidth(labelString) >> 1;
        }

        public override void SetInhibitFlag(bool state) { inhibitDraw = state; }

        enum Quadrant { LEFT, RIGHT, MIDDLE }
        public override bool GetRayHit(Tpoint from, Tpoint vector, Tpoint collide, float boxScale = 1.0f)
        {
            Tpoint origin = new Tpoint(), vec = new Tpoint();
            Tpoint pos = new Tpoint();
            int i = 0;
            float[] minBp = null, maxBp = null, orgp = null, vecp = null, collp = null;
            Quadrant[] quadrant = new Quadrant[3] { Quadrant.LEFT, Quadrant.LEFT, Quadrant.LEFT };
            float t = 0.0F, tMax = 0.0F;
            float[] minB = new float[3] { 0.0F, 0.0F, 0.0F };
            float[] maxB = new float[3] { 0.0F, 0.0F, 0.0F };
            float[] candidatePlane = new float[3] { 0.0F, 0.0F, 0.0F };
            int whichPlane = 0;
            BOOL inside = true;

            // First we transform the origin and vector into object space (since that's easier than rotating the box)
            pos.x = from.x - position.x;
            pos.y = from.y - position.y;
            pos.z = from.z - position.z;
            origin.x = pos.x * orientation.M11 + pos.y * orientation.M21 + pos.z * orientation.M31;
            origin.y = pos.x * orientation.M12 + pos.y * orientation.M22 + pos.z * orientation.M32;
            origin.z = pos.x * orientation.M13 + pos.y * orientation.M23 + pos.z * orientation.M33;
            vec.x = vector.x * orientation.M11 + vector.y * orientation.M21 + vector.z * orientation.M31;
            vec.y = vector.x * orientation.M12 + vector.y * orientation.M22 + vector.z * orientation.M32;
            vec.z = vector.x * orientation.M13 + vector.y * orientation.M23 + vector.z * orientation.M33;

            // Account for object scaling
            boxScale *= scale;

            if (boxScale == 1.0f)
            {
                minB[0] = instance.BoxBack();
                minB[1] = instance.BoxLeft();
                minB[2] = instance.BoxTop();
                maxB[0] = instance.BoxFront();
                maxB[1] = instance.BoxRight();
                maxB[2] = instance.BoxBottom();
            }
            else
            {
                minB[0] = boxScale * instance.BoxBack();
                minB[1] = boxScale * instance.BoxLeft();
                minB[2] = boxScale * instance.BoxTop();
                maxB[0] = boxScale * instance.BoxFront();
                maxB[1] = boxScale * instance.BoxRight();
                maxB[2] = boxScale * instance.BoxBottom();
            }

            // find candiate planes
            orgp = (float[])origin;
            minBp = (float[])minB;
            maxBp = (float[])maxB;

            for (i = 0; i < 3; i++)
            {
                if (orgp[i] < minBp[i])
                {
                    quadrant[i] = Quadrant.LEFT;
                    candidatePlane[i] = minBp[i];
                    inside = false;
                }
                else if (orgp[i] > maxBp[i])
                {
                    quadrant[i] = Quadrant.RIGHT;
                    candidatePlane[i] = maxBp[i];
                    inside = false;
                }
                else
                {
                    quadrant[i] = Quadrant.MIDDLE;
                }
            }

            // origin is in box
            if (inside)
            {
                collide = from;
                return true;
            }

            // calculate T distances to candidate planes and accumulate the largest
            if (quadrant[0] != Quadrant.MIDDLE && vec.x != 0.0f)
            {
                tMax = (candidatePlane[0] - origin.x) / vec.x;
                whichPlane = 0;
            }
            else
            {
                tMax = -1.0f;
            }

            if (quadrant[1] != Quadrant.MIDDLE && vec.y != 0.0f)
            {
                t = (candidatePlane[1] - origin.y) / vec.y;

                if (t > tMax)
                {
                    tMax = t;
                    whichPlane = 1;
                }
            }

            if (quadrant[2] != Quadrant.MIDDLE && vec.z != 0.0f)
            {
                t = (candidatePlane[2] - origin.z) / vec.z;

                if (t > tMax)
                {
                    tMax = t;
                    whichPlane = 2;
                }
            }

            // Check final candidate is within the segment of interest
            if (tMax < 0.0f || tMax > 1.0f)
            {
                return false;
            }

            // Check final candidate is within the bounds of the side of the box
            orgp = (float[])origin;
            vecp = (float[])vec;
            collp = (float[])pos;

            for (i = 0; i < 3; i++)
            {
                if (whichPlane != i)
                {
                    collp[i] = orgp[i] + tMax * (vecp[i]);

                    if (collp[i] < minB[i] || collp[i] > maxB[i])
                    {
                        // outside box
                        return false;
                    }
                }
                else
                {
                    collp[i] = candidatePlane[i];
                }
            }

            // We must transform the collision point from object space back into world space
            collide.x = pos.x * orientation.M11 + pos.y * orientation.M12 + pos.z * orientation.M13 + position.x;
            collide.y = pos.x * orientation.M21 + pos.y * orientation.M22 + pos.z * orientation.M23 + position.y;
            collide.z = pos.x * orientation.M31 + pos.y * orientation.M32 + pos.z * orientation.M33 + position.z;


            return true;
        }

        // This function setup  visibility stuff for a BSP
        // it returns false is the BSP results not visible for any reason
        public bool SetupVisibility(RenderOTW renderer)
        {
#if TODO
            float alpha, fog, z;

            // RED - Linear Fog - checvk if under visibility limit
            if (position.z > realWeather.VisibleLimit()) return false;


            //////////////////////////////////// FOG / HAZE ///////////////////////////////////////////////////

            z = renderer.ZDistanceFromCamera(position);

            // RED - Linear Fog, if inside the layer, modulate with Hze, we can not use linear fog there
            if (realWeather.weatherCondition > FAIR && position.z > (realWeather.HiOvercast))
            {
                alpha = 1.0f - (-realWeather.HiOvercast + position.z) / (realWeather.stratusDepth / 2.0f);
                alpha *= alpha * alpha;
            }
            else
            {
                if (z > renderer.haze_start + renderer.haze_depth) alpha = 0.0f;
                else if (z < renderer.PERSPECTIVE_RANGE) alpha = 1.0f;
                else
                {
                    if (renderer.GetHazeMode())
                    {
                        fog = Math.Min(renderer.GetValleyFog(z, position.z), .65f);

                        if (z < renderer.haze_start) alpha = 1.0f - fog;
                        else
                        {
                            alpha = renderer.GetRangeOnlyFog(z);

                            if (alpha < fog) alpha = fog;

                            alpha = 1.0f - alpha;
                        }
                    }
                    else
                    {
                        if (z < renderer.haze_start) alpha = 1.0f;
                        else
                        {
                            alpha = renderer.GetRangeOnlyFog(z);
                            alpha = 1.0f - alpha;
                        }
                    }
                }
            }

            // Set the Fog stuff...
            TheStateStack.SetFog(alpha, (Pcolor)renderer.GetFogColor());


            // OBJECT TO DRAW
            return true;
#endif
            throw new NotImplementedException();
        }

        public override void Draw(RenderOTW renderer, int LOD)
        {
#if TODO
            ThreeDVertex labelPoint;
            float x, y;

            Debug.Assert(id >= 0);

            // check for inhibit
            if (inhibitDraw)
            {
                SetInhibitFlag(false);
                return;
            }

            if (!SetupVisibility(renderer)) return;

            // JB 010112
            float scalefactor = 1;

            if (g_bSmartScaling || PlayerOptions.ObjectDynScalingOn())
            {
                renderer.TransformPoint(position, ref labelPoint);

                if (radius <= 150 && (GetClass() == DrawClass.Guys || GetClass() == DrawClass.GroundVehicle || GetClass() == DrawClass.BSP))
                    scalefactor = (labelPoint.csZ - 1200) / 6076 + 1;

                if (scalefactor < 1)
                    scalefactor = 1;
                else if (scalefactor > 2)
                    scalefactor = 2;
            }

            // JB 010112

            // Draw the object
            //JAM 09Dec03

            BOOL isShadow = false;

            if (PlayerOptions.ShadowsOn() && realWeather.weatherCondition == FAIR)
            {
                Tpoint pv;
                Tcolor light;

                TheTimeOfDay.GetTextureLightingColor(&light);
                renderer.TransformPointToViewSwapped(&position, &pv);

                for (int row = 2; row < realWeather.numCells - 2; row++)
                {
                    for (int col = 2; col < realWeather.numCells - 2; col++)
                    {
                        if (realWeather.weatherCellArray[row][col].onScreen)
                        {
                            float dx = pv.x - realWeather.weatherCellArray[row][col].shadowPos.x;
                            float dy = pv.y - realWeather.weatherCellArray[row][col].shadowPos.y;
                            float dz = pv.z - realWeather.weatherCellArray[row][col].shadowPos.z;
                            float range = (float)Math.Abs(Math.Sqrt(dx * dx + dy * dy + dz * dz));

                            if (range < realWeather.cloudRadius)
                            {
                                isShadow = true;

                                float interp = Math.Max(1.f - (realWeather.cloudRadius - range) / realWeather.cloudRadius, .5f);

                                float r = interp * light.r;
                                float g = interp * light.g;
                                float b = interp * light.b;

                                TheColorBank.SetLight(r, g, b);
                            }
                        }
                    }
                }
            }

            if (g_bSmartScaling || PlayerOptions.ObjectDynScalingOn())
                TheStateStack.DrawObject(instance, orientation, position, scale * scalefactor);  // JB 010112 added scalefactor
            else
                TheStateStack.DrawObject(instance, orientation, position, scale);

            if (isShadow)
            {
                Tcolor light;

                TheTimeOfDay.GetTextureLightingColor(ref light);
                TheColorBank.SetLight(light.r, light.g, light.b);
            }


#if  DEBUG_LOD_ID

    // Now compute the starting location for our label text
    if (drawLabels && TheDXEngine.GetLodUsedLabel()[0])
    {
        renderer.TransformPoint(&position, &labelPoint);

        if (labelPoint.clipFlag == ON_SCREEN)
        {
            x = labelPoint.x - labelLen; // Centers text
            y = labelPoint.y - 12; // Place text above center of object
            renderer.SetColor(labelColor);
            renderer.SetFont(2);
            renderer.ScreenText(x, y, TheDXEngine.GetLodUsedLabel());
        }
    }

#else

            // Now compute the starting location for our label text
            if (drawLabels && labelLen)
            {
                if (!g_bSmartScaling && !PlayerOptions.ObjectDynScalingOn())
                    renderer.TransformPoint(position, ref labelPoint);   // JB 010112

                // JB 000807 Add near label limit and labels that get brighter as they get closer
                // if ( labelPoint.clipFlag == ON_SCREEN )//-
                // {//-
                // x = labelPoint.x - renderer.ScreenTextWidth(label) / 2; // Centers text//-
                // y = labelPoint.y - 12; // Place text above center of object//-
                // renderer.SetColor( labelColor );//-
                // renderer.ScreenText( x, y, label );//-
                // } //-

                // RV - RED - If ACMI force Label Limit to 150 nMiles
                long limit = (renderACMI ? 150 : g_nNearLabelLimit) * 6076 + 8, limitcheck;

                if (!DrawablePoint.drawLabels)
                    limitcheck = (renderACMI ? 150 : g_nNearLabelLimit) * 6076 + 8;
                else limitcheck = 300 * 6076 + 8; //

                //dpc LabelRadialDistanceFix
                //First check if Z distance is below "limitcheck" and only if it is then do additional
                //radial distance check (messes up .csZ value - but it shouldn't matter
                // since labelPoint is local and .csZ is not used afterwards)
                // Besides no need to calculate radial distance is Z distance is already greater
                if (g_bLabelRadialFix)
                    if (labelPoint.clipFlag == ClippingFlags.ON_SCREEN &&
                        labelPoint.csZ < limitcheck) //Same condition as below!!!
                    {
                        float dx = position.x - renderer.X();
                        float dy = position.y - renderer.Y();
                        float dz = position.z - renderer.Z();
                        labelPoint.csZ = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                    }

                //end LabelRadialDistanceFix

                if (labelPoint.clipFlag == ClippingFlags.ON_SCREEN &&
                    labelPoint.csZ < limitcheck)
                {
                    int colorsub = (int)((labelPoint.csZ / (limit >> 3))) << 5;

                    if (colorsub > 180) // let's not reduce brightness too much, keep a glimpse of the original color
                        colorsub = 180;

                    int red = (int)(labelColor & 0x000000ff);
                    red -= Math.Min(red, colorsub);
                    int green = (int)((labelColor & 0x0000ff00) >> 8);
                    green -= Math.Min(green, colorsub + 30); // green would be too light . +30
                    int blue = (int)((labelColor & 0x00ff0000) >> 16);
                    blue -= Math.Min(blue, colorsub);

                    long newlabelColor = blue << 16 | green << 8 | red;

                    x = labelPoint.x - renderer.ScreenTextWidth(label) / 2; // Centers text
                    y = labelPoint.y - 12; // Place text above center of object
                    renderer.SetColor(newlabelColor);
                    renderer.ScreenText(x, y, label);

                    //dpc LabelRadialDistanceFix
                    if (g_bLabelShowDistance)
                    {
                        string label2;
                        sprintf(label2, "%4.1f nm", labelPoint.csZ / 6076); // convert from ft to nm
                        float x2 = labelPoint.x - renderer.ScreenTextWidth(label2) / 2; // Centers text
                        float y2 = labelPoint.y + 4; // Distance below center object
                        renderer.ScreenText(x2, y2, label2);
                    }

                    //end LabelRadialDistanceFix
                }

                // JB 000807
            }

#endif

            if (g_bDrawBoundingBox) DrawBoundingBox(renderer);

#if  _DEBUG
    // TESTING CODE TO SHOW BOUNDING BOXES
    // DrawBoundingBox( renderer );
#endif
#endif
        }

        public override void Draw(Render3D renderer)
        {
#if TODO
            ThreeDVertex labelPoint;
            float x, y;

            Debug.Assert(id >= 0);

            if (renderer == null)
                return;

            // RED - NOPE - must be similar to any object
            // Make sure no left over fog affects this object...
            //TheStateStack.SetFog(1.f,null); //JAM 26Dec03

            TheStateStack.DrawObject(instance, orientation, position, scale);

#if  DEBUG_LOD_ID

    // Now compute the starting location for our label text
    if (drawLabels && TheDXEngine.GetLodUsedLabel()[0])
    {
        renderer.TransformPoint(&position, &labelPoint);

        if (labelPoint.clipFlag == ON_SCREEN)
        {
            x = labelPoint.x - labelLen; // Centers text
            y = labelPoint.y - 12; // Place text above center of object
            renderer.SetFont(2);
            renderer.SetColor(labelColor);
            renderer.ScreenText(x, y, TheDXEngine.GetLodUsedLabel());
        }
    }

#else

            // Now compute the starting location for our label text
            if (drawLabels && labelLen != 0)
            {
                renderer.TransformPoint(position, ref labelPoint);

                if (labelPoint.clipFlag == ClippingFlags.ON_SCREEN)
                {
                    x = labelPoint.x - labelLen; // Centers text
                    y = labelPoint.y - 12; // Place text above center of object
                    renderer.SetColor(labelColor);
                    renderer.ScreenText(x, y, label);
                }
            }

#endif
#endif
            throw new NotImplementedException();
        }


        public int GetID() { return id; }

        // These two functions are used to handle preloading BSP objects for quick drawing later
        public static void LockAndLoad(int id)
        {
#if TODO
            TheObjectList[id].ReferenceWithFetch(); 
#endif
            throw new NotImplementedException();
        }

        public static void Unlock(int id)
        {
#if TODO
            TheObjectList[id].Release(); 
#endif
            throw new NotImplementedException();
        }

        // get object's bounding box
        public void GetBoundingBox(ref Tpoint minB, ref Tpoint maxB)
        {
            minB.x = instance.BoxBack();
            minB.y = instance.BoxLeft();
            minB.z = instance.BoxTop();
            maxB.x = instance.BoxFront();
            maxB.y = instance.BoxRight();
            maxB.z = instance.BoxBottom();
        }


        // This one is for internal use only.  Don't use it or you'll break things...
        public void ForceZ(float z) { position.z = z; }


        public Trotation orientation;
        public static bool drawLabels;		// Shared by ALL drawable BSPs

        public ObjectInstance instance;


        protected int id;				// TODO: With the new BSP lib, this id could go...
        //ObjectInstance		instance;

        protected bool inhibitDraw;	// When true, the Draw function just returns

        protected string label;
        protected int labelLen;
        protected DWORD labelColor;

        // Handle heading of day notifications
        protected static void TimeUpdateCallback(Object unused)
        {
#if TODO
            Tcolor light;

            // Get the light level from the time of day manager
            TheTimeOfDay.GetTextureLightingColor(&light);

            // Update the staticly lit object colors
            TheColorBank.SetLight(light.r, light.g, light.b);

            /*JAM 05Jan04
             // Update all the textures which aren't dynamicly lit
             ThePaletteBank.LightReflectionPalette( 2, &light );
             ThePaletteBank.LightBuildingPalette( 3, &light );*/
#endif
            throw new NotImplementedException();
        }


        public static void SetupTexturesOnDevice(DXContext rc)
        {
#if TODO
            // Initialize the lighting conditions and register for future time of day updates
            TimeUpdateCallback(null);
            TheTimeManager.RegisterTimeUpdateCB(TimeUpdateCallback, null);
#endif
            throw new NotImplementedException();
        }
        public static void ReleaseTexturesOnDevice(DXContext rc)
        {
#if TODO
            // Stop receiving time updates
            TheTimeManager.ReleaseTimeUpdateCB(TimeUpdateCallback, null);
#endif
            throw new NotImplementedException();
        }



        protected void DrawBoundingBox(Render3D renderer)
        {
#if TODO
            Tpoint max, min;
            Tpoint p, p1, p2;
            Trotation M;

            Debug.Assert(id >= 0);

            // TEMPORARY:  We're putting the data into the min/max structure in Erick's old
            // x Right, y Down, z Front ordering to avoid changes in the code below....
            max.x = instance.BoxRight();
            max.y = instance.BoxBottom();
            max.z = instance.BoxFront();
            min.x = instance.BoxLeft();
            min.y = instance.BoxTop();
            min.z = instance.BoxBack();

            // MatrixTranspose( &orientation, &M );
            M = orientation;

            renderer.SetColor(0xFF0000FF);

            // We need to transform the box into world space -- right now it is in object space
            p.x = max.z;
            p.y = max.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = max.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = max.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = min.z;
            p.y = max.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = max.z;
            p.y = min.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = min.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = min.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = min.z;
            p.y = min.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = max.z;
            p.y = max.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = min.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = min.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = min.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = min.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = min.z;
            p.y = max.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = max.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = max.x;
            p.z = max.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = max.z;
            p.y = max.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = min.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = min.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = min.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = min.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = min.z;
            p.y = max.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);

            p.x = min.z;
            p.y = max.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p1);
            p1.x += position.x;
            p1.y += position.y;
            p1.z += position.z;

            p.x = max.z;
            p.y = max.x;
            p.z = min.y;
            MatrixMult(&M, &p, &p2);
            p2.x += position.x;
            p2.y += position.y;
            p2.z += position.z;

            renderer.Render3DLine(&p1, &p2);
#endif
        }

#if  USE_SH_POOLS
  public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { Debug.Assert( size == sizeof(DrawableBSP) ); return MemAllocFS(pool);	};
	void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
	static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(DrawableBSP), 50, 0 ); };
	static void ReleaseStorage()	{ MemPoolFree( pool ); };
	static MEM_POOL	pool;
#endif
    }
}
