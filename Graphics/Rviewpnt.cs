using System;
using System.Diagnostics;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        RViewPnt.h
        Scott Randolph
        August 20, 1996

        Manages information about a world space location and keeps the weather,
        terrain, and object lists in synch.
    \***************************************************************************/
    public struct ObjectListRecord
    {
        public ObjectDisplayList displayList;
        public float Ztop;
    };

    public class RViewPoint : TViewPoint
    {

        public RViewPoint()
        {
            objectLists = null;
            nObjectLists = 0;
            weather = null;
        }
        //public  ~RViewPoint()	{ Debug.Assert( !IsReady() ); };

        public void Setup(float gndRange, int maxDetail, int minDetail, float wxRange)
        { throw new NotImplementedException(); }

        public void Cleanup()
        { throw new NotImplementedException(); }

        public void SetGroundRange(float gndRange, int maxDetail, int minDetail)
        { throw new NotImplementedException(); }

        public void SetWeatherRange(float wxRange)
        { throw new NotImplementedException(); }

        public override bool IsReady()
        {
            return (objectLists != null) && (base.IsReady());
        }

        // Add/remove drawable objects from this viewpoint's display list
        public void InsertObject(DrawableObject object_)
        { throw new NotImplementedException(); }

        public void RemoveObject(DrawableObject object_)
        { throw new NotImplementedException(); }

        // Query terrain and weather properties for the area arround this viewpoint
        public float GetTerrainFloor()
        {
            return terrainFloor;
        }

        public float GetTerrainCeiling()
        {
            return terrainCeiling;
        }

        public float GetCloudBase()
        {
            return cloudBase;
        }

        public float GetCloudTops()
        {
            return cloudTops;
        }

        public float GetWeatherRange()
        {
            return weather != null ? weather.GetMaxRange() : 0.0f;
        }

        public float GetRainFactor()
        {
            return weather != null ? weather.GetRainFactor() : 0.0f;
        }

        public float GetVisibility()
        {
            return weather != null ? weather.GetVisibility() : 1.0f;
        }

        public bool GetLightning()
        {
            return weather != null ? weather.HasLightning() : false;
        }

        public float GetLocalCloudTops()
        {
            return weather != null ? weather.GetLocalCloudTops() : 0.0f;
        }

        // Cloud properties at this viewpoint's exact position in space
        public float CloudOpacity()
        {
            return cloudOpacity;
        }

        public Tcolor CloudColor()
        {
            return cloudColor;
        }

        // Ask if a line of sight exists between two points with respect to both clouds and terrain
        public float CompositLineOfSight(Tpoint p1, Tpoint p2)
        {
            if (LineOfSight(p1, p2))
            {
                if (weather != null)
                {
                    return weather.LineOfSight(p1, p2);
                }
                else
                {
                    return 1.0f;
                }
            }
            else
            {
                return 0.0f;
            }
        }

        public int CloudLineOfSight(Tpoint p1, Tpoint p2)
        {
            if (weather != null)
            {
                return (int)(weather.LineOfSight(p1, p2));
            }
            else
            {
                return 1;
            }
        }

        public void Update(Tpoint pos)
        {
#if TODO
            int i;
            DrawableObject first, p;
            TransportStr transList; // used to move DrawableObjects from one list to another
            float previousTop;

            Debug.Assert(IsReady());

            // Update the terrain center of attention
            base.Update(pos);
            base.GetAreaFloorAndCeiling(ref terrainFloor, ref terrainCeiling);

            // Update the weather
            if (weather != null)
            {
                weather.Update(pos, TheTimeManager.GetClockTime());
                cloudBase = weather.GetAreaFloor();
                cloudTops = weather.GetAreaCeiling();
            }
            roofHeight = -SKY_ROOF_HEIGHT;

            // For now we fix this here.  We might get smarter later...
            // (This comes into play when there is no data available for a weather tile)
            // Don't forget that -Z is up!
            //	if (cloudTops <= roofHeight)  cloudTops = roofHeight + 0.1f;

            // Update the ceiling values of the object display lists
            objectLists[0].Ztop = terrainCeiling;
            objectLists[1].Ztop = cloudBase;
            objectLists[2].Ztop = cloudTops;
            objectLists[3].Ztop = roofHeight;
            // objectLists[4].Ztop is always "infinity"
            //  JPO - clouds can be below terrain level now
            //	Debug.Assert( terrainCeiling > cloudBase );	// -Z is down - 
            Debug.Assert(cloudBase >= cloudTops);		// -Z is down

            previousTop = 1e12f;		// -Z is up
            for (i = 0; i < nObjectLists; i++)
            {
                transList.list[i] = null;
                transList.top[i] = objectLists[i].Ztop;
                transList.bottom[i] = previousTop;
                previousTop = transList.top[i];
            }
            transList.top[nObjectLists - 1] = -1e12f;

            // Update the membership of the altitude segregated object lists
            previousTop = 1e12f;		// -Z is up
            for (i = 0; i < nObjectLists; i++)
                objectLists[i].displayList.UpdateMetrics(i, pos, transList);


            for (i = 0; i < nObjectLists; i++)
            {
                // Put any objects moved into their new list
                first = transList.list[i];
                while (first != null)
                {
                    p = first;
                    first = first.next;
                    objectLists[i].displayList.InsertObject(p);
                }
            }

            // Resort each object list
            for (i = 0; i < nObjectLists; i++)
            {
                objectLists[i].displayList.SortForViewpoint();
            }


            // Resort the cloud object list
            cloudList.UpdateMetrics(pos);
            //	Debug.Assert( (!lowList) && (!highList) );
            cloudList.SortForViewpoint();


            // Decide if we're affected by the nearest cloud
            // TODO:  Would we win elsewhere if we set a bool flag?
            DrawableOvercast nearestCloud = (DrawableOvercast)cloudList.GetNearest();
            if (nearestCloud)
            {
                Debug.Assert(nearestCloud.GetClass() == DrawableObject.DrawClass.Overcast);
                cloudOpacity = nearestCloud.GetLocalOpacity(pos);
                nearestCloud.GetLocalColor(pos, ref cloudColor);
            }
            else
            {
                cloudOpacity = defcloudop;
            }
            if (cloudOpacity == 0) // Hack - set to haze color
                CTimeOfDay.TheTimeOfDay.GetVisColor(ref cloudColor);
#endif
            throw new NotImplementedException();
        }

        public void UpdateMoon()
        { throw new NotImplementedException(); }

        public void ResetObjectTraversal()
        { throw new NotImplementedException(); }

        public int GetContainingList(float zValue)
        { throw new NotImplementedException(); }

        public ObjectDisplayList ObjectsInTerrain()
        {
            return objectLists[0].displayList;
        }

        public ObjectDisplayList ObjectsBelowClouds()
        {
            return objectLists[1].displayList;
        }

        public ObjectDisplayList Clouds()
        {
            return cloudList;
        }

        public ObjectDisplayList ObjectsInClouds()
        {
            return objectLists[2].displayList;
        }

        public ObjectDisplayList ObjectsAboveClouds()
        {
            return objectLists[3].displayList;
        }

        public ObjectDisplayList ObjectsAboveRoof()
        {
            return objectLists[4].displayList;
        }

        public Texture SunTexture, GreenSunTexture;
        public Texture MoonTexture, GreenMoonTexture, OriginalMoonTexture;
        protected int lastDay;				// Used to decide when the moon needs updating

        protected LocalWeather weather;
        protected int nObjectLists;
        protected ObjectListRecord[] objectLists;
        protected ObjectDisplayList cloudList;
        protected float cloudOpacity;	// 0.0 for no effect, 1.0 when inCloud is TRUE
        protected Tcolor cloudColor;
        protected float terrainFloor;
        protected float terrainCeiling;
        protected float cloudBase;
        protected float cloudTops;
        protected float roofHeight;

        protected void SetupTextures()
        { throw new NotImplementedException(); }

        protected void ReleaseTextures()
        { throw new NotImplementedException(); }

        protected static void TimeUpdateCallback(object self)
        { throw new NotImplementedException(); }
    }
}

