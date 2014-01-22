using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Text;
using Sandstorm.Navigation;

namespace Sandstorm
{
    [Serializable] 
    public class Camera
    {
        private static Vector3 WORLD_XAXIS = new Vector3(1f, 0f, 0f);
        private static Vector3 WORLD_YAXIS = new Vector3(0f, 1f, 0f);
        private static Vector3 WORLD_ZAXIS = new Vector3(0f, 0f, 1f);
        
        private Camera() { }

        private CameraProperties _cameraSettings = null;
        public CameraProperties CameraSettings { get { return this._cameraSettings; } set { this._cameraSettings = value; } }

        private float _farPlane = 10000f;
        private float _nearPlane = .1f;
        private float _fieldOfView = MathHelper.PiOver4;
        private float _orbitalDistance = 1000f;
        private Viewport _viewport;
        private Vector3 _lookAt = new Vector3(0f);
        private Vector3 _eyePos = new Vector3(0f);
        private Quaternion _orientation = new Quaternion(0f, 0f, 0f, 1f);
        private int _orthoWidth = 492;
        private int _orthoHeight = 492;

        private float _pitch;

        public Camera(Viewport pViewPort,ProjectionType type,String cameraName = "Unknown")
        {
          //  this.CameraSettings = CameraProperties.Default.MemberwiseClone();
            this.CameraSettings = new CameraProperties();
            this.CameraSettings.ProjectionType = type;
            this.CameraSettings.CameraName = cameraName;

            _viewport = pViewPort;

            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        public Quaternion Orientation
        {
            get { return _orientation; }
            set { _orientation = value; UpdateViewMatrix(); }
        }

        public float FarPlane 
        {
            get { return _farPlane; }
            set { _farPlane = value; UpdateProjectionMatrix(); } 
        }

        public float NearPlane
        {
            get { return _nearPlane; }
            set { _nearPlane = value; UpdateProjectionMatrix(); }
        }

        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; UpdateProjectionMatrix(); }
        }

        public float OrbitalDistance
        {
            get { return _orbitalDistance; }
            set { _orbitalDistance = value; }
        }

        public Viewport Viewport
        {
            get { return _viewport; }
            set 
            { 
                _viewport = value; 
                UpdateProjectionMatrix(); 
            }
        }

        public void Rotate(float pYaw, float pPitch, float pRoll = 0f)
        {
	        switch (CameraSettings.CameraMode)
	        {
		        case CAMERA_MODE.CAMERA_MODE_ORBIT:
			        RotateOrbital(pYaw, pPitch);
			        break;
                case CAMERA_MODE.CAMERA_MODE_TURNTABLE:
                    RotateTurntable(pYaw, pPitch);
                    break;
                case CAMERA_MODE.CAMERA_MODE_FIRST_PERSON:
			        RotateFirstPerson(pYaw, pPitch);
			        break;
                case CAMERA_MODE.CAMERA_MODE_FLIGHT:
			        //TODO
			        break;
		        default:
			        break;
	        }

	        UpdateViewMatrix();
        }

        public void Horizontal(float factor)
        {
            Matrix trans = Matrix.CreateTranslation(factor, 0f, 0f);
            CameraSettings.ViewMatrix = Matrix.Multiply(CameraSettings.ViewMatrix, trans);
        }

        public void Vertical(float factor)
        {
            Matrix trans = Matrix.CreateTranslation(0f, factor, 0f);
            CameraSettings.ViewMatrix = Matrix.Multiply(CameraSettings.ViewMatrix, trans);
        }

        public void Zoom(float pFactor)
        {
            if (CameraSettings.ProjectionType == ProjectionType.PERSPECTIVE_PROJECTION)
            {
                _orbitalDistance += pFactor;
                UpdateViewMatrix();
            }
            else
            {
                _orthoHeight = _orthoWidth += (int)pFactor;
                UpdateProjectionMatrix();
            }
        }

        public void RotateOrbital(float pYaw, float pPitch)
        {
            float yawRad   = MathHelper.ToRadians(pYaw);
            float pitchRad = MathHelper.ToRadians(pPitch);

            Quaternion rot;

            if (yawRad != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_YAXIS, yawRad, out rot);
                Quaternion.Multiply(ref rot, ref _orientation, out _orientation);
            }

            if (pitchRad != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_XAXIS, pitchRad, out rot);
                Quaternion.Multiply(ref rot, ref _orientation, out _orientation);
            }
        }

        public void RotateTurntable(float pYaw, float pPitch)
        {
            _pitch += pPitch;

            if (_pitch > 70.0f)
            {
                pPitch = 70.0f - (_pitch - pPitch);
                _pitch = 70.0f;
            }
            else if (_pitch < 0.0f)
            {
                pPitch = 0.0f - (_pitch - pPitch);
                _pitch = 0.0f;
            }

            float yawRad = MathHelper.ToRadians(pYaw);
            float pitchRad = MathHelper.ToRadians(pPitch);

            Quaternion rot;

            Vector3 blub = CameraSettings.ViewMatrix.Up;

            if (yawRad != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref blub, yawRad, out rot);
                Quaternion.Multiply(ref rot, ref _orientation, out _orientation);
            }

            if (pitchRad != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_XAXIS, pitchRad, out rot);
                Quaternion.Multiply(ref rot, ref _orientation, out _orientation);
            }   
        }

        public void RotateFirstPerson(float pYaw, float pPitch)
        {
            _pitch += pPitch;

            if (_pitch > 90.0f)
            {
                pPitch = 90.0f - (_pitch - pPitch);
                _pitch = 90.0f;
            }

            if (_pitch < -90.0f)
            {
                pPitch = -90.0f - (_pitch - pPitch);
                _pitch = -90.0f;
            }

            float yawRad   = MathHelper.ToRadians(pYaw);
            float pitchRad = MathHelper.ToRadians(pPitch);

            Quaternion rot;

            if (yawRad != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_YAXIS, yawRad, out rot);
                Quaternion.Multiply(ref rot, ref _orientation, out _orientation);
            }

            if (pitchRad != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_XAXIS, pitchRad, out rot);
                Quaternion.Multiply(ref _orientation, ref rot, out _orientation);
            }
        }

        public void UpdateProjectionMatrix()
        {
            int width, height;

            if (_viewport.Width > _viewport.Height)
            {
                height = _orthoHeight;
                width = (_viewport.Width * height / _viewport.Height);
            }
            else if (_viewport.Width < _viewport.Height)
            {
                width = _orthoWidth;
                height = (_viewport.Height * width / _viewport.Width);
            }
            else
            {
                width = _orthoWidth;
                height = _orthoHeight;
            }

            switch (CameraSettings.ProjectionType)
            {
                case ProjectionType.PERSPECTIVE_PROJECTION:
                    Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _viewport.AspectRatio, _nearPlane, _farPlane, out CameraSettings._projMatrix);
                    break;
                case ProjectionType.ORTHOGRAPHIC_PROJECTION:
                    Matrix.CreateOrthographic(width, height, _nearPlane, _farPlane, out CameraSettings._projMatrix);
                    break;
            }
        }

        public void UpdateViewMatrix()
        {
            //Normalize orientation
            Quaternion.Normalize(ref _orientation, out _orientation);

            //Generate view matrix from orientation quaternion
            Matrix.CreateFromQuaternion(ref _orientation, out CameraSettings._viewMatrix);

            if (CameraSettings.CameraMode == CAMERA_MODE.CAMERA_MODE_ORBIT || CameraSettings.CameraMode == CAMERA_MODE.CAMERA_MODE_TURNTABLE)
            {
                //Calculate new eye position
                _eyePos = _lookAt + CameraSettings.ViewMatrix.Forward * _orbitalDistance;
            }

            //Set the translation
            CameraSettings._viewMatrix.Translation = new Vector3(-Vector3.Dot(CameraSettings.ViewMatrix.Right, _eyePos),
                                                  -Vector3.Dot(CameraSettings.ViewMatrix.Up, _eyePos),
                                                  -Vector3.Dot(CameraSettings.ViewMatrix.Forward, _eyePos));
        }
    }


}
