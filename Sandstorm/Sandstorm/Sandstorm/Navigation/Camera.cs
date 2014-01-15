using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Text;

namespace Sandstorm
{
    [Serializable] 
    public class Camera
    {
        static XmlSerializer _serializer = new XmlSerializer(typeof(Camera));

        public enum CameraMode
        {
            CAMERA_MODE_ORBIT,
            CAMERA_MODE_TURNTABLE,
            CAMERA_MODE_FIRST_PERSON,
            CAMERA_MODE_FLIGHT
        }

        public enum ProjectionType
        {
            PERSPECTIVE_PROJECTION,
            ORTHOGRAPHIC_PROJECTION
        }
        private Camera() { }

        private Vector3 WORLD_XAXIS = new Vector3(1f, 0f, 0f);
        private Vector3 WORLD_YAXIS = new Vector3(0f, 1f, 0f);
        private Vector3 WORLD_ZAXIS = new Vector3(0f, 0f, 1f);

        private float _farPlane = 10000f;
        private float _nearPlane = .1f;
        private float _fieldOfView = MathHelper.PiOver4;
        private float _orbitalDistance = 1000f;
        private Viewport _viewport;
        private CameraMode _mode = CameraMode.CAMERA_MODE_TURNTABLE;
        private Vector3 _lookAt = new Vector3(0f);
        private Vector3 _eyePos = new Vector3(0f);
        private Quaternion _orientation = new Quaternion(0f, 0f, 0f, 1f);
        private ProjectionType _type = ProjectionType.PERSPECTIVE_PROJECTION;
        private int _orthoWidth = 492;
        private int _orthoHeight = 492;

        private float _pitch;
        //private float _yaw;

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
                case CameraMode.CAMERA_MODE_TURNTABLE:
                    RotateTurntable(pYaw, pPitch);
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

        public void Horizontal(float factor)
        {
            Matrix trans = Matrix.CreateTranslation(factor, 0f, 0f);
            _viewMatrix = Matrix.Multiply(_viewMatrix, trans);
        }

        public void Vertical(float factor)
        {
            Matrix trans = Matrix.CreateTranslation(0f, factor, 0f);
            _viewMatrix = Matrix.Multiply(_viewMatrix, trans);
        }

        public void Zoom(float pFactor)
        {
            if (_type == ProjectionType.PERSPECTIVE_PROJECTION)
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

            Vector3 blub = _viewMatrix.Up;

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

            if (_mode == CameraMode.CAMERA_MODE_ORBIT || _mode == CameraMode.CAMERA_MODE_TURNTABLE)
            {
                //Calculate new eye position
                _eyePos = _lookAt + _viewMatrix.Forward * _orbitalDistance;
            }

            //Set the translation
            _viewMatrix.Translation = new Vector3(-Vector3.Dot(_viewMatrix.Right, _eyePos),
                                                  -Vector3.Dot(_viewMatrix.Up, _eyePos),
                                                  -Vector3.Dot(_viewMatrix.Forward, _eyePos));
        }


        public static Camera LoadCamera(ProjectionType pCamera,int width,int height)
        {
            switch (pCamera)
            {
                case ProjectionType.ORTHOGRAPHIC_PROJECTION:
                    {
                        if (File.Exists("ortho.xml"))
                            return readCamera("ortho.xml");
                        else
                        {
                            // Create orthographic camera for the beamer
                            Camera c = new Camera(new Viewport(0, 0, width, height));
                            //_orthoCamera.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2);
                            Quaternion rot1 = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2);
                            Quaternion rot2 = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), MathHelper.Pi);

                            c.Orientation = Quaternion.Multiply(rot1, rot2);
                            c.Type = Camera.ProjectionType.ORTHOGRAPHIC_PROJECTION;
                            return c;
                        }
                    }
                case ProjectionType.PERSPECTIVE_PROJECTION:
                    {
                        if (File.Exists("persp.xml"))
                            return readCamera("persp.xml");
                        else
                        {
                            // Create perspective camera for the editor
                            Camera c = new Camera(new Viewport(0, 0, width, height));
                            c.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver4);
                            return c;
                        }
                    }
                default:
                    break;
            }
            return null;
        }

        private static Camera readCamera(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            // Declare an object variable of the type to be deserialized.
            Camera i;

            // Use the Deserialize method to restore the object's state.
            i = (Camera)_serializer.Deserialize(reader);
            fs.Close();
            return i;
        }
        private static void writeCamera(string filename,Camera c)
        {
            Stream fs = new FileStream(filename, FileMode.Create);
            XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
            // Serialize using the XmlTextWriter.
            _serializer.Serialize(writer, c);
            writer.Close();
        }
        public static void saveCamera(ProjectionType pCamera,Camera c)
        {
            switch (pCamera)
            {
                case ProjectionType.ORTHOGRAPHIC_PROJECTION:
                    {
                        writeCamera("ortho.xml", c);
                    }
                    break;
                case ProjectionType.PERSPECTIVE_PROJECTION:
                    {
                        writeCamera("persp.xml", c);
                    }
                    break;
                default:
                    break;
            }
        }
    }


}
