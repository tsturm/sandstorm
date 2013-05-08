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
        Vector3 _pos;
        Vector3 _oldpos;
        Vector3 _force;
        Vector3 _oldforce;
        static float _friction = 0.8f;//=0 -> no friction bounces like a Ball, =1 -> sticky like hell 
        static float _radius = 0.7f;
        static float _flexibitity = 0.8f;

        public Particle(Vector3 pPos, Vector3 pForce)
        {
            _pos = pPos;
            _oldpos = new Vector3(0, 0, 0);
            _force = pForce;
            _oldforce = new Vector3(0, 0, 0);
        }

        public void move()
        {
            this._oldpos = _pos;
            this._pos += this._force;
        }

        public void applyExternalForce(Vector3 pForce)
        {
            this._force += pForce;
        }

        override
        public string ToString()
        {
            return "Pos: " + this._pos + " force: " + this._force;
        }

        public Vector3 getPosition()
        {
            return this._pos;
        }

        public Vector3 getOldPosition()
        {
            return this._oldpos;
        }

        public Vector3 getForce()
        {
            return this._force;
        }

        public void setForce(Vector3 pForce)
        {
            this._force = pForce;
        }

        public Vector3 getOldForce()
        {
            return _oldforce;
        }

        public float getRadius()
        {
            return _radius;
        }

        public void setRadius(float radius)
        {
            _radius = radius;
        }

        public bool checkCollision(Particle p2)
        {
            if (p2 == this)
                return false;
            float distance = Vector3.Distance(this.getPosition(), p2.getPosition());
            if (distance <= this.getRadius() + p2.getRadius())
                return true;
            else
                return false;
        }

        public void collide(Particle p2)
        {
            if (checkCollision(p2))
            {
                _oldforce = _force;
                Vector3 p2normal = this.getPosition() - p2.getPosition();
                p2normal.Normalize();
                reflect(p2normal);
                applyFriction();
                applyExternalForce((p2normal * (1 / (this.getRadius() + p2.getRadius()))) * (1 - _flexibitity));
                applyExternalForce(p2.getOldForce() * (1 - _flexibitity));
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

        internal Particle Clone()
        {
            return new Particle(_pos, _force);
        }



        internal void setPosition(Vector3 pos)
        {
            _pos = pos;
        }
    }
}
