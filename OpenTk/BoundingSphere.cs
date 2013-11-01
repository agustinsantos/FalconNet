
namespace OSGNet.Osg
{
    /// <summary>
    /// General purpose bounding sphere class for enclosing nodes/objects/vertices.
    /// Bounds internal osg::Nodes in the scene, assists in view frustum culling,
    /// etc. Similar in function to BoundingBox, it's quicker for evaluating
    ///  culling but generally will not cull as aggressively because it encloses a
    /// greater volume.
    /// </summary>
    public class BoundingSphere
    {
        /// <summary>
        /// Construct a default bounding sphere with radius to -1.0f, representing an invalid/unset bounding sphere.
        /// </summary>
        public BoundingSphere()
        {
            _center = new Vec3f();
            _radius = -1.0f;
        }

        /// <summary>
        /// Creates a bounding sphere initialized to the given extents.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public BoundingSphere(Vec3f center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        /// <summary>
        /// Creates a bounding sphere initialized to the given extents. 
        /// </summary>
        /// <param name="bs"></param>
        public BoundingSphere(BoundingSphere bs)
        {
            _center = bs._center;
            _radius = bs._radius;
        }

        /// <summary>
        /// Creates a bounding sphere initialized to the given extents.
        /// </summary>
        /// <param name="bb"></param>
        public BoundingSphere(BoundingBox bb)
        {
            ExpandBy(bb);
        }

        /// <summary>
        /// Clear the bounding sphere. Reset to default values.
        /// </summary>
        public void Init()
        {
            _center.Set(0.0f, 0.0f, 0.0f);
            _radius = -1.0f;
        }

        /// <summary>
        /// Returns true of the bounding sphere extents are valid, false
        ///  otherwise.
        /// </summary>
        public bool IsValid { get { return _radius >= 0.0f; } }

        /*  */
        /// <summary>
        /// Set the bounding sphere to the given center/radius using floats.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void Set(Vec3f center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        /// <summary>
        /// Returns the center of the bounding sphere.
        /// </summary>
        /// <returns></returns>
        public Vec3f Center
        {
            get { return _center; }
        }

        /// <summary>
        /// Returns the radius of the bounding sphere.
        /// </summary>
        /// <returns></returns>
        public float Radius
        {
            get { return _radius; }
        }

        /// <summary>
        /// Returns the squared length of the radius. Note, For performance
        ///  reasons, the calling method is responsible for checking to make
        ///  sure the sphere is valid.
        /// </summary>
        /// <returns></returns>
        public float Radius2 { get { return _radius * _radius; } }

        /// <summary>
        /// Expands the sphere to encompass the given point. Repositions the
        /// sphere center to minimize the radius increase. If the sphere is
        ///  uninitialized, set its center to v and radius to zero.
        /// </summary>
        /// <param name="?"></param>
        public void ExpandBy(Vec3f v)
        {
            if (this.IsValid)
            {
                Vec3f dv = v - _center;
                float r = dv.Length;
                if (r > _radius)
                {
                    float dr = (r - _radius) * 0.5f;
                    _center += dv * (dr / r);
                    _radius += dr;
                } // else do nothing as vertex is within sphere.
            }
            else
            {
                _center = v;
                _radius = 0.0f;
            }
        }

        /// <summary>
        /// Expands the sphere to encompass the given point. Does not
        /// reposition the sphere center. If the sphere is
        /// uninitialized, set its center to v and radius to zero.
        /// </summary>
        /// <param name="?"></param>
        public void ExpandRadiusBy(Vec3f v)
        {
            if (this.IsValid)
            {
                float r = (v - _center).Length;
                if (r > _radius) _radius = r;
                // else do nothing as vertex is within sphere.
            }
            else
            {
                _center = v;
                _radius = 0.0f;
            }
        }

        /// <summary>
        /// Expands the sphere to encompass the given sphere. Repositions the
        /// sphere center to minimize the radius increase. If the sphere is
        /// uninitialized, set its center and radius to match sh.
        /// </summary>
        /// <param name="sh"></param>
        public void ExpandBy(BoundingSphere sh)
        {
            // ignore operation if incomming BoundingSphere is invalid.
            if (!sh.IsValid) return;

            // This sphere is not set so use the inbound sphere
            if (!this.IsValid)
            {
                _center = sh._center;
                _radius = sh._radius;

                return;
            }

            // Calculate d == The distance between the sphere centers
            double d = (_center - sh.Center).Length;

            // New sphere is already inside this one
            if (d + sh.Radius <= _radius)
            {
                return;
            }

            //  New sphere completely contains this one
            if (d + _radius <= sh.Radius)
            {
                _center = sh._center;
                _radius = sh._radius;
                return;
            }

            // Build a new sphere that completely contains the other two:
            //
            // The center point lies halfway along the line between the furthest
            // points on the edges of the two spheres.
            //
            // Computing those two points is ugly - so we'll use similar triangles
            double new_radius = (_radius + d + sh.Radius) * 0.5;
            double ratio = (new_radius - _radius) / d;

            _center[0] += (float)((sh.Center[0] - _center[0]) * ratio);
            _center[1] += (float)((sh.Center[1] - _center[1]) * ratio);
            _center[2] += (float)((sh.Center[2] - _center[2]) * ratio);

            _radius = (float)new_radius;

        }

        /// <summary>
        /// Expands the sphere to encompass the given sphere. Does not
        ///  repositions the sphere center. If the sphere is
        /// uninitialized, set its center and radius to match sh.
        /// </summary>
        /// <param name="sh"></param>
        public void ExpandRadiusBy(BoundingSphere sh)
        {
            if (sh.IsValid)
            {
                if (this.IsValid)
                {
                    float r = (sh._center - _center).Length + sh._radius;
                    if (r > _radius) _radius = r;
                    // else do nothing as vertex is within sphere.
                }
                else
                {
                    _center = sh._center;
                    _radius = sh._radius;
                }
            }
        }


        /// <summary>
        /// Expands the sphere to encompass the given box. Repositions the
        /// sphere center to minimize the radius increase.
        /// </summary>
        /// <param name="bb"></param>
        public void ExpandBy(BoundingBox bb)
        {
            if (bb.IsValid)
            {
                if (this.IsValid)
                {
                    BoundingBox newbb = bb;

                    for (int c = 0; c < 8; ++c)
                    {
                        Vec3f v = bb.Corner(c) - _center; // get the direction vector from corner
                        v.Normalize(); // normalise it.
                        v *= -_radius; // move the vector in the opposite direction distance radius.
                        v += _center; // move to absolute position.
                        newbb.ExpandBy(v); // add it into the new bounding box.
                    }

                    _center = newbb.Center;
                    _radius = newbb.Radius;

                }
                else
                {
                    _center = bb.Center;
                    _radius = bb.Radius;
                }
            }
        }

        /// <summary>
        /// Expands the sphere to encompass the given box. Does not
        ///  repositions the sphere center.
        /// </summary>
        /// <param name="bb"></param>
        public void ExpandRadiusBy(BoundingBox bb)
        {
            if (bb.IsValid)
            {
                if (this.IsValid)
                {
                    for (int c = 0; c < 8; ++c)
                    {
                        ExpandRadiusBy(bb.Corner(c));
                    }
                }
                else
                {
                    _center = bb.Center;
                    _radius = bb.Radius;
                }
            }
        }



        /** Returns true if v is within the sphere. */
        public bool Contains(Vec3f v)
        {
            return this.IsValid && ((v - _center).Length2 <= Radius2);
        }

        /** Returns true if there is a non-empty intersection with the given
         * bounding sphere. */
        public bool Intersects(BoundingSphere bs)
        {
            return this.IsValid && bs.IsValid &&
                   ((_center - bs._center).Length2 <= (_radius + bs._radius) * (_radius + bs._radius));
        }


        #region Protected
        protected internal Vec3f _center;
        protected internal  float _radius = -1.0f;
        #endregion
    }
}
