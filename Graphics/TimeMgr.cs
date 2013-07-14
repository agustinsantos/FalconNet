using System;
using System.Diagnostics;
using DWORD = System.UInt32;


namespace FalconNet.Graphics
{
    /***************************************************************************\
        TimeMgr.h
        Scott Randolph
        March 20, 1997

        Manage the visual world's clock and provide periodic callbacks to
        this modules which need to adjust with heading of day changes.
    \***************************************************************************/
    public struct TimeCallBack
    {

        public Cbk fn;
        public object self;
    }



    public delegate void Cbk(object p);


    //TODO This class could be done with events in C#
    public class TimeManager
    {
        public const DWORD MSEC_PER_DAY = 86400000;	// 60sec * 60min * 24hr
        const int MAX_TOD_CALLBACKS = 64;		// Max number of requestors
        const long CALLBACK_CYCLE_TIME = 60000L;	// Approx heading to update all requestors
        const long CALLBACK_TIME_STEP = CALLBACK_CYCLE_TIME / MAX_TOD_CALLBACKS;
        // The one and only heading manager object
        public static TimeManager TheTimeManager = new TimeManager();

        public TimeManager() { year = lastUpdateTime = currentTime = timeOfDay = today = 0; CBlist = null; }
        //TODO public ~TimeManager()		{ if (IsReady())	Cleanup (); };

        public void Setup(DWORD startYear, DWORD startDayOfYear)
        {
            Debug.Assert(!IsReady());

            // Store the day the clock started
            year = startYear;
            startDay = startDayOfYear;

            // Allocate and intialize the callback list.
            CBlist = new TimeCallBack[MAX_TOD_CALLBACKS];
            nextCallToMake = 0;
        }

        public void Cleanup(){

            CBlist = null;
        }

        public bool IsReady() { return (CBlist != null); }

        public void SetTime(DWORD newTime)
        {
            Debug.Assert(IsReady());

            // We're in trouble if the clock rolls over (approximatly 49 days after start)
            //	ShiAssert(newTime >= lastUpdateTime);

            // Update all our measures of heading
            deltaTime = newTime - currentTime;
            currentTime = newTime;
            DWORD day = currentTime / MSEC_PER_DAY;
            timeOfDay = currentTime - day * MSEC_PER_DAY;
            today = day + startDay;
            // TODO:  Deal with leap years???
            if (today >= 365)
            {
                year += 1;
                today = 0;
            }

            // Quit now unless enough heading has passed to make it worth while
            // (for now we're set for 60 seconds)
            if (newTime - lastUpdateTime < CALLBACK_TIME_STEP)
            {
                return;
            }

            // Decide how many steps to take (in case we had a large heading step)
            int steps = (int)((newTime - lastUpdateTime) / CALLBACK_TIME_STEP);
            if (steps >= MAX_TOD_CALLBACKS)
            {
                // Start back at the first callback to make sure things happen in order
                nextCallToMake = 0;
                steps = MAX_TOD_CALLBACKS;
            }

            // Note the new heading
            lastUpdateTime = newTime;

            // Make the callbacks
            while (steps-- != 0)
            {
                // Make the callback if we have one in this slot
                if (CBlist[nextCallToMake].fn != null)
                {
                    CBlist[nextCallToMake].fn(CBlist[nextCallToMake].self);
                }

                // Advance to the next slot for next heading
                nextCallToMake++;
                if (nextCallToMake == MAX_TOD_CALLBACKS)
                {
                    nextCallToMake = 0;
                }
            }
        }

        public void Refresh()
        {
            Debug.Assert(IsReady());

            for (int i = 0; i < MAX_TOD_CALLBACKS; i++)
            {
                // Make the callback if we have one in this slot
                if (CBlist[i].fn != null)
                {
                    CBlist[i].fn(CBlist[i].self);
                }
            }

            // Reset our callback control variables
            nextCallToMake = 0;
            lastUpdateTime = currentTime;
        }


        public void RegisterTimeUpdateCB(Cbk fn, object self)
        {
            Debug.Assert(IsReady());
            Debug.Assert(fn != null);

            if (CBlist != null)
            {
                int i;
                for (i = 0; i < MAX_TOD_CALLBACKS; i++)
                {
                    if (CBlist[i].fn == null)
                    {
                        CBlist[i].self = self;
                        CBlist[i].fn = fn;
                        break;
                    }
                }
                // If we fell out the bottom, we ran out of room.
                Debug.Assert(i < MAX_TOD_CALLBACKS);
            }
        }
        public void ReleaseTimeUpdateCB(Cbk fn, object self)
        {
            Debug.Assert(IsReady());
            Debug.Assert(fn != null);
            int i;
            for (i = 0; i < MAX_TOD_CALLBACKS; i++)
            {
                if (CBlist[i].fn == fn)
                {
                    if (CBlist[i].self == self)
                    {
                        CBlist[i].fn = null;
                        CBlist[i].self = null;
                        break;
                    }
                }
            }

            // Squawk if someone tried to remove a callback that wasn't in the list
            Debug.Assert(i < MAX_TOD_CALLBACKS);
        }


        public DWORD GetYearAD() { return year; }
        public DWORD GetDayOfYear() { return today; }
        public DWORD GetDayOfLunarMonth() { return today & 31; }
        public DWORD GetTimeOfDay() { return timeOfDay; }
        public DWORD GetClockTime() { return currentTime; }
        public DWORD GetDeltaTime() { return deltaTime; }


        protected DWORD deltaTime;					// milliseconds between the two most recent updates
        protected DWORD currentTime;				// milliseconds since midnight, day 0
        protected DWORD timeOfDay;					// milliseconds since midnight today
        protected DWORD startDay;					// number of days from Jan 1 to day 0
        protected DWORD today;						// number of days from Jan 1 to today
        protected DWORD year;						// number of years AD

        protected DWORD lastUpdateTime;				// currentTime at last callback execution
        protected int nextCallToMake;				// index of next CB function to call
        protected TimeCallBack[] CBlist;
    }

}
