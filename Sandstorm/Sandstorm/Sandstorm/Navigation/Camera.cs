using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandstorm
{
    public class Camera
    {
        public enum CameraMode
        {
            CAMERA_MODE_ORBIT,
            CAMERA_MODE_FIRST_PERSON,
            CAMERA_MODE_FLIGHT
        }

        public enum ProjectionType
        {
            PERSPECTIVE_PROJECTION,
            ORTHOGRAPHIC_PROJECTION
        }

        private Vector3 WORLD_XAXIS = new Vector3(1f, 0f, 0f);
        private Vector3 WORLD_YAXIS = new Vector3(0f, 1f, 0f);
        private Vector3 WORLD_ZAXIS = new Vector3(0f, 0f, 1f);

        private float _farPlane = 10000f;
        private float _nearPlane = .1f;
        private float _fieldOfView = MathHelper.PiOver4;
        private float _orbitalDistance = 1000f;
        private Viewport _viewport;
        private CameraMode _mode = CameraMode.CAMERA_MODE_ORBIT;
        private Vector3 _lookAt = new Vector3(0f);
        private Vector3 _eyePos = new Vector3(0f);
        private Quaternion _orientation = new Quaternion(0f, 0f, 0f, 1f);
        private ProjectionType _type = ProjectionType.PERSPECTIVE_PROJECTION;
        private int _orthoWidth;
        private int _orthoHeight;

        private float _pitch;

        private Matrix _viewMatrix = Matrix.Identity;
        private Matrix _projMatrix = Matrix.Identity;

        public Camera(Viewport pViewPort)
        {
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

        public CameraMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public ProjectionType Type
        {
            get { return _type; }
            set { _type = value; UpdateProjectionMatrix(); }
        }

        public Matrix ViewMatrix
        {
            get { return _viewMatrix; }
            set { _viewMatrix = value; }
        }

        public Matrix ProjMatrix
        {
            get { return _projMatrix; }
            set { _projMatrix = value; }
        }

        public void Rotate(float pYaw, float pPitch, float pRoll = 0f)
        {
	        switch (_mode)
	        {
		        case CameraMode.CAMERA_MODE_ORBIT:
			        RotateOrbital(pYaw, pPitch);
			        break;
		        case CameraMode.CAMERA_MODE_FIRST_PERSON:
			        RotateFirstPerson(pYaw, pPitch);
			        break;
                case CameraMode.CAMERA_MODE_FLIGHT:
			        //TODO
			        break;
		        default:
			        break;
	        }

	        UpdateViewMatrix();
        }

        public void Zoom(float pFactor)
        {
            _orbitalDistance += pFactor;
            UpdateViewMatrix();
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
                height = 510;
                width = (_viewport.Width * height / _viewport.Height);
            }
            else if (_viewport.Width < _viewport.Height)
            {
                width = 510;
                height = (_viewport.Height * width / _viewport.Width);
            }
            else
            {
                width = 510;
                height = 510;
            }

            switch (_type)
            {
                case ProjectionType.PERSPECTIVE_PROJECTION:
                    Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _viewport.AspectRatio, _nearPlane, _farPlane, out _projMatrix);
                    break;
                case ProjectionType.ORTHOGRAPHIC_PROJECTION:
                    Matrix.CreateOrthographic(width, height, _nearPlane, _farPlane, out _projMatrix);
                    break;
            }
        }

        public void UpdateViewMatrix()
        {
            //Normalize orientation
            Quaternion.Normalize(ref _orientation, out _orientation);

            //Generate view matrix from orientation quaternion
            Matrix.CreateFromQuaternion(ref _orientation, out _viewMatrix);

            if (_mode == CameraMode.CAMERA_MODE_ORBIT)
            {
                //Calculate new eye position
                _eyePos = _lookAt + _viewMatrix.Forward * _orbitalDistance;
            }

            //Set the translation
            _viewMatrix.Translation = new Vector3(-Vector3.Dot(_viewMatrix.Right, _eyePos),
                                                  -Vector3.Dot(_viewMatrix.Up, _eyePos),
                                                  -Vector3.Dot(_viewMatrix.Forward, _eyePos));
        }
    }
}
