﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSGNet.Osg
{
    /// <summary>
    /// General purpose axis-aligned bounding box class for enclosing objects/vertices.
    /// Bounds leaf objects in a scene such as osg::Drawable objects. Used for frustum
    /// culling etc.
    /// </summary>
    public class BoundingBox
    {

        /// <summary>
        /// Creates an uninitialized bounding box.
        /// </summary>
        public BoundingBox()
        {
            _min = new Vec3f(float.MaxValue, float.MaxValue, float.MaxValue);
            _max = new Vec3f(float.MinValue, float.MinValue, float.MinValue);
        }

        /// <summary>
        /// Creates a bounding box initialized to the given extents.
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="ymin"></param>
        /// <param name="zmin"></param>
        /// <param name="xmax"></param>
        /// <param name="ymax"></param>
        /// <param name="zmax"></param>
        public BoundingBox(float xmin, float ymin, float zmin,
                           float xmax, float ymax, float zmax)
        {
            _min = new Vec3f(xmin, ymin, zmin);
            _max = new Vec3f(xmax, ymax, zmax);
        }

        /// <summary>
        /// Creates a bounding box initialized to the given extents.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public BoundingBox(Vec3f min, Vec3f max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Clear the bounding box. Erases existing minimum and maximum extents.
        /// </summary>
        public void Init()
        {
            _min = new Vec3f(float.MaxValue, float.MaxValue, float.MaxValue);
            _max = new Vec3f(float.MinValue, float.MinValue, float.MinValue);
        }

        /// <summary>
        /// Returns true if the bounding box extents are valid, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get { return _max.X >= _min.X && _max.Y >= _min.Y && _max.Z >= _min.Z; }
        }

        /// <summary>
        /// Sets the bounding box extents.
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="ymin"></param>
        /// <param name="zmin"></param>
        /// <param name="xmax"></param>
        /// <param name="ymax"></param>
        /// <param name="zmax"></param>
        public void Set(float xmin, float ymin, float zmin,
                           float xmax, float ymax, float zmax)
        {
            _min.Set(xmin, ymin, zmin);
            _max.Set(xmax, ymax, zmax);
        }

        /// <summary>
        /// Sets the bounding box extents.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void set(Vec3f min, Vec3f max)
        {
            _min = min;
            _max = max;
        }


        public float xMin { get { return _min.X; } }

        public float yMin { get { return _min.Y; } }

        public float zMin { get { return _min.Z; } }

        public float xMax { get { return _max.X; } }

        public float yMax { get { return _max.Y; } }

        public float zMax { get { return _max.Z; } }

        /// <summary>
        /// Calculates and returns the bounding box center.
        /// </summary>
        public Vec3f Center
        {
            get { return (_min + _max) * 0.5f; }
        }

        /// <summary>
        /// Calculates and returns the bounding box radius.
        /// </summary>
        public float Radius
        {
            get { return (float)Math.Sqrt(this.Radius2); }
        }

        /// <summary>
        /// Calculates and returns the squared length of the bounding box radius.
        /// Note, radius2() is faster to calculate than radius().
        /// </summary>
        public float Radius2
        {
            get { return 0.25f * ((_max - _min).Length2); }
        }

        /// <summary>
        /// Returns a specific corner of the bounding box.
        /// pos specifies the corner as a number between 0 and 7.
        /// Each bit selects an axis, X, Y, or Z from least- to
        /// most-significant. Unset bits select the minimum value
        ///  for that axis, and set bits select the maximum. 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vec3f Corner(int pos)
        {
            return new Vec3f((pos & 1) != 0 ? _max.X : _min.X, (pos & 2) != 0 ? _max.Y : _min.Y, (pos & 4) != 0 ? _max.Z : _min.Z);
        }

        /// <summary>
        /// Expands the bounding box to include the given coordinate.
        /// If the box is uninitialized, set its min and max extents to v.
        /// </summary>
        /// <param name="v"></param>
        public void ExpandBy(Vec3f v)
        {
            if (v.X < _min.X) _min.X = v.X;
            if (v.X > _max.X) _max.X = v.X;

            if (v.Y < _min.Y) _min.Y = v.Y;
            if (v.Y > _max.Y) _max.Y = v.Y;

            if (v.Z < _min.Z) _min.Z = v.Z;
            if (v.Z > _max.Z) _max.Z = v.Z;
        }

        /// <summary>
        /// Expands the bounding box to include the given coordinate.
        /// If the box is uninitialized, set its min and max extents to
        /// Vec3(x,y,z).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void ExpandBy(float x, float y, float z)
        {
            if (x < _min.X) _min.X = x;
            if (x > _max.X) _max.X = x;

            if (y < _min.Y) _min.Y = y;
            if (y > _max.Y) _max.Y = y;

            if (z < _min.Z) _min.Z = z;
            if (z > _max.Z) _max.Z = z;
        }

        /// <summary>
        /// Expands this bounding box to include the given bounding box.
        /// If this box is uninitialized, set it equal to bb. 
        /// </summary>
        /// <param name="bb"></param>
        public void ExpandBy(BoundingBox bb)
        {
            if (!bb.IsValid) return;

            if (bb._min.X < _min.X) _min.X = bb._min.X;
            if (bb._max.X > _max.X) _max.X = bb._max.X;

            if (bb._min.Y < _min.Y) _min.Y = bb._min.Y;
            if (bb._max.Y > _max.Y) _max.Y = bb._max.Y;

            if (bb._min.Z < _min.Z) _min.Z = bb._min.Z;
            if (bb._max.Z > _max.Z) _max.Z = bb._max.Z;
        }

        /// <summary>
        /// Expands this bounding box to include the given sphere.
        /// If this box is uninitialized, set it to include sh. 
        /// </summary>
        /// <param name="sh"></param>
        public void ExpandBy(BoundingSphere sh)
        {
            if (!sh.IsValid) return;

            if (sh.Center.X - sh.Radius < _min.X) _min.X = sh.Center.X - sh.Radius;
            if (sh.Center.X + sh.Radius > _max.X) _max.X = sh.Center.X + sh.Radius;

            if (sh.Center.Y - sh.Radius < _min.Y) _min.Y = sh.Center.Y - sh.Radius;
            if (sh.Center.Y + sh.Radius > _max.Y) _max.Y = sh.Center.Y + sh.Radius;

            if (sh.Center.Z - sh.Radius < _min.Z) _min.Z = sh.Center.Z - sh.Radius;
            if (sh.Center.Z + sh.Radius > _max.Z) _max.Z = sh.Center.Z + sh.Radius;
        }


        /// <summary>
        /// Returns the intersection of this bounding box and the specified bounding box.
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        public BoundingBox Intersect(BoundingBox bb)
        {
            return new BoundingBox(Math.Max(xMin, bb.xMin), Math.Max(yMin, bb.yMin), Math.Max(zMin, bb.zMin),
                                    Math.Min(xMax, bb.xMax), Math.Min(yMax, bb.yMax), Math.Min(zMax, bb.zMax));

        }

        /// <summary>
        /// Return true if this bounding box intersects the specified bounding box.
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        public bool intersects(BoundingBox bb)
        {
            return Math.Max(xMin, bb.xMin) <= Math.Min(xMax, bb.xMax) &&
                   Math.Max(yMin, bb.yMin) <= Math.Min(yMax, bb.yMax) &&
                   Math.Max(zMin, bb.zMin) <= Math.Min(zMax, bb.zMax);

        }


        /// <summary>
        /// Returns true if this bounding box contains the specified coordinate.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool Contains(Vec3f v)
        {
            return IsValid &&
                   (v.X >= _min.X && v.X <= _max.X) &&
                   (v.Y >= _min.Y && v.Y <= _max.Y) &&
                   (v.Z >= _min.Z && v.Z <= _max.Z);
        }

        #region Protected

        /// <summary>
        /// Minimum extent. (Smallest X, Y, and Z values of all coordinates.)
        /// </summary>
        protected Vec3f _min;
        /// <summary>
        /// Maximum extent. (Greatest X, Y, and Z values of all coordinates.)
        /// </summary>
        protected Vec3f _max;

        #endregion
    }
}
