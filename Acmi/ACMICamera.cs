#define DO
using FalconNet.Common.Graphics;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acmi
{
    public class ACMICamera
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const int DETTACHED_CAM = 0;
        public const int ATTACHED_CAM = 1;

        public const int LOCAL_ROTATION = 0;
        public const int OBJECT_ROTATION = 1;

        public const int NUM_TRACKING_CAMS = 2;

        public const int NO_TRACKING = 0;
        public const int LOCAL_TRACKING = 1;
        public const int GLOBAL_TRACKING = 2;

        public const int NO_ACTION = 0;
        public const int ZOOM_IN = 1;
        public const int ZOOM_OUT = 2;
        public const int LOCAL_RIGHT_ROT = 3;
        public const int LOCAL_LEFT_ROT = 4;
        public const int LOCAL_UP_ROT = 5;
        public const int LOCAL_DOWN_ROT = 6;
        public const int OBJECT_RIGHT_ROT = 7;
        public const int OBJECT_LEFT_ROT = 8;
        public const int OBJECT_UP_ROT = 9;
        public const int OBJECT_DOWN_ROT = 10;
        public const int OBJECT_XRT_YUP_ROT = 11;
        public const int OBJECT_XLT_YDN_ROT = 12;
        public const int OBJECT_XRT_YDN_ROT = 13;
        public const int OBJECT_XLT_YUP_ROT = 14;
        public const int NO_ROTATION = 15;
        public const int HOME = 16;
        public const int ACMI_PANNER = 17;

        public const float HOME_RANGE = -300.0F;
        public ACMICamera()
        {
            _pos.x = 0.0F;
            _pos.y = 0.0F;
            _pos.z = 0.0F;

            _rot.M11 = 1.0F;
            _rot.M12 = 0.0F;
            _rot.M13 = 0.0F;
            _rot.M21 = 0.0F;
            _rot.M22 = 1.0F;
            _rot.M23 = 0.0F;
            _rot.M31 = 0.0F;
            _rot.M32 = 0.0F;
            _rot.M33 = 1.0F;
            _objectAz = _pannerAz = 0.0f;
            _objectEl = _pannerEl = 0.0f;

            // debugData = fopen("debugData.txt", "wb");
        }

        // Access.
        public void SetType(int type)
        {
            _type = type;
            _objectAz = 0.0F;
            _objectEl = 0.0F;
            _objectRoll = 0.0F;
            _localAz = 0.0F;
            _localEl = 0.0F;
            _objectRange = HOME_RANGE;
            _rotate = 0.0F;
            _azDir = 0.0F;
            _elDir = 0.0F;
            _slewRate = 0.1F;
            _tracking = NO_TRACKING;

            switch (_type)
            {
                case ATTACHED_CAM:
                    {
                        _rotType = OBJECT_ROTATION;
                        break;
                    }

                case DETTACHED_CAM:
                    {
                        _rotType = LOCAL_ROTATION;
                        break;
                    }
            }
        }
        public int Type()
        {
            return _type;
        }


        public void SetAction(int currentAction)
        {
            _action = currentAction;
        }

        public void SetAction(int currentAction, float az, float el)
        {
            _action = currentAction;
            SetAzDir(az);
            SetElDir(el);
        }

        public int Action()
        {
            return _action;
        }


        public void ToggleTracking()
        {
            /*
            if(_tracking >= NUM_TRACKING_CAMS)
            {
             _tracking = NO_TRACKING;
            }
            else
            {
             _tracking++;
            }
            */
            _tracking ^= 1;
        }

        public void SetTracking(int n)
        {
            _tracking = n;
        }

        public int Tracking()
        {
            return _tracking;
        }

        public void SetRotateType(int type)
        {
            //! _rotType = _type;
            _rotType = type;
        }

        //public int RotateType();

        public void SetPosition(Tpoint pos)
        {
            _pos.x = pos.x;
            _pos.y = pos.y;
            _pos.z = pos.z;
        }

        public void GetPosition(Tpoint pos)
        {
            pos.x = _pos.x;
            pos.y = _pos.y;
            pos.z = _pos.z;
        }

        public Tpoint Position()
        {
            return _pos;
        }

        public void SetRotation(Trotation rot)
        {
            _rot = rot;
        }

        public void GetRotation(out Trotation rot)
        {
            rot = _rot;
        }

        public Trotation Rotation()
        {
            return _rot;
        }

        public void SetWorldPosition(Tpoint pos)
        {
            _worldPos.x = pos.x;
            _worldPos.y = pos.y;
            _worldPos.z = pos.z;
        }

        public void SetElDir(float diff)
        {
            _elDir = diff;
        }

        public void SetAzDir(float diff)
        {
            _azDir = diff;
        }

        public void SetObjectEl(float diff)
        {
            _objectEl = diff;
        }

        public void SetObjectAz(float diff)
        {
            _objectAz = diff;
        }

        public void SetObjectRoll(float diff)
        {
            _objectRoll = diff;
        }

        public void SetPannerAz()
        {
            _pannerAz = _objectAz;
            _pannerEl = _objectEl;
        }

        public void IncrementPannerAzEl(int currentAction, float az, float el)
        {
            _action = currentAction;
            _objectAz = _pannerAz + az;
            _objectEl = _pannerEl + el;

            log.DebugFormat("PannA: {0}", _pannerAz);
            log.DebugFormat("PannE: {0}", _pannerEl);
            log.DebugFormat("ObjeA: {0}", _objectAz);
            log.DebugFormat("ObjeE: {0}", _objectEl);

            SetAzDir(0.0F);
            SetElDir(0.0F);
        }
        public void SetLocalEl(float diff)
        {
            _localEl = diff;
        }
        public void SetLocalAz(float diff)
        {
            _localAz = diff;
        }

        public float El()
        {
            return (_objectEl + _localEl);
        }

        public float Az()
        {
            return (_objectAz + _localAz);
        }

        public void SetObjectRange(float diff, int instruction)
        {
            switch (instruction)
            {
                case ZOOM_IN:
                    _objectRange = Math.Min(-50.0F, _objectRange + 50.0F);
                    break;

                case ZOOM_OUT:
                    _objectRange -= diff;
                    break;

                case HOME:
                    _objectRange = diff;
                    break;
            }

            SetRotateType(OBJECT_ROTATION);
        }

        public float GetObjectRange()
        {
            return _objectRange;
        }

        public void SetSlewRate(float diff)
        {
            _slewRate = diff;
        }
        public void TrackPoint(Tpoint trackingPt)
        {
            float
            trackingAz,
            trackingEl;

            float
            deltaX,
            deltaY,
            deltaZ,
            deltaRange;

            if (_tracking == NO_TRACKING)
                return;

            deltaX = trackingPt.x - (_worldPos.x + _pos.x);
            deltaY = trackingPt.y - (_worldPos.y + _pos.y);

            if (deltaX != 0)
                trackingAz = (float)Math.Atan2(deltaY, deltaX);
            else
                trackingAz = 0.0F;

            deltaZ = (_worldPos.z + _pos.z) - trackingPt.z;

#if DO
            deltaRange =
        (
            (float)Math.Sqrt
            (
                deltaX * deltaX +
                deltaY * deltaY +
                deltaZ * deltaZ
            )
        );

            if (deltaRange != 0.0F)
            {
                deltaRange = (deltaZ / deltaRange);
            }

            trackingEl = (float)Math.Asin(deltaRange);
#else
            trackingEl = (float)atan2(sqrt(deltaX * deltaX + deltaY * deltaY), deltaZ);
#endif

            /*
            switch(_tracking)
            {
             case LOCAL_TRACKING:
             {
             SetLocalAz((float)trackingAz);
             SetLocalEl((float)trackingEl);
             break;
             }
             case GLOBAL_TRACKING:
             {
             SetObjectAz((float)trackingAz);
             SetObjectEl((float)trackingEl - TRACKING_OFFSET);
             break;
             }
            }
            */
            SetLocalAz((float)trackingAz);
            SetLocalEl((float)trackingEl);
            // SetObjectAz((float)trackingAz);
            // SetObjectEl((float)trackingEl);
        }

        // Update methods.
        public void UpdatePosition()
        {
            Trotation tilt = new Trotation();

            DoAction();

            if (_rotType == LOCAL_ROTATION)
            {
                _localAz += _azDir * _slewRate;
                _localEl += _elDir * _slewRate;
                Rotate(_objectEl + _localEl, 0.0F, _objectAz + _localAz, ref _rot);
            }
            else
            {
                // Asmth and elevation angles
                _objectAz += _azDir * _slewRate;
                _objectEl += _elDir * _slewRate;
            }

            // Be around ownship, looking at it
            Tilt(_objectEl, 0.0F, _objectAz, ref tilt);
            Translate(_objectRange * tilt.M11, _objectRange * tilt.M12, _objectRange * tilt.M13, ref _pos);

            if (_tracking != 0)
                Rotate(_localEl, 0.0F, _localAz, ref _rot);
            else
                Rotate(_objectEl, _objectRoll, _objectAz, ref _rot);

        }

        public void UpdateChasePosition(float dT)
        {
            Tpoint dPos;
            float dRoll;
            float dist;

            DoAction();

            if (dT < 0.0f)
                dT = -dT;
            else if (dT > 0.0f)
                dT = dT;

            if (_rotType == LOCAL_ROTATION)
            {
                _localAz += _azDir * _slewRate;
                _localEl += _elDir * _slewRate;
            }
            else
            {
                // Asmth and elevation angles
                _objectAz += _azDir * _slewRate;
                _objectEl += _elDir * _slewRate;
            }

            // Be around ownship, looking at it
            // Tilt(_objectEl, 0.0F, _objectAz, &tilt);
            // Translate(_objectRange * tilt.M11, _objectRange * tilt.M12, _objectRange * tilt.M13, &_pos);

            /*
            if ( _tracking )
             Rotate(_localEl, 0.0F, _localAz, &_rot);
            else
             Rotate(_objectEl, _objectRoll, _objectAz, &_rot);
            */

            // "spring" constants for camera roll and move
            const float KMOVE = 0.29f;
            const float KROLL = 0.30f;

            // convert frame loop time to secs from ms
            // dT = (float)frameTime * 0.001;

            // get the diff between desired and current camera pos
            dPos.x = _chasePos.x - _pos.x;
            dPos.y = _chasePos.y - _pos.y;
            dPos.z = _chasePos.z - _pos.z;

            // send the camera thataway
            _pos.x += dPos.x * dT * KMOVE;
            _pos.y += dPos.y * dT * KMOVE;
            _pos.z += dPos.z * dT * KMOVE;

            // "look at" vector
            dPos.x = -_pos.x;
            dPos.y = -_pos.y;
            dPos.z = -_pos.z;

            // get new camera roll
            if (_objectRoll < -0.5f * (float)Math.PI && _chaseRoll > 0.5f * (float)Math.PI)
            {
                dRoll = _objectRoll + (2.0f * (float)Math.PI) - _chaseRoll;
            }
            else if (_objectRoll > 0.5f * (float)Math.PI && _chaseRoll < -0.5f * (float)Math.PI)
            {
                dRoll = _objectRoll - (0.5f * (float)Math.PI) - _chaseRoll;
            }
            else // same sign
            {
                dRoll = _objectRoll - _chaseRoll;
            }

            // apply roll
            _chaseRoll += dRoll * dT * KROLL;

            // keep chase cam roll with +/- 180
            if (_chaseRoll > 1.0f * (float)Math.PI)
                _chaseRoll -= 2.0f * (float)Math.PI;
            else if (_chaseRoll < -1.0f * (float)Math.PI)
                _chaseRoll += 2.0f * (float)Math.PI;

            // now get yaw and pitch based on look at vector
            dist = (float)Math.Sqrt(dPos.x * dPos.x + dPos.y * dPos.y + dPos.z * dPos.z);
            _objectEl = (float)-Math.Asin(dPos.z / dist);
            _objectAz = (float)Math.Atan2(dPos.y, dPos.x);
            Rotate(_objectEl, _chaseRoll, _objectAz, ref _rot);

        }

        public void UpdatePannerPosition()
        {
            Trotation tilt = new Trotation();

            Rotate(_objectEl, 0.0F, _objectAz, ref _rot);

            // Be around ownship, looking at it
            Tilt(_objectEl, 0.0F, _objectAz, ref tilt);
            Translate(_objectRange * tilt.M11, _objectRange * tilt.M12, _objectRange * tilt.M13, ref _pos);
        }


        public void Tilt(float pitch, float roll, float yaw, ref Trotation tilt)
        {

            float costha, sintha, cospsi, sinpsi;

            // Be around ownship, looking at it
            costha = (float)Math.Cos(pitch);
            sintha = (float)Math.Sin(pitch);
            cospsi = (float)Math.Cos(yaw);
            sinpsi = (float)Math.Sin(yaw);

            tilt.M11 = cospsi * costha;
            tilt.M12 = sinpsi * costha;
            tilt.M13 = -sintha;
        }

        public void Rotate(float pitch, float roll, float yaw, ref Trotation viewRotation)
        {
            float costha, sintha, cosphi, sinphi, cospsi, sinpsi;

            costha = (float)Math.Cos(pitch);
            sintha = (float)Math.Sin(pitch);
            cosphi = (float)Math.Cos(roll);
            sinphi = (float)Math.Sin(roll);
            cospsi = (float)Math.Cos(yaw);
            sinpsi = (float)Math.Sin(yaw);

            viewRotation.M11 = cospsi * costha;
            viewRotation.M21 = sinpsi * costha;
            viewRotation.M31 = -sintha;

            viewRotation.M12 = -sinpsi * cosphi + cospsi * sintha * sinphi;
            viewRotation.M22 = cospsi * cosphi + sinpsi * sintha * sinphi;
            viewRotation.M32 = costha * sinphi;

            viewRotation.M13 = sinpsi * sinphi + cospsi * sintha * cosphi;
            viewRotation.M23 = -cospsi * sinphi + sinpsi * sintha * cosphi;
            viewRotation.M33 = costha * cosphi;
        }

        public void Translate(float x, float y, float z, ref Tpoint camView)
        {
            camView.x = x;
            camView.y = y;
            camView.z = z;
        }

        public object /*CRITICAL_SECTION*/ criticalSection;

        public void SetChasePosition(Tpoint pos)
        {
            _chasePos.x = pos.x;
            _chasePos.y = pos.y;
            _chasePos.z = pos.z;
        }

        public void SetChaseRoll(float roll)
        {
            _chaseRoll = roll;
        }


        // Detached or attached?
        private int _type;

        // Detached or attached?
        private int _rotType;

        // Camera position relative to object.
        private Tpoint _pos;

        // World coords of camera position.
        private Tpoint _worldPos;

        // Camera rotation matrix.
        private Trotation _rot;

        private float _objectAz, _localAz, _pannerAz;
        private float _objectEl, _localEl, _pannerEl;
        private float _objectRoll;

        // For a dettached camera, set this value to 0.0F
        private float _objectRange;

        // Used to rotate around self.
        private float _rotate;

        // Used to rotate around object.
        float _objectRotate;

        private float _azDir;
        private float _elDir;
        private float _slewRate;
        private int _action;
        private int _tracking;
        private Tpoint _chasePos;
        private Tpoint _chaseObjPos;
        private float _chaseRoll;

        private void DoAction()
        {
            switch (_type)
            {
                case ATTACHED_CAM:
                    {
                        switch (_action)
                        {
                            case NO_ACTION:
                                {
                                    SetAzDir(0.0F);
                                    SetElDir(0.0F);
                                    SetRotateType(OBJECT_ROTATION);
                                    break;
                                }

                            case ZOOM_IN:
                                {
                                    SetObjectRange(0.0F, ZOOM_IN);
                                    break;
                                }

                            case ZOOM_OUT:
                                {
                                    SetObjectRange(10.0F, ZOOM_OUT);
                                    break;
                                }

                            case LOCAL_RIGHT_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetAzDir(1.0F);
                                    break;
                                }

                            case LOCAL_LEFT_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetAzDir(-1.0F);
                                    break;
                                }

                            case LOCAL_UP_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetElDir(1.0F);
                                    break;
                                }

                            case LOCAL_DOWN_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetElDir(-1.0F);
                                    break;
                                }

                            case OBJECT_RIGHT_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetAzDir(-1.0F);
                                    break;
                                }

                            case OBJECT_LEFT_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetAzDir(1.0F);
                                    break;
                                }

                            case OBJECT_UP_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetElDir(1.0F);
                                    break;
                                }

                            case OBJECT_DOWN_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetElDir(-1.0F);
                                    break;
                                }

                            case OBJECT_XRT_YUP_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetAzDir(-1.0F);
                                    SetElDir(1.0F);
                                    break;
                                }

                            case OBJECT_XLT_YDN_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetAzDir(1.0F);
                                    SetElDir(-1.0F);
                                    break;
                                }

                            case OBJECT_XRT_YDN_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetAzDir(-1.0F);
                                    SetElDir(-1.0F);
                                    break;
                                }

                            case OBJECT_XLT_YUP_ROT:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    SetAzDir(1.0F);
                                    SetElDir(1.0F);
                                    break;
                                }

                            case ACMI_PANNER:
                                {
                                    SetRotateType(OBJECT_ROTATION);
                                    break;
                                }
                        }

                        break;
                    }

                case DETTACHED_CAM:
                    {
                        switch (_action)
                        {
                            case NO_ACTION:
                                {
                                    SetAzDir(0.0F);
                                    SetElDir(0.0F);
                                    // SetRotateType(OBJECT_ROTATION);
                                    break;
                                }

                            /* case ZOOM_IN:
                             {
                             // SetObjectRange(0.0F, ZOOM_IN);
                             SetRotateType(NO_ROTATION);
                             break;
                             }
                             case ZOOM_OUT:
                             {
                             // SetObjectRange(10.0F, ZOOM_OUT);
                             SetRotateType(NO_ROTATION);
                             break;
                             }
                             case LOCAL_RIGHT_ROT:
                             {
                             SetRotateType(NO_ROTATION);
                             // SetAzDir(1.0F);
                             break;
                             }
                             case LOCAL_LEFT_ROT:
                             {
                             SetRotateType(NO_ROTATION);
                             // SetAzDir(-1.0F);
                             break;
                             }
                             case LOCAL_UP_ROT:
                             {
                             SetRotateType(NO_ROTATION);
                             // SetElDir(1.0F);
                             break;
                             }
                             case LOCAL_DOWN_ROT:
                             {
                             SetRotateType(NO_ROTATION);
                             // SetElDir(-1.0F);
                             break;
                             } */
                            case OBJECT_RIGHT_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetAzDir(-1.0F);
                                    break;
                                }

                            case OBJECT_LEFT_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetAzDir(1.0F);
                                    break;
                                }

                            case OBJECT_UP_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetElDir(1.0F);
                                    break;
                                }

                            case OBJECT_DOWN_ROT:
                                {
                                    SetRotateType(LOCAL_ROTATION);
                                    SetElDir(-1.0F);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }
    }
}
