namespace DesktopMascotMaker
{
    using OpenCvSharp;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using UnityEngine;

    [RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(Camera)), AddComponentMenu("DesktopMascotMaker/MascotMakerMulti")]
    public class MascotMakerMulti : MonoBehaviour
    {
        private byte opacity = 0xff;
        private static IntPtr mainWindowHandle = IntPtr.Zero;
        private bool isMouseHover;
        public bool PlayOnAwake = true;
        public bool TopMost = true;
        public UnityEngine.Vector2 MascotFormSize = new UnityEngine.Vector2(480f, 640f);
        private UnityEngine.Vector2 mascotFormSizePre = new UnityEngine.Vector2(480f, 640f);
        public AntiAliasingType AntiAliasing = AntiAliasingType.FourSamples;
        private AntiAliasingType antiAliasingPre = AntiAliasingType.FourSamples;
        public UpdateFuncType UpdateFunc = UpdateFuncType.Update;
        public bool ShowMascotFormOutline = false;
        private CvScalar frameColor;
        public bool ChromaKeyCompositing = false;
        public UnityEngine.Color ChromaKeyColor;
        public float ChromaKeyRange = 0.002f;
        private float chromaKeyRangePre;
        private RenderTexture mascotMakerTexture;
        private Material mascotMakerMaterial;
        private Material mascotMakerMaterialChromakey;
        private Texture2D cameraTexture;
        private Color32[] cameraTexturePixels;
        private GCHandle cameraTexturePixelsHandle;
        private IntPtr cameraTexturePixelsPtr;
        private MascotForm Form;
        [NonSerialized]
        private IplImage img = null;
        private Vector3 scaleVector;
        private bool isRender = false;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MouseEventHandler _LeftDoubleClick = null;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MouseEventHandler _RightDoubleClick = null;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MouseEventHandler _MiddleDoubleClick = null;
        private float LeftUpPreviousTime = float.MinValue;
        private float RightUpPreviousTime = float.MinValue;
        private float MiddleUpPreviousTime = float.MinValue;
        public bool DragMove = true;
        private int offsetX;
        private int offsetY;
        private bool isLeftMouseDown;
        private Camera cam;
        private Renderer rend;
        private System.Windows.Forms.Form formDummy;

        private event MouseEventHandler LeftDoubleClick
        {
            add
            {
                MouseEventHandler comparand = this._LeftDoubleClick;
                while (true)
                {
                    MouseEventHandler a = comparand;
                    comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._LeftDoubleClick, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                    if (ReferenceEquals(comparand, a))
                    {
                        return;
                    }
                }
            }
            remove
            {
                MouseEventHandler comparand = this._LeftDoubleClick;
                while (true)
                {
                    MouseEventHandler source = comparand;
                    comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._LeftDoubleClick, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                    if (ReferenceEquals(comparand, source))
                    {
                        return;
                    }
                }
            }
        }

        private event MouseEventHandler MiddleDoubleClick
        {
            add
            {
                MouseEventHandler comparand = this._MiddleDoubleClick;
                while (true)
                {
                    MouseEventHandler a = comparand;
                    comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._MiddleDoubleClick, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                    if (ReferenceEquals(comparand, a))
                    {
                        return;
                    }
                }
            }
            remove
            {
                MouseEventHandler comparand = this._MiddleDoubleClick;
                while (true)
                {
                    MouseEventHandler source = comparand;
                    comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._MiddleDoubleClick, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                    if (ReferenceEquals(comparand, source))
                    {
                        return;
                    }
                }
            }
        }

        private event MouseEventHandler RightDoubleClick
        {
            add
            {
                MouseEventHandler comparand = this._RightDoubleClick;
                while (true)
                {
                    MouseEventHandler a = comparand;
                    comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._RightDoubleClick, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                    if (ReferenceEquals(comparand, a))
                    {
                        return;
                    }
                }
            }
            remove
            {
                MouseEventHandler comparand = this._RightDoubleClick;
                while (true)
                {
                    MouseEventHandler source = comparand;
                    comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._RightDoubleClick, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                    if (ReferenceEquals(comparand, source))
                    {
                        return;
                    }
                }
            }
        }

        public event EventHandler OnActivated
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.Activated += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.Activated -= value;
                }
            }
        }

        public event EventHandler OnDeactivate
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.Deactivate += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.Deactivate -= value;
                }
            }
        }

        public event DragEventHandler OnDragDrop
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.DragDrop += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.DragDrop -= value;
                }
            }
        }

        public event DragEventHandler OnDragEnter
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.DragEnter += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.DragEnter -= value;
                }
            }
        }

        public event EventHandler OnDragLeave
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.DragLeave += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.DragLeave -= value;
                }
            }
        }

        public event DragEventHandler OnDragOver
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.DragOver += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.DragOver -= value;
                }
            }
        }

        public event KeyEventHandler OnKeyDown
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.KeyDown += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.KeyDown -= value;
                }
            }
        }

        public event KeyEventHandler OnKeyUp
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.KeyUp += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.KeyUp -= value;
                }
            }
        }

        public event MouseEventHandler OnLeftDoubleClick
        {
            add
            {
                if (this.Form != null)
                {
                    this._LeftDoubleClick += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this._LeftDoubleClick -= value;
                }
            }
        }

        public event MouseEventHandler OnLeftMouseDown
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.LeftMouseDown += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.LeftMouseDown -= value;
                }
            }
        }

        public event MouseEventHandler OnLeftMouseUp
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.LeftMouseUp += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.LeftMouseUp -= value;
                }
            }
        }

        public event MouseEventHandler OnMiddleDoubleClick
        {
            add
            {
                if (this.Form != null)
                {
                    this._MiddleDoubleClick += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this._MiddleDoubleClick -= value;
                }
            }
        }

        public event MouseEventHandler OnMiddleMouseDown
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.MiddleMouseDown += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.MiddleMouseDown -= value;
                }
            }
        }

        public event MouseEventHandler OnMiddleMouseUp
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.MiddleMouseUp += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.MiddleMouseUp -= value;
                }
            }
        }

        public event MouseEventHandler OnMouseWheel
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.MouseWheel += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.MouseWheel -= value;
                }
            }
        }

        public event EventHandler OnMove
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.Move += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.Move -= value;
                }
            }
        }

        public event MouseEventHandler OnRightDoubleClick
        {
            add
            {
                if (this.Form != null)
                {
                    this._RightDoubleClick += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this._RightDoubleClick -= value;
                }
            }
        }

        public event MouseEventHandler OnRightMouseDown
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.RightMouseDown += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.RightMouseDown -= value;
                }
            }
        }

        public event MouseEventHandler OnRightMouseUp
        {
            add
            {
                if (this.Form != null)
                {
                    this.Form.RightMouseUp += value;
                }
            }
            remove
            {
                if (this.Form != null)
                {
                    this.Form.RightMouseUp -= value;
                }
            }
        }

        private void Awake()
        {
            if (mainWindowHandle == IntPtr.Zero)
            {
                SetForegroundWindow(Process.GetCurrentProcess().Handle);
                mainWindowHandle = GetActiveWindow();
            }
            this.cam = base.transform.GetComponent<Camera>();
            this.rend = base.transform.GetComponent<Renderer>();
            this.cam.clearFlags = CameraClearFlags.Color;
            this.Form = new MascotForm();
            this.Form.Show();
            this.formDummy = new System.Windows.Forms.Form();
            this.formDummy.FormBorderStyle = FormBorderStyle.None;
            this.formDummy.Opacity = 0.0;
            this.formDummy.Show();
            base.Invoke("closeDummy", 0.01f);
            if (this.PlayOnAwake)
            {
                this.isRender = true;
            }
            else
            {
                this.isRender = false;
                this.Form.Hide();
            }
            this.mascotMakerMaterial = (Material) Resources.Load("MascotMakerMaterial", typeof(Material));
            this.mascotMakerMaterialChromakey = (Material) Resources.Load("MascotMakerMaterialChromakey", typeof(Material));
            this.mascotMakerTexture = new RenderTexture((int) this.MascotFormSize.x, (int) this.MascotFormSize.y, 0x18, RenderTextureFormat.ARGB32);
            this.mascotMakerTexture.antiAliasing = (int) this.AntiAliasing;
            this.mascotFormSizePre = this.MascotFormSize;
            this.antiAliasingPre = this.AntiAliasing;
            this.ChromaKeyColor = new UnityEngine.Color(this.ChromaKeyColor.r, this.ChromaKeyColor.g, this.ChromaKeyColor.b, 0f);
            this.cam.backgroundColor = new UnityEngine.Color(this.ChromaKeyColor.r, this.ChromaKeyColor.g, this.ChromaKeyColor.b, 0f);
            this.mascotMakerMaterialChromakey.color = new UnityEngine.Color(this.ChromaKeyColor.r, this.ChromaKeyColor.g, this.ChromaKeyColor.b, 0f);
            this.ChromaKeyRange = Mathf.Clamp(this.ChromaKeyRange, 0.002f, 1f);
            this.chromaKeyRangePre = this.ChromaKeyRange;
            this.mascotMakerMaterialChromakey.SetFloat("_Amount", this.chromaKeyRangePre);
            this.rend.material = !this.ChromaKeyCompositing ? this.mascotMakerMaterial : this.mascotMakerMaterialChromakey;
            this.rend.sharedMaterial.mainTexture = this.mascotMakerTexture;
            this.cameraTexture = new Texture2D(this.mascotMakerTexture.width, this.mascotMakerTexture.height, TextureFormat.ARGB32, false);
            this.cam.targetTexture = this.mascotMakerTexture;
            CvSize size = new CvSize(this.mascotMakerTexture.width, this.mascotMakerTexture.height);
            this.img = Cv.CreateImage(size, BitDepth.U8, 4);
            this.scaleVector = new Vector3(1f, -1f, 1f);
            this.isMouseHover = false;
            this.Form.LeftMouseDown += new MouseEventHandler(this.LeftMouseDown);
            this.Form.LeftMouseUp += new MouseEventHandler(this.LeftMouseUp);
            this.Form.RightMouseUp += new MouseEventHandler(this.RightMouseUp);
            this.Form.MiddleMouseUp += new MouseEventHandler(this.MiddleMouseUp);
            this.offsetX = 0;
            this.offsetY = 0;
            this.isLeftMouseDown = false;
            this.frameColor = new CvScalar(255.0, 0.0, 0.0, 128.0);
        }

        private void ChangeRenderTexture(UnityEngine.Vector2 renderTextureSize, AntiAliasingType antiAliasingType)
        {
            if (this.mascotMakerTexture != null)
            {
                this.cam.targetTexture = null;
                this.mascotMakerTexture.Release();
            }
            this.mascotMakerTexture = new RenderTexture((int) renderTextureSize.x, (int) renderTextureSize.y, 0x18, RenderTextureFormat.ARGB32);
            this.mascotMakerTexture.antiAliasing = (int) antiAliasingType;
            this.mascotFormSizePre = renderTextureSize;
            this.antiAliasingPre = antiAliasingType;
            this.rend.sharedMaterial.mainTexture = this.mascotMakerTexture;
            this.cameraTexture.Resize((int) renderTextureSize.x, (int) renderTextureSize.y, TextureFormat.ARGB32, false);
            this.cam.targetTexture = this.mascotMakerTexture;
            CvSize size = new CvSize((int) renderTextureSize.x, (int) renderTextureSize.y);
            this.img = Cv.CreateImage(size, BitDepth.U8, 4);
            this.Form.Size = new Size((int) renderTextureSize.x, (int) renderTextureSize.y);
        }

        private void closeDummy()
        {
            if (this.formDummy != null)
            {
                this.formDummy.Close();
                this.formDummy = null;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        public void Hide()
        {
            if (this.isRender)
            {
                this.isRender = false;
                this.Form.Hide();
            }
        }

        private void LateUpdate()
        {
            if (this.UpdateFunc == UpdateFuncType.LateUpdate)
            {
                this.UpdateCore();
            }
        }

        private void LeftMouseDown(object sender, MouseEventArgs e)
        {
            this.offsetX = this.Form.Left - System.Windows.Forms.Cursor.Position.X;
            this.offsetY = this.Form.Top - System.Windows.Forms.Cursor.Position.Y;
            this.isLeftMouseDown = true;
        }

        private void LeftMouseUp(object sender, MouseEventArgs e)
        {
            if (((Time.time - this.LeftUpPreviousTime) < 0.4f) && (this._LeftDoubleClick != null))
            {
                this._LeftDoubleClick(this.Form, new MouseEventArgs(MouseButtons.Left, 2, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0));
            }
            this.LeftUpPreviousTime = Time.time;
            this.isLeftMouseDown = false;
        }

        private void MiddleMouseUp(object sender, MouseEventArgs e)
        {
            if (((Time.time - this.MiddleUpPreviousTime) < 0.4f) && (this._MiddleDoubleClick != null))
            {
                this._MiddleDoubleClick(this.Form, new MouseEventArgs(MouseButtons.Middle, 2, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0));
            }
            this.MiddleUpPreviousTime = Time.time;
        }

        private void OnApplicationQuit()
        {
            this.isRender = false;
            if (this.formDummy != null)
            {
                this.formDummy.Close();
                this.formDummy = null;
            }
            if (this.Form != null)
            {
                this.Form.Close();
                this.Form = null;
            }
            UnregisterClass("Mono.WinForms.1.0", IntPtr.Zero);
        }

        private void OnDestroy()
        {
            if (this.formDummy != null)
            {
                this.formDummy.Close();
                this.formDummy = null;
            }
            if (this.Form != null)
            {
                this.Form.Close();
                this.Form = null;
            }
            if (this.rend.sharedMaterial.mainTexture != null)
            {
                this.rend.sharedMaterial.mainTexture = null;
            }
        }

        private void OnPostRender()
        {
            GL.invertCulling = false;
        }

        private void OnPreCull()
        {
            this.cam.ResetWorldToCameraMatrix();
            this.cam.ResetProjectionMatrix();
            this.cam.projectionMatrix *= Matrix4x4.Scale(this.scaleVector);
        }

        private void OnPreRender()
        {
            GL.invertCulling = true;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (this.ChromaKeyCompositing)
            {
                if (this.mascotMakerMaterialChromakey != null)
                {
                    UnityEngine.Graphics.Blit(source, destination, this.mascotMakerMaterialChromakey);
                }
            }
            else if (this.mascotMakerMaterial != null)
            {
                UnityEngine.Graphics.Blit(source, destination, this.mascotMakerMaterial);
            }
        }

        private void RightMouseUp(object sender, MouseEventArgs e)
        {
            if (((Time.time - this.RightUpPreviousTime) < 0.4f) && (this._RightDoubleClick != null))
            {
                this._RightDoubleClick(this.Form, new MouseEventArgs(MouseButtons.Right, 2, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0));
            }
            this.RightUpPreviousTime = Time.time;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        public void Show()
        {
            if (!this.isRender)
            {
                this.isRender = true;
                this.Form.Show();
            }
        }

        [DllImport("user32.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        private static extern short UnregisterClass(string lpClassName, IntPtr hInstance);
        private void Update()
        {
            if (this.UpdateFunc == UpdateFuncType.Update)
            {
                this.UpdateCore();
            }
        }

        private void UpdateCore()
        {
            if (this.isRender)
            {
                if ((this.mascotFormSizePre != this.MascotFormSize) || (this.antiAliasingPre != this.AntiAliasing))
                {
                    if (this.MascotFormSize.x < 1f)
                    {
                        this.MascotFormSize = new UnityEngine.Vector2(1f, (float) ((int) this.MascotFormSize.y));
                    }
                    if (this.MascotFormSize.y < 1f)
                    {
                        this.MascotFormSize = new UnityEngine.Vector2((float) ((int) this.MascotFormSize.x), 1f);
                    }
                    int antiAliasing = (int) this.AntiAliasing;
                    if ((antiAliasing != 1) && ((antiAliasing != 2) && ((antiAliasing != 4) && (antiAliasing != 8))))
                    {
                        this.AntiAliasing = AntiAliasingType.None;
                    }
                    this.ChangeRenderTexture(this.MascotFormSize, this.AntiAliasing);
                }
                if ((this.ChromaKeyColor.r != this.cam.backgroundColor.r) || ((this.ChromaKeyColor.g != this.cam.backgroundColor.g) || (this.ChromaKeyColor.b != this.cam.backgroundColor.b)))
                {
                    this.ChromaKeyColor = new UnityEngine.Color(this.ChromaKeyColor.r, this.ChromaKeyColor.g, this.ChromaKeyColor.b, 0f);
                    this.cam.backgroundColor = new UnityEngine.Color(this.ChromaKeyColor.r, this.ChromaKeyColor.g, this.ChromaKeyColor.b, 0f);
                    this.mascotMakerMaterialChromakey.color = new UnityEngine.Color(this.ChromaKeyColor.r, this.ChromaKeyColor.g, this.ChromaKeyColor.b, 0f);
                }
                if (this.ChromaKeyRange != this.chromaKeyRangePre)
                {
                    this.ChromaKeyRange = Mathf.Clamp(this.ChromaKeyRange, 0.002f, 1f);
                    this.chromaKeyRangePre = this.ChromaKeyRange;
                    if (this.mascotMakerMaterialChromakey)
                    {
                        this.mascotMakerMaterialChromakey.SetFloat("_Amount", this.chromaKeyRangePre);
                    }
                }
                if (this.mascotMakerTexture != null)
                {
                    RenderTexture active = RenderTexture.active;
                    RenderTexture.active = this.mascotMakerTexture;
                    this.cam.Render();
                    this.cameraTexture.ReadPixels(new UnityEngine.Rect(0f, 0f, (float) this.mascotMakerTexture.width, (float) this.mascotMakerTexture.height), 0, 0);
                    GL.Clear(true, true, this.mascotMakerMaterialChromakey.color);
                    this.cameraTexture.Apply();
                    RenderTexture.active = active;
                    this.cameraTexturePixels = this.cameraTexture.GetPixels32();
                    this.cameraTexturePixelsHandle = GCHandle.Alloc(this.cameraTexturePixels, GCHandleType.Pinned);
                    this.cameraTexturePixelsPtr = this.cameraTexturePixelsHandle.AddrOfPinnedObject();
                    this.img.ImageData = this.cameraTexturePixelsPtr;
                    if (this.ShowMascotFormOutline)
                    {
                        CvPoint[] pointArray = new CvPoint[] { new CvPoint(0, 0), new CvPoint(this.mascotMakerTexture.width - 1, 0), new CvPoint(this.mascotMakerTexture.width - 1, this.mascotMakerTexture.height - 1), new CvPoint(0, this.mascotMakerTexture.height - 1) };
                        CvPoint[][] pts = new CvPoint[][] { pointArray };
                        this.img.PolyLine(pts, true, this.frameColor, 2);
                    }
                    this.img.CvtColor(this.img, ColorConversion.BgraToRgba);
                    this.Form.Repaint(this.img.ToBitmap(), this.opacity);
                    this.cameraTexturePixelsHandle.Free();
                }
                if (this.TopMost != this.Form.TopMost)
                {
                    this.Form.TopMost = this.TopMost;
                }
            }
            if (this.Form.HitTestCount == this.Form.HitTestCountTmp)
            {
                this.isMouseHover = false;
            }
            else
            {
                this.isMouseHover = true;
                this.Form.HitTestCountTmp = this.Form.HitTestCount;
            }
            if (this.DragMove && this.isLeftMouseDown)
            {
                this.Form.Left = System.Windows.Forms.Cursor.Position.X + this.offsetX;
                this.Form.Top = System.Windows.Forms.Cursor.Position.Y + this.offsetY;
            }
        }

        public int Opacity
        {
            get => 
                this.opacity;
            set =>
                this.opacity = (byte)Mathf.Clamp(value, 0, 0xff);
        }

        public static IntPtr MainWindowHandle =>
            mainWindowHandle;

        public bool IsMouseHover =>
            this.isMouseHover;

        public int Left
        {
            get => 
                ((this.Form == null) ? -1 : this.Form.Left);
            set
            {
                if (this.Form != null)
                {
                    this.Form.Left = value;
                }
            }
        }

        public int Top
        {
            get => 
                ((this.Form == null) ? -1 : this.Form.Top);
            set
            {
                if (this.Form != null)
                {
                    this.Form.Top = value;
                }
            }
        }

        public Point Location
        {
            get => 
                ((this.Form == null) ? new Point(0, 0) : this.Form.Location);
            set
            {
                if (this.Form != null)
                {
                    this.Form.Location = value;
                }
            }
        }

        public int Width
        {
            get => 
                ((this.Form == null) ? -1 : this.Form.Width);
            set
            {
                if (this.Form != null)
                {
                    this.MascotFormSize.x = value;
                }
            }
        }

        public int Height
        {
            get => 
                ((this.Form == null) ? -1 : this.Form.Height);
            set
            {
                if (this.Form != null)
                {
                    this.MascotFormSize.y = value;
                }
            }
        }

        public int ScreenWidth =>
            ((this.Form == null) ? -1 : System.Windows.Forms.Screen.GetBounds(this.Form).Width);

        public int ScreenHeight =>
            ((this.Form == null) ? -1 : System.Windows.Forms.Screen.GetBounds(this.Form).Height);

        public string Title
        {
            get => 
                ((this.Form == null) ? "" : this.Form.Text);
            set
            {
                if (this.Form != null)
                {
                    this.Form.Text = value;
                }
            }
        }

        public bool AllowDrop
        {
            get => 
                ((this.Form != null) && this.Form.AllowDrop);
            set
            {
                if (this.Form != null)
                {
                    this.Form.AllowDrop = value;
                }
            }
        }

        public enum AntiAliasingType
        {
            None = 1,
            TwoSamples = 2,
            FourSamples = 4,
            EightSamples = 8
        }

        private class MascotForm : Form
        {
            private const int ULW_COLORKEY = 1;
            private const int ULW_ALPHA = 2;
            private const int ULW_OPAQUE = 4;
            private const byte AC_SRC_OVER = 0;
            private const byte AC_SRC_ALPHA = 1;
            private BLENDFUNC blend;
            public int HitTestCount;
            public int HitTestCountTmp;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MouseEventHandler _LeftMouseDown;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MouseEventHandler _LeftMouseUp;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MouseEventHandler _RightMouseDown;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MouseEventHandler _RightMouseUp;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MouseEventHandler _MiddleMouseDown;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MouseEventHandler _MiddleMouseUp;

            public event MouseEventHandler LeftMouseDown
            {
                add
                {
                    MouseEventHandler comparand = this._LeftMouseDown;
                    while (true)
                    {
                        MouseEventHandler a = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._LeftMouseDown, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                        if (ReferenceEquals(comparand, a))
                        {
                            return;
                        }
                    }
                }
                remove
                {
                    MouseEventHandler comparand = this._LeftMouseDown;
                    while (true)
                    {
                        MouseEventHandler source = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._LeftMouseDown, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                        if (ReferenceEquals(comparand, source))
                        {
                            return;
                        }
                    }
                }
            }

            public event MouseEventHandler LeftMouseUp
            {
                add
                {
                    MouseEventHandler comparand = this._LeftMouseUp;
                    while (true)
                    {
                        MouseEventHandler a = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._LeftMouseUp, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                        if (ReferenceEquals(comparand, a))
                        {
                            return;
                        }
                    }
                }
                remove
                {
                    MouseEventHandler comparand = this._LeftMouseUp;
                    while (true)
                    {
                        MouseEventHandler source = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._LeftMouseUp, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                        if (ReferenceEquals(comparand, source))
                        {
                            return;
                        }
                    }
                }
            }

            public event MouseEventHandler MiddleMouseDown
            {
                add
                {
                    MouseEventHandler comparand = this._MiddleMouseDown;
                    while (true)
                    {
                        MouseEventHandler a = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._MiddleMouseDown, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                        if (ReferenceEquals(comparand, a))
                        {
                            return;
                        }
                    }
                }
                remove
                {
                    MouseEventHandler comparand = this._MiddleMouseDown;
                    while (true)
                    {
                        MouseEventHandler source = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._MiddleMouseDown, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                        if (ReferenceEquals(comparand, source))
                        {
                            return;
                        }
                    }
                }
            }

            public event MouseEventHandler MiddleMouseUp
            {
                add
                {
                    MouseEventHandler comparand = this._MiddleMouseUp;
                    while (true)
                    {
                        MouseEventHandler a = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._MiddleMouseUp, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                        if (ReferenceEquals(comparand, a))
                        {
                            return;
                        }
                    }
                }
                remove
                {
                    MouseEventHandler comparand = this._MiddleMouseUp;
                    while (true)
                    {
                        MouseEventHandler source = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._MiddleMouseUp, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                        if (ReferenceEquals(comparand, source))
                        {
                            return;
                        }
                    }
                }
            }

            public event MouseEventHandler RightMouseDown
            {
                add
                {
                    MouseEventHandler comparand = this._RightMouseDown;
                    while (true)
                    {
                        MouseEventHandler a = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._RightMouseDown, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                        if (ReferenceEquals(comparand, a))
                        {
                            return;
                        }
                    }
                }
                remove
                {
                    MouseEventHandler comparand = this._RightMouseDown;
                    while (true)
                    {
                        MouseEventHandler source = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._RightMouseDown, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                        if (ReferenceEquals(comparand, source))
                        {
                            return;
                        }
                    }
                }
            }

            public event MouseEventHandler RightMouseUp
            {
                add
                {
                    MouseEventHandler comparand = this._RightMouseUp;
                    while (true)
                    {
                        MouseEventHandler a = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._RightMouseUp, (MouseEventHandler) Delegate.Combine(a, value), comparand);
                        if (ReferenceEquals(comparand, a))
                        {
                            return;
                        }
                    }
                }
                remove
                {
                    MouseEventHandler comparand = this._RightMouseUp;
                    while (true)
                    {
                        MouseEventHandler source = comparand;
                        comparand = Interlocked.CompareExchange<MouseEventHandler>(ref this._RightMouseUp, (MouseEventHandler) Delegate.Remove(source, value), comparand);
                        if (ReferenceEquals(comparand, source))
                        {
                            return;
                        }
                    }
                }
            }

            public MascotForm()
            {
                this.Text = "";
                this.AllowDrop = false;
                base.FormBorderStyle = FormBorderStyle.None;
                base.TopMost = false;
                base.ShowInTaskbar = false;
                base.MaximizeBox = false;
                base.MinimizeBox = false;
                this.DoubleBuffered = true;
                BLENDFUNC blendfunc = new BLENDFUNC();
                this.blend = blendfunc;
                this.blend.BlendOp = 0;
                this.blend.BlendFlags = 0;
                this.blend.AlphaFormat = 1;
                base.Capture = true;
                base.FormClosed += new FormClosedEventHandler(this.OnFormClosed);
                base.MouseDown += new MouseEventHandler(this.Form_MouseDown);
                base.MouseUp += new MouseEventHandler(this.Form_MouseUp);
                this.HitTestCount = 0;
            }

            [DllImport("gdi32.dll", SetLastError=true, ExactSpelling=true)]
            private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll", SetLastError=true, ExactSpelling=true)]
            private static extern int DeleteDC(IntPtr hdc);
            [DllImport("gdi32.dll", SetLastError=true, ExactSpelling=true)]
            private static extern int DeleteObject(IntPtr hObject);
            private void Form_MouseDown(object sender, MouseEventArgs e)
            {
                MouseEventHandler handler = null;
                MouseButtons button = e.Button;
                if (button == MouseButtons.Left)
                {
                    handler = this._LeftMouseDown;
                }
                else if (button == MouseButtons.Right)
                {
                    handler = this._RightMouseDown;
                }
                else if (button == MouseButtons.Middle)
                {
                    handler = this._MiddleMouseDown;
                }
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            private void Form_MouseUp(object sender, MouseEventArgs e)
            {
                MouseEventHandler handler = null;
                MouseButtons button = e.Button;
                if (button == MouseButtons.Left)
                {
                    handler = this._LeftMouseUp;
                }
                else if (button == MouseButtons.Right)
                {
                    handler = this._RightMouseUp;
                }
                else if (button == MouseButtons.Middle)
                {
                    handler = this._MiddleMouseUp;
                }
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            [DllImport("user32.dll", SetLastError=true, ExactSpelling=true)]
            private static extern IntPtr GetDC(IntPtr hWnd);
            private void OnFormClosed(object sender, EventArgs e)
            {
                base.MouseDown -= new MouseEventHandler(this.Form_MouseDown);
                base.MouseUp -= new MouseEventHandler(this.Form_MouseUp);
            }

            [DllImport("user32.dll", ExactSpelling=true)]
            private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
            public void Repaint(Bitmap bitmap, byte opacity)
            {
                if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");
                }
                IntPtr dC = GetDC(IntPtr.Zero);
                IntPtr hDC = CreateCompatibleDC(dC);
                IntPtr zero = IntPtr.Zero;
                IntPtr hObject = IntPtr.Zero;
                zero = bitmap.GetHbitmap(System.Drawing.Color.FromArgb(0));
                hObject = SelectObject(hDC, zero);
                Vector2 psize = new Vector2(bitmap.Width, bitmap.Height);
                Vector2 pprSrc = new Vector2(0, 0);
                Vector2 pptDst = new Vector2(base.Left, base.Top);
                this.blend.SourceConstantAlpha = opacity;
                UpdateLayeredWindow(base.Handle, dC, ref pptDst, ref psize, hDC, ref pprSrc, 0, ref this.blend, 2);
                if (zero != IntPtr.Zero)
                {
                    SelectObject(hDC, hObject);
                    DeleteObject(zero);
                }
                DeleteDC(hDC);
                ReleaseDC(IntPtr.Zero, dC);
            }

            [DllImport("gdi32.dll", ExactSpelling=true)]
            private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
            [DllImport("user32.dll", SetLastError=true, ExactSpelling=true)]
            private static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Vector2 pptDst, ref Vector2 psize, IntPtr hdcSrc, ref Vector2 pprSrc, int crKey, ref BLENDFUNC pblend, int dwFlags);
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x84)
                {
                    this.HitTestCount++;
                    if (this.HitTestCount >= 0x186a0)
                    {
                        this.HitTestCount = -1;
                    }
                }
                m.Result = (IntPtr) 1;
                base.WndProc(ref m);
            }

            protected override System.Windows.Forms.CreateParams CreateParams
            {
                get
                {
                    System.Windows.Forms.CreateParams createParams = base.CreateParams;
                    createParams.ExStyle |= 0x80000;
                    return createParams;
                }
            }

            [StructLayout(LayoutKind.Sequential, Pack=1)]
            private struct BGRA
            {
                public byte B;
                public byte G;
                public byte R;
                public byte A;
            }

            [StructLayout(LayoutKind.Sequential, Pack=1)]
            private struct BLENDFUNC
            {
                public byte BlendOp;
                public byte BlendFlags;
                public byte SourceConstantAlpha;
                public byte AlphaFormat;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct Vector2
            {
                public int x;
                public int y;
                public Vector2(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }
            }
        }

        public enum UpdateFuncType
        {
            Update = 1,
            LateUpdate = 2
        }
    }
}

