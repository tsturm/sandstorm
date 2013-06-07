using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandstorm.ParticleSystem.structs;

namespace Sandstorm.ParticleSystem
{
    public class Particle
    {
        private Vector3 _pos;
        private Vector3 _oldpos;
        private Vector3 _force;
        private Vector3 _oldforce;
        static float _friction = 0.8f;//=0 -> no friction bounces like a Ball, =1 -> sticky like hell 
        static float _radius = 0.2f;
        static float _flexibitity = 0.8f;

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
        }

        public void applyExternalForce(Vector3 pForce)
        {
            this.Force += pForce;
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
                _oldforce = _force;
                Vector3 p2normal = this.Pos - p2.Pos;
                p2normal.Normalize();
                reflect(p2normal);
                applyFriction();
                applyExternalForce((p2normal * (1 / (this.Radius + p2.Radius))) * (1 - _flexibitity));
                applyExternalForce(p2.OldForce);
            }
        }

        public void collide(Particle[] p2)
        {
            foreach (Particle p in p2)
                collide(p);
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
