using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sandstorm
{
    class CameraController
    {
        private Camera _camera;
        private bool _mlb = false;
        private Vector2 _currentMousePos;
        private bool _msw = false;
        private int _scrollWheel;

        public CameraController(Camera pCamera)
        {
            _camera = pCamera;
        }

        public void Update(GameTime pGameTime)
        {
            float totalSeconds = (float)pGameTime.ElapsedGameTime.TotalSeconds;

            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected)
            {
                // TODO gamepad support
            }
            else
            {

                if (_camera.Type == Camera.ProjectionType.PERSPECTIVE_PROJECTION)
                {
                    // Handle mouse input
                    MouseState mouseState = Mouse.GetState();

                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (!_mlb)
                        {
                            _mlb = true;
                            _currentMousePos = new Vector2(mouseState.X, mouseState.Y);
                        }
                        else
                        {
                            Vector2 mPos = new Vector2(mouseState.X, mouseState.Y);
                            Vector2 delta = mPos - _currentMousePos;
                            _currentMousePos = mPos;

                            float yaw = delta.X;
                            float pitch = delta.Y;

                            _camera.Rotate(yaw, pitch);
                        }
                    }
                    else
                    {
                        _mlb = false;
                    }

                    //Zoom
                    if (_msw)
                    {
                        _msw = true;
                        _scrollWheel = mouseState.ScrollWheelValue;
                    }
                    else
                    {
                        int delta = _scrollWheel - mouseState.ScrollWheelValue;

                        _camera.Zoom(delta);

                        _scrollWheel = mouseState.ScrollWheelValue;
                    }

                }
                else
                {


                    KeyboardState keyState = Keyboard.GetState();

                    if (keyState.IsKeyDown(Keys.Left))
                    {
                        _camera.Horizontal(-.5f);
                    }
                    else if (keyState.IsKeyDown(Keys.Right))
                    {
                        _camera.Horizontal(.5f);
                    }
                    else if (keyState.IsKeyDown(Keys.Up))
                    {
                        _camera.Vertical(.5f);
                    }
                    else if (keyState.IsKeyDown(Keys.Down))
                    {
                        _camera.Vertical(-.5f);
                    }
                    else if (keyState.IsKeyDown(Keys.Add))
                    {
                        _camera.Zoom(-1f);
                    }
                    else if (keyState.IsKeyDown(Keys.Subtract))
                    {
                        _camera.Zoom(1f);
                    }
                }
            }
        }
    }
}
