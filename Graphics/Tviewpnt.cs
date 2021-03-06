using FalconNet.CampaignBase;
using FalconNet.Common.Graphics;
using System;
using System.Diagnostics;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        Tviewpnt.h
        Scott Randolph
        August 21, 1995

        This class represents a single viewer of the terrain database.  There
        may be multiple viewers simultainiously, each represented by their
        own instance of this class.
    \***************************************************************************/



    public class TpathFeature
    {
        public float[] x = new float[2];
        public float[] y = new float[2];
        public float width;
    }

    public struct TareaFeature
    {
        public float x, y;
        public float radius;
    }


    public class TViewPoint
    {

        public TViewPoint() { nLists = 0; }
        //public ~TViewPoint()	{ if (nLists != 0)  Cleanup(); };

        public void Setup(int minimumLOD, int maximumLOD, float[] fetchRanges)
        {
#if TODO
            Debug.Assert(minimumLOD >= 0);
            Debug.Assert(maximumLOD >= minimumLOD);

            minLOD = minimumLOD;
            maxLOD = maximumLOD;
            nLists = maxLOD + 1;		// Wastes extra array entries if minLOD != 0 (~ 50 bytes per)

            maxRange = new float[(maxLOD + 1)];
            if (maxRange == null)
            {
                throw new Exception("Failed memory allocation for viewer's range list");
            }
            memcpy(maxRange, fetchRanges, (maxLOD + 1) * sizeof(float));


            // Allocate memory for the array of block lists
            blockLists = new TBlockList[nLists];
            if (blockLists == null)
            {
                throw new Exception("Failed memory allocation for viewer's block list");
            }


            // Initialize the viewer's position to something rediculous to force a full update
            pos.x = -1e12f;
            pos.y = -1e12f;
            pos.z = -1e12f;

            // Initially enable all detail levels at once.  This will be adjusted by the first
            // call to UpdateViewpoint().
            highDetail = minLOD;
            lowDetail = maxLOD;

            Speed = 0.0; // JB 010610

            // Initialize each block list in turn
            for (int i = minLOD; i <= maxLOD; i++)
            {
                blockLists[i].Setup(TheMap.Level(i), fetchRanges[i]);
            }


            // Setup the critical section used to protect our active block lists
            InitializeCriticalSection(cs_update);
#endif
            throw new NotImplementedException();
        }

        public virtual void Cleanup()
        {
#if TODO
            // Ensure nobody is using this viewpoint while it is being destroyed
            EnterCriticalSection(cs_update);

            // Release all the block lists
            Debug.Assert(blockLists != null);
            for (int i = minLOD; i <= maxLOD; i++)
            {
                blockLists[i].Cleanup();
            }
            //TODO delete[] blockLists;

            // Release the range array memory
            //TODO delete[] maxRange;

            // Wipe out our private variables
            blockLists = null;
            maxRange = null;
            nLists = 0;

            // Release the critical section used to protect our active block lists
            LeaveCriticalSection(cs_update);
            DeleteCriticalSection(cs_update);
#endif
            throw new NotImplementedException();
        }

        public virtual bool IsReady() { return (nLists != 0); }
        ulong prevvuxGameTime;
        // Move the viewer and swap blocks as needed
        public void Update(Tpoint position)
        {
#if TODO
            float altAGL;
            int level;

            // Lock everyone else out of this viewpoint while it is being updated
            EnterCriticalSection(cs_update);

            // JB 010608 for weather effects start


            if (vuxGameTime != prevvuxGameTime)
            {
                float dist = (float)Math.Sqrt(((pos.x - position.x) * (pos.x - position.x) + (pos.y - position.y) * (pos.y - position.y)));
                dist = (float)Math.Sqrt(dist * dist + (pos.z - position.z) * (pos.z - position.z));

                Speed = dist * FT_TO_NM / ((vuxGameTime - prevvuxGameTime) / (3600000.0));
                prevvuxGameTime = vuxGameTime;
            }
            // JB 010608 for weather effects end

            // Store the viewer's position
            pos = position;


            // Ask the list at each level to do any required flushing and/or prefetching of blocks
            for (level = maxLOD; level >= minLOD; level--)
            {
                blockLists[level].Update(X(), Y());
            }


            // TODO:  FIX THIS CRITICAL SECTION STUFF (NO NESTING!!!)
            LeaveCriticalSection(cs_update);

            // Figure out the viewer's height above the terrain
            // At this point convert from Z down to positive altitude up so
            // that the following conditional tree is less confusing
            altAGL = GetGroundLevelApproximation(X(), Y()) - Z();

            // TODO:  FIX THIS CRITICAL SECTION STUFF (NO NESTING!!!)
            EnterCriticalSection(cs_update);



            // Adjust the range of active LODs based on altitude
            if (altAGL < 500.0f)
            {
                highDetail = minLOD;
                lowDetail = maxLOD - F4Config.g_nLowDetailFactor;
            }
            else if (altAGL < 6000.0f)
            {
                highDetail = minLOD;
                lowDetail = maxLOD;
            }
            else if (altAGL < 24000.0f)
            {
                highDetail = minLOD + 1;
                lowDetail = maxLOD;
            }
            else if (altAGL < 36000.0f)
            {
                highDetail = minLOD + 2;
                lowDetail = maxLOD;
            }
            else
            {
                highDetail = minLOD + 3;
                lowDetail = maxLOD;
            }


#if NOTHING
	// Disable the highest detail level
	if (highDetail < 2)		highDetail = 2;
#endif


            // Clamp the values to the avialable range of LODs
            if (lowDetail < minLOD) lowDetail = minLOD;
            if (lowDetail > maxLOD) lowDetail = maxLOD;
            if (highDetail > lowDetail) highDetail = lowDetail;
            if (highDetail < minLOD) highDetail = minLOD;

            // Unlock the viewpoint so others can query it
            LeaveCriticalSection(cs_update);
#endif
            throw new NotImplementedException();
        }


        // Return the Nth feature of the required type from the tile containing the viewpoint
        public int GetTileID(int r, int c)
        {
#if TODO
            Tpost post;
            const int LOD = TheMap.LastNearTexLOD();
            int texID;

            // Lock everyone else out of this viewpoint while we're using it
            EnterCriticalSection(cs_update);

            // See if we have the data we'll need
            if (blockLists[LOD].RangeFromCenter(r, c) >= blockLists[LOD].GetAvailablePostRange())
            {
                LeaveCriticalSection(cs_update);
                return -1;
            }

            // Get the terrain post governing the area of interest
            post = blockLists[LOD].GetPost(r, c);
            Debug.Assert(post != null);
            texID = post.texID;

            // Unlock the viewpoint
            LeaveCriticalSection(cs_update);

            return texID;
#endif
            throw new NotImplementedException();
        }
        public bool GetPath(int TileID, int type, int offset, TpathFeature target)
        {
#if TODO
            TexPath path;
            const int LOD = TheMap.LastNearTexLOD();

            Debug.Assert(texID >= 0);

            // Get the requested path feature in tile space
            path = TheTerrTextures.GetPath((TextureID)(texID), type, offset);
            if (!path)
            {
                return false;
            }

            // If we got a feature, convert it to world space units with 0,0 at lower left of the tile
            target.width = path.width;
            target.x[0] = (path.x1) * TheMap.Level(LOD).FTperPOST();
            target.x[1] = (path.x2) * TheMap.Level(LOD).FTperPOST();
            target.y[0] = (path.y1) * TheMap.Level(LOD).FTperPOST();
            target.y[1] = (path.y2) * TheMap.Level(LOD).FTperPOST();

            // Return success
            return true;
#endif
            throw new NotImplementedException();
        }
        public bool GetArea(int TileID, int type, int offset, out TareaFeature target)
        {
#if TODO
            TexArea area;
            const int LOD = TheMap.LastNearTexLOD();

            Debug.Assert(texID >= 0);

            // Get the requested area feature in tile space
            area = TheTerrTextures.GetArea(static_cast<TextureID>(texID), type, offset);
            if (!area)
            {
                return false;
            }

            // If we got a feature, convert it to world space units with 0,0 at lower left of the tile
            target.radius = area.radius;
            target.x = (area.x) * TheMap.Level(LOD).FTperPOST();
            target.y = (area.y) * TheMap.Level(LOD).FTperPOST();

            // Return success
            return true;
#endif
            throw new NotImplementedException();
        }

        // Return the min and max LODs ever useable by this viewpoint
        public int GetMinLOD() { return minLOD; }
        public int GetMaxLOD() { return maxLOD; }

        // Return the highest and lowest terrain elevation within range of this viewpoint
        public void GetAreaFloorAndCeiling(ref float floor, ref float ceiling)
        {
#if TODO  
            int level;
            float minZ;
            float maxZ;


            // Lock everyone else out of this viewpoint while we're using it
            EnterCriticalSection(cs_update);

            // Ask the list at each level for its min/max Z values
            minZ = 1e6f;
            maxZ = -1e6f;
            for (level = maxLOD; level >= minLOD; level--)
            {
                minZ = Math.Min(minZ, blockLists[level].GetMinZ());
                maxZ = Math.Max(maxZ, blockLists[level].GetMaxZ());
            }

            // Unlock the viewpoint
            LeaveCriticalSection(cs_update);

            // In case we don't have any terrain loaded yet...
            if (maxZ < minZ)
            {
                maxZ = minZ = 0.0f;
            }

            floor = maxZ;		// Remember, positive Z is downward in world space
            ceiling = minZ;	// Remember, positive Z is downward in world space
#endif
            throw new NotImplementedException();
        }

        // Return the low and high detail levels to be used for drawing	the next frame
        public int GetHighLOD() { return highDetail; }
        public int GetLowLOD() { return lowDetail; }

        // Return the largest distance from the viewer a post will ever want to be drawn
        // in world space and level post space
        public float GetMaxRange() { return maxRange[maxLOD]; }
        public float GetMaxRange(int LOD) { return maxRange[LOD]; }
#if TODO       
        public int GetMaxPostRange(int LOD) { return blockLists[LOD].GetMaxPostRange(); }

        // Return the maximum distance from the current postion at which all
        // data is owned by the specified LOD list in world space and in level posts
        public float GetAvailableRange() { return LEVEL_POST_TO_WORLD(blockLists[maxLOD].GetAvailablePostRange(), maxLOD); }
        public float GetAvailableRange(int LOD) { return LEVEL_POST_TO_WORLD(blockLists[LOD].GetAvailablePostRange(), LOD); }
        public int GetAvailablePostRange(int LOD) { return blockLists[LOD].GetAvailablePostRange(); }
#endif
        // Return the distance to the farthest piece of terrain that will be drawn in the next frame
        // (The .65 factor is a magic number that seems to work to account for the fact that
        //  the terrain engine leaves a safty margin of undrawn posts arround the viewpoint).
        public float GetDrawingRange()
        {
#if TODO
            return LEVEL_POST_TO_WORLD(blockLists[lowDetail].GetAvailablePostRange(), lowDetail) * 0.65f; 
#endif
            throw new NotImplementedException();
        }

        // Return a pointer to the requested post in the given level.
        // The caller of this function must ensure that the post is within
        // the available range.  Also, the post pointer may become invalid
        // after a call to "Update"
        public Tpost GetPost(int levelPostRow, int levelPostCol, int LOD)
        {
#if TODO
            return blockLists[LOD].GetPost(levelPostRow, levelPostCol);
#endif
            throw new NotImplementedException();
        }

        // Return the type of ground under the specified point on the ground
        // (requires terrain data including textures to be loaded at that point,
        //  otherwise, 0 is returned.)
        public int GetGroundType(float x, float y)
        {
#if TODO        
            TexArea area;
            TexPath path;
            Tpost post;
            TextureID texID;
            int row, col;
            float xPos, yPos;
            Edge segment;
            float dx, dy, d;
            float r;
            int i;
            int type = -1;
            const int LOD = TheMap.LastNearTexLOD();


            // Lock everyone else out of this viewpoint while we're using it
            EnterCriticalSection(cs_update);


            // Compute our row and column address and our offset into the tile
            row = WORLD_TO_LEVEL_POST(x, LOD);
            col = WORLD_TO_LEVEL_POST(y, LOD);
            xPos = x - LEVEL_POST_TO_WORLD(row, LOD);
            yPos = y - LEVEL_POST_TO_WORLD(col, LOD);
            Debug.Assert((xPos >= -0.5f) && (xPos <= LEVEL_POST_TO_WORLD(1, LOD) + 0.5f));
            Debug.Assert((yPos >= -0.5f) && (yPos <= LEVEL_POST_TO_WORLD(1, LOD) + 0.5f));


            // See if we have the data we'll need
            if (blockLists[LOD].RangeFromCenter(row, col) >= blockLists[LOD].GetAvailablePostRange())
            {
                LeaveCriticalSection(cs_update);
                return 0;
            }


            // Get the terrain post governing the area of interest
            post = blockLists[LOD].GetPost(row, col);
            Debug.Assert(post != null);
            texID = post.texID;


            // Check all segment features for inclusion
            i = 0;

            path = TheTerrTextures.GetPath(texID, 0, i++);
            while ((type == -1) && path)
            {

                r = path.width * 0.5f;

                // Skip this one if we're not inside its bounding box
                if (path.x2 < path.x1)
                {
                    if ((path.x2 > xPos + r) || (path.x1 < xPos - r))
                    {
                        path = TheTerrTextures.GetPath(texID, 0, i++);
                        continue;
                    }
                }
                else
                {
                    if ((path.x1 > xPos + r) || (path.x2 < xPos - r))
                    {
                        path = TheTerrTextures.GetPath(texID, 0, i++);
                        continue;
                    }
                }
                if (path.y2 < path.y1)
                {
                    if ((path.y2 > yPos + r) || (path.y1 < yPos - r))
                    {
                        path = TheTerrTextures.GetPath(texID, 0, i++);
                        continue;
                    }
                }
                else
                {
                    if ((path.y1 > yPos + r) || (path.y2 < yPos - r))
                    {
                        path = TheTerrTextures.GetPath(texID, 0, i++);
                        continue;
                    }
                }

                // Now check the line segment itself
                segment.SetupWithPoints(path.x1, path.y1, path.x2, path.y2);
                segment.Normalize();
                d = (float)fabs(segment.DistanceFrom(xPos, yPos));
                if (d < r)
                {
                    type = path.type;
                }
                path = TheTerrTextures.GetPath(texID, 0, i++);
            }

            // Check all area features for inclusion
            i = 0;
            area = TheTerrTextures.GetArea(texID, 0, i++);
            while ((type == -1) && area)
            {
                dx = xPos - area.x;
                dy = yPos - area.y;
                d = dx * dx + dy * dy;

                if (d < (area.radius * area.radius))
                {
                    type = area.type;
                }
                area = TheTerrTextures.GetArea(texID, 0, i++);
            }

            // Get the basic terrain type of this tile
            if (type == -1)
            {
                type = TheTerrTextures.GetTerrainType(texID);
            }

            // Unlock the viewpoint
            LeaveCriticalSection(cs_update);

            Debug.Assert(type != -1);

            return type;
#endif
            throw new NotImplementedException();
        }

        // Return the z value of the terrain at the specified point.  (positive Z down)
        // If the third argument is provided to the exact version, then the normal
        // will also be returned
        public float GetGroundLevelApproximation(float x, float y)
        {
#if TODO
            int LOD;
            int row;
            int col;
            Tpost* post;
            float elevation;


            // Compute the level relative post address of interest at the highest available LOD
            LOD = minLOD;
            row = FloatToInt32(x / TheMap.Level(LOD).FTperPOST());
            col = FloatToInt32(y / TheMap.Level(LOD).FTperPOST());

            // Lock everyone else out of this viewpoint while we're using it
            EnterCriticalSection(cs_update);

            // Figure out the highest detail level which has the required data available
            while (blockLists[LOD].RangeFromCenter(row, col) >= blockLists[LOD].GetAvailablePostRange())
            {
                row >>= 1;
                col >>= 1;
                LOD++;

                if (LOD > maxLOD)
                {
                    // Unlock the viewpoint
                    LeaveCriticalSection(cs_update);

                    return 0.0f;
                }
            }

            // Get the requested value
            post = blockLists[LOD].GetPost(row, col);
            if (post)
            {
                Debug.Assert(post);
                elevation = post.z;
            }
            else
            {
                elevation = 0.0F;
            }

            // Unlock the viewpoint
            LeaveCriticalSection(cs_update);

            return elevation;
#endif
            throw new NotImplementedException();
        }
        public float GetGroundLevel(float x, float y, Tpoint normal) //TODO default paramenter normal = null
        {
#if TODO
            int LOD;
            int row, col;
            float x_pos, y_pos;
            Tpost p1, p2, p3;
            float Nx, Ny, Nz;


            // Compute the level post address of the point of interest
            LOD = minLOD;
            row = WORLD_TO_LEVEL_POST(x, LOD);
            col = WORLD_TO_LEVEL_POST(y, LOD);

            // Lock everyone else out of this viewpoint while we're using it
            EnterCriticalSection(cs_update);

            // Figure out the highest detail level which has the required data available
            while (blockLists[LOD].RangeFromCenter(row, col) >= blockLists[LOD].GetAvailablePostRange())
            {
                row >>= 1;
                col >>= 1;
                LOD++;

                // See if we've run out of luck
                if (LOD > maxLOD)
                {

                    // Unlock the viewpoint
                    LeaveCriticalSection(cs_update);

                    // Return some default data
                    if (normal)
                    {
                        normal.x = 0.0f;
                        normal.y = 0.0f;
                        normal.z = 1.0f;
                    }
                    return 0.0f;
                }
            }

            // Compute the location of interest relative to the lower left bounding post
            x_pos = x - LEVEL_POST_TO_WORLD(row, LOD);
            y_pos = y - LEVEL_POST_TO_WORLD(col, LOD);
#if NOTHING
	// We occasionally run into precision problems here which cause the asserts
	// to fail.  The basic algorithm seems to be sound, however, and this is a
	// non-fatal condition, so these have been disabled.
	ShiAssert( x_pos >= -0.05f );	ShiAssert( x_pos <= TheMap.Level(LOD).FTperPOST() + 0.05f );
	ShiAssert( y_pos >= -0.05f );	ShiAssert( y_pos <= TheMap.Level(LOD).FTperPOST() + 0.05f );
#endif


            // Compute the normal from the three posts which bound the point of interest
            p1 = blockLists[LOD].GetPost(row, col);
            p3 = blockLists[LOD].GetPost(row + 1, col + 1);
            Debug.Assert(p1);
            Debug.Assert(p3);
            Nz = -TheMap.Level(LOD).FTperPOST();	// (remember positive Z is down)

            if (x_pos >= y_pos
                && p1 && p3) // JB 011019 CTD fix
            {
                // upper left triangle
                p2 = blockLists[LOD].GetPost(row + 1, col);
                Debug.Assert(p2);
                if (p2) // JB 011019 CTD fix
                {
                    Nx = p2.z - p1.z;		// (remember positive Z is down)
                    Ny = p3.z - p2.z;		// (remember positive Z is down)
                }
            }
            else
                if (p1 && p3) // JB 011019 CTD fix
                {
                    // lower right triangle
                    p2 = blockLists[LOD].GetPost(row, col + 1);
                    Debug.Assert(p2);
                    if (p2) // JB 011019 CTD fix
                    {
                        Nx = p3.z - p2.z;		// (remember positive Z is down)
                        Ny = p2.z - p1.z;		// (remember positive Z is down)
                    }
                }


            // Unlock the viewpoint
            LeaveCriticalSection(cs_update);


            // If the caller provided a place to store the normal, do it
            // NOTE: This vector is NOT a unit vector
            if (normal)
            {
                normal.x = Nx;
                normal.y = Ny;
                normal.z = Nz;
            }

            // Compute the z of the plane at the given x,y location using the
            // fact that the dot product between the normal and any vector in
            // the plane will be zero.
            // We choose (0,0,p1.z) and (xpos, ypos, Z) as two points to define
            // a line in the plane and dot that with the normal and solve for Z.
            if (p1) // JB 011019 CTD fix
                return p1.z - Nx / Nz * x_pos - Ny / Nz * y_pos;

            return 0; // JB 011019 CTD fix
#endif
            throw new NotImplementedException();
        }

        // Return true if the given point is on or under the terrain
        public bool UnderGround(Tpoint position)
        {
#if TODO
            return (position.z >= GetGroundLevel(position.x, position.y));
#endif
            throw new NotImplementedException();
        }


        // Return true if the two specified points can see each other over the terrain
        public bool LineOfSight(Tpoint p1, Tpoint p2)
        {
#if TODO
            int Px = 0, Py = 0;				// row and column to lower left of p1
            int Qx = 0, Qy = 0;				// row and column to lower left of p2
            int LOD_P = 0;				// Most detailed LOD which contains point P
            int LOD_Q = 0;				// Most detailed LOD which contains point Q
            float z = 0.0F, dz = 0.0F;		// Current LOS z and dz/per major step


            // Find the most detailed LOD which contains each point
            for (LOD_P = minLOD; LOD_P <= maxLOD; LOD_P++)
            {

                // Compute the coordinates of the lower left neighbor post
                Px = WORLD_TO_LEVEL_POST(p1.x, LOD_P);
                Py = WORLD_TO_LEVEL_POST(p1.y, LOD_P);

                // If the point lies within this level's range, stop looking
                if (blockLists[LOD_P].RangeFromCenter(Px, Py) <= blockLists[LOD_P].GetAvailablePostRange())
                {
                    break;
                }
            }
            for (LOD_Q = minLOD; LOD_Q <= maxLOD; LOD_Q++)
            {

                // Compute the coordinates of the lower left neighbor post
                Qx = WORLD_TO_LEVEL_POST(p2.x, LOD_Q);
                Qy = WORLD_TO_LEVEL_POST(p2.y, LOD_Q);

                // If the point lies within this level's range, stop looking
                if (blockLists[LOD_Q].RangeFromCenter(Qx, Qy) <= blockLists[LOD_Q].GetAvailablePostRange())
                {
                    break;
                }
            }

            // TODO:  make this work over multiple LODs
            // For now, use the highest detail which contains both points
            int LOD = max(LOD_Q, LOD_P);

            // TODO:  Clip the segment to the available map data
            // For now, if one or both of the points are entirely off our current map,
            // return false (can't see between points)
            if (LOD > maxLOD)
            {
                return false;
            }


            // Adjust the post coordinates as required
            if (LOD_P < LOD)
            {
                Px >>= LOD - LOD_P;
                Py >>= LOD - LOD_P;
            }
            if (LOD_Q < LOD)
            {
                Qx >>= LOD - LOD_Q;
                Qy >>= LOD - LOD_Q;
            }


            // Only check this LOD if at least one end point is "in" the terrain
            if ((p1.z > blockLists[LOD].GetMinZ()) || (p2.z > blockLists[LOD].GetMinZ()))
            {

                // Compute the z step rate
                z = p1.z;
                dz = (p2.z - p1.z) / Math.Max(Math.Abs(Qx - Px), Math.Abs(Qy - Py));

                /****************************************************\
                Don't know why, buy Leon reports empirically better
                results with the following factor included.  I'll
                hopefully look into it later, but for now...
                \****************************************************/
                dz *= 0.2f;

                // Call the single LOD Line of Sight function
                if (!SingleLODLineOfSight(Px, Py, Qx, Qy, z, dz, LOD))
                {
                    return false;
                }

            }

            return true;
#endif
            throw new NotImplementedException();
        }


        // Find the intersection with the terrain (return false if there isn't one)
        public bool GroundIntersection(Tpoint dir, Tpoint intersection)
        {
#if TODO
            int LOD;
            int range;
            bool stepUp, stepRt;
            int hStep, vStep;
            int endRow, endCol;
            int prevRow, prevCol;
            float dzdx, dzdy, dydx, dxdy;
            int row, col;
            int rowt, colt;
            float x, y, z;
            float xt, yt, zt;


            // Store parameters of the vector we're testing
            dzdx = dir.z / dir.x;
            dzdy = dir.z / dir.y;
            dydx = dir.y / dir.x;
            dxdy = dir.x / dir.y;
            stepUp = (dir.x > 0.0f);
            stepRt = (dir.y > 0.0f);
            hStep = stepRt ? 1 : -1;
            vStep = stepUp ? 1 : -1;

            // Lock everyone else out of this viewpoint while we're using it
            EnterCriticalSection(cs_update);


            // Start at the eyepoint at the highest drawn detail level
            row = WORLD_TO_LEVEL_POST(X(), highDetail);
            col = WORLD_TO_LEVEL_POST(Y(), highDetail);


            // Walk through all the LODs being drawn
            for (LOD = highDetail; LOD <= lowDetail; LOD++)
            {

                // Get the available data range at this LOD -- skip if we don't have anything available
                range = blockLists[LOD].GetAvailablePostRange() - 1;
                if (range <= 0)
                {
                    row >>= 1;
                    col >>= 1;
                    continue;
                }

                // Decide if we need to use x or y as the control variable
                if (Math.Abs(dir.x) > Math.Abs(dir.y))
                {
                    // North/South dominant vector

                    // Set up the stepping parameters
                    if (stepUp)
                    {
                        endRow = WORLD_TO_LEVEL_POST(X(), LOD) + range;
                        row += 1;
                    }
                    else
                    {
                        endRow = WORLD_TO_LEVEL_POST(X(), LOD) - range;
                    }

                    // Walk the line
                    while (row != endRow)
                    {

                        // Compute row/col for next check
                        x = LEVEL_POST_TO_WORLD(row, LOD);
                        prevCol = col;
                        y = Y() + dydx * (x - X());
                        z = Z() + dzdx * (x - X());
                        col = WORLD_TO_LEVEL_POST(y, LOD);

                        // Do vertical edge check if we've changed columns
                        if (col != prevCol)
                        {

                            // Compute row/col for the check
                            rowt = Math.Min(row, row - vStep);
                            colt = Math.Max(col, prevCol);
                            yt = LEVEL_POST_TO_WORLD(colt, LOD);
                            xt = X() + dxdy * (yt - Y());
                            zt = Z() + dzdy * (yt - Y());

                            // Check vertical edge we crossed
                            if (verticalEdgeTest(rowt, colt, xt, yt, zt, LOD))
                            {
                                LineSquareIntersection(rowt, colt - stepRt, dir, intersection, LOD);

                                // Unlock the viewpoint
                                LeaveCriticalSection(cs_update);

                                return true;
                            }

                        }

                        // Check horizontal edge between (row,col) and (row,col+1)
                        if (horizontalEdgeTest(row, col, x, y, z, LOD))
                        {
                            LineSquareIntersection(row - stepUp, col, dir, intersection, LOD);

                            // Unlock the viewpoint
                            LeaveCriticalSection(cs_update);

                            return true;
                        }


                        // Take on vertical step
                        row += vStep;
                    }
                }
                else
                {
                    // East/West dominant vector

                    // Set up the stepping parameters
                    if (stepRt)
                    {
                        endCol = WORLD_TO_LEVEL_POST(Y(), LOD) + range;
                        col += 1;
                    }
                    else
                    {
                        endCol = WORLD_TO_LEVEL_POST(Y(), LOD) - range;
                    }

                    // Walk the line
                    while (col != endCol)
                    {

                        // Compute row/col for next check
                        y = LEVEL_POST_TO_WORLD(col, LOD);
                        prevRow = row;
                        x = X() + dxdy * (y - Y());
                        z = Z() + dzdy * (y - Y());
                        row = WORLD_TO_LEVEL_POST(x, LOD);

                        // Do horizontal edge check if we've changed rows
                        if (row != prevRow)
                        {

                            // Compute row/col for next check
                            rowt = max(row, prevRow);
                            colt = min(col, col - hStep);
                            xt = LEVEL_POST_TO_WORLD(rowt, LOD);
                            yt = Y() + dydx * (xt - X());
                            zt = Z() + dzdx * (xt - X());

                            // Check horizontal edge between we crossed
                            if (horizontalEdgeTest(rowt, colt, xt, yt, zt, LOD))
                            {
                                LineSquareIntersection(rowt - stepUp, colt, dir, intersection, LOD);

                                // Unlock the viewpoint
                                LeaveCriticalSection(cs_update);

                                return true;
                            }

                        }

                        // Check vertical edge between (row,col) and (row+1,col)
                        if (verticalEdgeTest(row, col, x, y, z, LOD))
                        {
                            LineSquareIntersection(row, col - stepRt, dir, intersection, LOD);

                            // Unlock the viewpoint
                            LeaveCriticalSection(cs_update);

                            return true;
                        }


                        // Take a horizontal step
                        col += hStep;
                    }
                }


                // TODO:  Don't restart search at eyepoint each time
#if NOTHING
		// Convert the row/col address we were about to check into the next
		// lowest detail level for the next loop iteration
		// (backup one post for good measure)
		row = (row - vStep) >> 1;
		col = (col - hStep) >> 1;
#else
                // Restart the search at the eyepoint to avoid the case where
                // we might be above ground at one LOD, but under at the next.
                row = WORLD_TO_LEVEL_POST(X(), LOD + 1);
                col = WORLD_TO_LEVEL_POST(Y(), LOD + 1);
#endif
            }


            // Unlock the viewpoint
            LeaveCriticalSection(cs_update);

            // If we got here, we didn't find an intersection with the ground
            return false;
#endif
            throw new NotImplementedException();
        }



        // Get the position and orientation matrix for this viewpoint
        public float X() { return pos.x; }
        public float Y() { return pos.y; }
        public float Z() { return pos.z; }

        public void GetPos(ref Tpoint p) { p = pos; }

        public float Speed; // JB 010608 for weather effects


        // Line of Sight helpers
        private bool SingleLODLineOfSight(int Px, int Py, int Qx, int Qy, float z, float dz, int LOD)
        {
#if TODO
            int nr;					// remainder
            int deltax, deltay;		// Q.x - P.x, Q.y - P.y
            int k;					// loop invariant constant
            int row, col;			// Current row and column being checked
            bool hit;				// Flag to indicate a terrain hit


            // Initialize values used in the following interations
            deltax = Qx - Px;
            deltay = Qy - Py;
            hit = false;


            // Lock everyone else out of this viewpoint while we're using it
            EnterCriticalSection(cs_update);





            // For reference purposes, let theta be the angle from P to Q
            if ((deltax >= 0) && (deltay >= 0) && (deltay < deltax))
            {			// theta < 45
                OCTANT(row = Px + 1, col = Py, k = deltax - deltay, row < Qx, row++, nr += deltay, col++, TestEast, TestSouth);
            }
            else if ((deltax > 0) && (deltay >= 0) && (deltay >= deltax))
            {	// 45 <= theta < 90
                OCTANT(col = Py + 1, row = Px, k = deltay - deltax, col < Qy, col++, nr += deltax, row++, TestNorth, TestWest);
            }
            else if ((deltax <= 0) && (deltay >= 0) && (deltay > -deltax))
            {	// 90 <= theta < 135
                OCTANT(col = Py + 1, row = Px, k = deltay + deltax, col < Qy, col++, nr -= deltax, row--, TestSouth, TestWest);
            }
            else if ((deltax <= 0) && (deltay > 0) && (deltay <= -deltax))
            {	// 135 <= theta < 180
                OCTANT(row = Px - 1, col = Py, k = -deltax - deltay, row > Qx, row--, nr += deltay, col++, TestEast, TestNorth);
            }
            else if ((deltax <= 0) && (deltay <= 0) && (deltay > deltax))
            {	// 180 <= theta < 225
                OCTANT(row = Px - 1, col = Py, k = -deltax + deltay, row > Qx, row--, nr -= deltay, col--, TestWest, TestNorth);
            }
            else if ((deltax < 0) && (deltay <= 0) && (deltay <= deltax))
            {	// 225 <= theta < 270
                OCTANT(col = Py - 1, row = Px, k = -deltay + deltax, col > Qy, col--, nr -= deltax, row--, TestSouth, TestEast);
            }
            else if ((deltax >= 0) && (deltay <= 0) && (-deltay > deltax))
            {	// 270 <= theta < 315
                OCTANT(col = Py - 1, row = Px, k = -deltay - deltax, col > Qy, col--, nr += deltax, row++, TestNorth, TestWest);
            }
            else if ((deltax >= 0) && (deltay < 0) && (-deltay <= deltax))
            {	// 315 <= theta < 360
                OCTANT(row = Px + 1, col = Py, k = deltax + deltay, row < Qx, row++, nr -= deltay, col--, TestWest, TestSouth);
            }
            else
            {	// P == Q
            }


            // Unlock the viewpoint
            LeaveCriticalSection(cs_update);

            return (!hit);
#endif
            throw new NotImplementedException();
        }
        private void OCTANT(int f1, int f2, int f3, int f4, int f5, int i1, int s1, int r1, int r2)
        {
#if TODO		
	for (f1, f2, f3, nr=0; ((f4) && (!hit)); f5) {		
		z += dz;										
  		if (nr < k) {									
			if (i1) {									
				hit = r1(row,col,z,LOD);				
			} else {									
				hit = TestVertex(row,col,z,LOD);		
			}											
		} else {										
			s1;											
			if (nr -= k) {								
				hit  = r2(row,col,z,LOD);				
				if (!hit) {								
					hit = r1(row,col,z,LOD);			
				}										
			} else {									
				hit = TestVertex(row,col,z,LOD);		
			}											
		}												
	}
#endif
            throw new NotImplementedException();
        }
        private bool TestVertex(int row, int col, float z, int LOD)
        { throw new NotImplementedException(); }
        private bool TestEast(int row, int col, float z, int LOD)
        { throw new NotImplementedException(); }
        private bool TestNorth(int row, int col, float z, int LOD)
        { throw new NotImplementedException(); }
        private bool TestWest(int row, int col, float z, int LOD)
        { throw new NotImplementedException(); }
        private bool TestSouth(int row, int col, float z, int LOD)
        { throw new NotImplementedException(); }

        // Ground Intersection helpers
        private bool horizontalEdgeTest(int row, int col, float x, float y, float z, int LOD)
        {
#if TODO
            float t, height;
            Tpost left, right;

            // Get the relevant posts
            left = blockLists[LOD].GetPost(row, col);
            right = blockLists[LOD].GetPost(row, col + 1);

            // Compute the height of the edge at the point the line crosses it
            t = WORLD_TO_FLOAT_LEVEL_POST(y, LOD) - col;
            Debug.Assert((t > -0.1f) && (t < 1.1f)); // Make it more tolerant since it actually works anyway
            height = left.z + t * (right.z - left.z);

            // Return true if the line crosses below the edge (ie: is less negative)
            return (height < z);
#endif
            throw new NotImplementedException();
        }
        private bool verticalEdgeTest(int row, int col, float x, float y, float z, int LOD)
        {
#if TODO
            float t, height;
            Tpost top, bottom;

            // Get the relevant posts
            top = blockLists[LOD].GetPost(row + 1, col);
            bottom = blockLists[LOD].GetPost(row, col);

            // Compute the height of the edge at the point the line crosses it
            t = WORLD_TO_FLOAT_LEVEL_POST(x, LOD) - row;

            // OW
            //Debug.Assert( (t >= -0.1f) && (t <= 1.1f) );
            height = bottom.z + t * (top.z - bottom.z);

            // Return true if the line crosses below the edge (ie: is less negative)
            return (height < z);
#endif
            throw new NotImplementedException();
        }
        private void LineSquareIntersection(int row, int col, Tpoint dir, out Tpoint intersection, int LOD)
        {
#if TODO
            Tpost SW, NW, NE, SE;
            float Nx, Ny, Nz;
            float SWx, SWy, SWz;
            float PQdotN;
            float NdotDIR;
            float t;

            // Get the posts which bound this square
            SW = blockLists[LOD].GetPost(row, col);
            NW = blockLists[LOD].GetPost(row + 1, col);
            NE = blockLists[LOD].GetPost(row + 1, col + 1);
            SE = blockLists[LOD].GetPost(row, col + 1);
            Debug.Assert(SW && NW && NE && SE);

            // Store the world space location of the upper left and lower right corner posts
            SWx = LEVEL_POST_TO_WORLD(row, LOD);
            SWy = LEVEL_POST_TO_WORLD(col, LOD);
            SWz = SW.z;

            // Compute the normal from the three posts which bound the point of interest
            // (don't forget - positive Z is DOWN)
            Nz = -TheMap.Level(LOD).FTperPOST();

            // upper left triangle (remember positive Z is down)
            Nx = NW.z - SW.z;
            Ny = NE.z - NW.z;

            // Only check the first triangle if it is front facing relative to the test ray
            if (Nx * dir.x + Ny * dir.y + Nz * dir.z < 0.0f)
            {

                // Compute the intersection of the line with the plane
                PQdotN = (SWx - X()) * Nx + (SWy - Y()) * Ny + (SWz - Z()) * Nz;
                NdotDIR = Nx * dir.x + Ny * dir.y + Nz * dir.z;
                t = PQdotN / NdotDIR;

                intersection.x = X() + t * dir.x;
                intersection.y = Y() + t * dir.y;
                intersection.z = Z() + t * dir.z;

                // Return now if the intersection is within the upper left half space
                if ((intersection.x - SWx) >= (intersection.y - SWy))
                {
                    return;
                }

            }


            // upper right triangle (remember positive Z is down)
            Nx = NE.z - SE.z;
            Ny = SE.z - SW.z;

            // Compute the intersection of the line with the plane
            PQdotN = (SWx - X()) * Nx + (SWy - Y()) * Ny + (SWz - Z()) * Nz;
            NdotDIR = Nx * dir.x + Ny * dir.y + Nz * dir.z;
            t = PQdotN / NdotDIR;

            intersection.x = X() + t * dir.x;
            intersection.y = Y() + t * dir.y;
            intersection.z = Z() + t * dir.z;

            // Make sure the intersection we found is within the lower right half space
            // Rounding errors could make this assertion fail occasionally
            //	Debug.Assert( (intersection.x-SWx) <= (intersection.y-SWy) );

#endif
            throw new NotImplementedException();
        }



        // Last reported world space location of the viewer (X north, Y east, Z down)
        protected Tpoint pos;

        // Range sorted lists of pointers to data blocks at each map level
        // (level 0 is highest level of detail)
        protected int nLists;
        //TODO protected TBlockList blockLists;
        //TODO protected CRITICAL_SECTION cs_update;
        protected object cs_update;
        private void EnterCriticalSection(object o)
        { throw new NotImplementedException(); }
        private void InitializeCriticalSection(object o)
        { throw new NotImplementedException(); }
        private void LeaveCriticalSection(object o)
        { throw new NotImplementedException(); }
        private void DeleteCriticalSection(object o)
        { throw new NotImplementedException(); }


        // The farest into the distance this viewer should ever see (in world space)
        protected float[] maxRange;

        // The lowest and highest detail levels ever to be used by this viewpoint
        protected int minLOD;
        protected int maxLOD;

        // The lowest and highest detail levels currently turned on for drawing
        protected int highDetail;		// 0 <= highDetail <= lowDetail
        protected int lowDetail;		// higheDetail <= lowDetail <= nLevels-1;
    }

}

