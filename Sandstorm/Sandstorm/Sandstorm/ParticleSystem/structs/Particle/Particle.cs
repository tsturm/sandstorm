using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandstorm.ParticleSystem.structs;
using Sandstorm.Terrain;

namespace Sandstorm.ParticleSystem
{
    public class Particle
    {
        private Vector3 _pos;
        private Vector3 _oldpos;
        private Vector3 _force;
        private Vector3 _oldforce;
        private Vector3 _tmpforce;
        static float _friction = 0.1f;//=0 -> no friction bounces like a Ball, =1 -> sticky like hell 
        static float _radius = 2.5f;
        static float _flexibitity = 0.9f;

        public Vector3 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Vector3 Force
        {
            get { return _force; }
            set { _force = value; }
        }
        public Vector3 OldPos
        {
            get { return _oldpos; }
            set { _oldpos = value; }
        }
        public Vector3 OldForce
        {
            get { return _oldforce; }
            set { _oldforce = value; }
        }
        public Vector3 TmpForce
        {
            get { return _tmpforce; }
            set { _tmpforce = value; }
        }

        public float Radius
        {
            get { return _radius; }
        }

        public Particle()
        {
        }

        public static Particle getParticle(Vector3 pPos, Vector3 pForce)
        {
            Particle p = SharedList.FreeParticles.Get();
            p.Pos = pPos;
            p.OldPos = p.Pos;
            p.Force = pForce;
            p.OldForce = new Vector3(0, 0, 0);
            return p;
        }

        public void move()
        {
            this.OldPos = Pos;
            this.Pos += this.Force;
            this.Pos += this.TmpForce;
            this.TmpForce = new Vector3(0, 0, 0);
            _oldforce = _force + _tmpforce;
        }

        public void applyExternalForce(Vector3 pForce)
        {
            this.Force += pForce;
        }

        public void applyTemporalExternalForce(Vector3 pForce)
        {
            this.TmpForce += pForce;
        }

        override public string ToString()
        {
            return "Pos: " + this.Pos + " force: " + this.Force;
        }
        
        public bool checkCollision(Particle p2)
        {
            if (p2 == null)
                return false;
            if (p2 == this)
                return false;
            float distance = Vector3.Distance(this.Pos, p2.Pos);
            if (distance <= this.Radius + p2.Radius)
                return true;
            else
                return false;
        }

        public void collide(Particle p2)
        {
            if (checkCollision(p2))
            {
                Vector3 p2normal = this.Pos - p2.Pos;
                p2normal.Normalize();
                reflect(p2normal);

                applyTemporalExternalForce((p2normal * (1 / (this.Radius + p2.Radius))) * (1 - _flexibitity));
                
                applyExternalForce(p2.OldForce / 2);

                applyFriction();
            }
        }

        public void collide(Particle[] p2)
        {
            foreach (Particle p in p2)
                collide(p);
        }

        public bool checkCollision(HeightMap hm)
        {
            return Pos.Y < hm.getHeightData(Pos.X, Pos.Z) + Radius + 1;
        }

        public void collide(HeightMap hm)
        {
            if (checkCollision(hm))
            {
                Vector3 normal = hm.getNormal(Pos.X, Pos.Z);
                reflect(normal);
                Vector3 f = Force;
                f.Y = 0;
                Force = f;
                applyTemporalExternalForce(new Vector3(0, 0.1f, 0));
                applyFriction();
                
            }
        }

        public void reflect(Vector3 normal)
        {
            _force = (_force - ((2 * Vector3.Dot(_force, normal)) * normal));
        }

        public void applyFriction(float friction = -1)
        {
            if (friction < 0)
                friction = _friction;
            _force *= 1 - friction;
        }

        /*internal Particle Clone()
        {
            return new Particle(_pos, _force);
        }*/

    }
}
