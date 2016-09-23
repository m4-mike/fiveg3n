using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using SharpDX.D3DCompiler;
using SharpDX.Mathematics;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace fivegen
{
    // EXAMPLE CODE TO SEE IF IT WORKS
    /* 
        Code snippets taken and modified from https://github.com/sharpdx/SharpDX-Samples/tree/master/Desktop/Direct3D11
     */
    public partial class RenderPanel : Panel
    {
        private DeviceContext context;

        private SwapChainDescription desc;
        private SwapChain swapChain;

        private RenderTargetView renderView;
        private DepthStencilView depthView;

        private VertexShader vertexShader;
        private PixelShader pixelShader;

        private Buffer vertices, constantBuffer;

        private InputLayout layout;

        private Device device;

        private Texture2D backBuffer;
        private Texture2D depthBuffer;

        private Matrix view, proj;

        private Stopwatch timer;

        private bool resized;

        private float  mouseX, mouseY;
        private float mouseDX, mouseDY;


        public RenderPanel()
        {
            InitializeComponent();
            // Stops Design view crashing trying to render DirectX stuff
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv") return;
            desc = new SwapChainDescription()
            {
                BufferCount = 2,
                ModeDescription = new ModeDescription(this.ClientSize.Width, this.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = this.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
            context = device.ImmediateContext;

            initShaders();
            initVertices();

            constantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            proj = Matrix.Identity;

            this.Resize += (sender, args) => resized = true;

            timer = new Stopwatch();
            timer.Start();

            resized = true;
            backBuffer = null;
            renderView = null;
            depthBuffer = null;
            depthView = null;
        }

        ~RenderPanel()
        {
            // Release all resources
            vertexShader.Dispose();
            pixelShader.Dispose();
            vertices.Dispose();
            depthBuffer.Dispose();
            constantBuffer.Dispose();
            depthView.Dispose();
            layout.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            context.ClearState();
            context.Flush();
            device.Dispose();
            context.Dispose();
            swapChain.Dispose();
        }

        void initShaders()
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("../../shaders/vertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug);
            vertexShader = new VertexShader(device, vertexShaderByteCode);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("../../shaders/pixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug);
            pixelShader = new PixelShader(device, pixelShaderByteCode);

            layout = new InputLayout(
               device,
               ShaderSignature.GetInputSignature(vertexShaderByteCode),
               new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            context.PixelShader.Set(pixelShader);
            context.VertexShader.Set(vertexShader);
        }

        void initVertices()
        {
            // Instantiate Vertex bufffer from vertex data
            vertices = Buffer.Create(device, BindFlags.VertexBuffer, new[]
                 {
                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                    new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),

                    new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),

                    new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                    new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

                    new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                    new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),

                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                    new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),

                    new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)
                 });
        }

        internal void mouseDrag(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                mouseDX += (mouseX - (float)e.X);
                mouseDY -= (mouseY - (float)e.Y);
                mouseX = e.X;
                mouseY = e.Y;
            }
        }
        
        internal void clearRotations()
        {
            mouseDX = 0;
            mouseDY = 0;
        }

        public void Draw()
        {
            if (resized)
            {
                Utilities.Dispose(ref backBuffer);
                Utilities.Dispose(ref renderView);
                Utilities.Dispose(ref depthBuffer);
                Utilities.Dispose(ref depthView);

                swapChain.ResizeBuffers(desc.BufferCount, this.ClientSize.Width, this.ClientSize.Height, Format.Unknown, SwapChainFlags.None);

                backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
                renderView = new RenderTargetView(device, backBuffer);

                depthBuffer = new Texture2D(device, new Texture2DDescription()
                {
                    Format = Format.D32_Float_S8X24_UInt,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = this.ClientSize.Width,
                    Height = this.ClientSize.Height,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                });

                depthView = new DepthStencilView(device, depthBuffer);
                context.Rasterizer.SetViewport(new Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height, 0.0f, 1.0f));
                context.OutputMerger.SetTargets(depthView, renderView);

                proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, this.ClientSize.Width / (float)this.ClientSize.Height, 0.1f, 100.0f);

                resized = false;
            }

            var viewProj = Matrix.Multiply(view, proj);

            context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            context.ClearRenderTargetView(renderView, SharpDX.Color.Black);

            var worldViewProj = Matrix.RotationX(-mouseDY / 200f) * Matrix.RotationY(mouseDX / 200f) * viewProj;
            worldViewProj.Transpose();
            context.UpdateSubresource(ref worldViewProj, constantBuffer);

            context.Draw(36, 0);
            swapChain.Present(0, PresentFlags.None);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            // Stops Design view crashing trying to render DirectX stuff
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv")
            {
                pe.Graphics.Clear(System.Drawing.Color.Chartreuse);
                pe.Graphics.DrawString("DIRECT X RENDERING PANEL", new Font("Arial", 16), new SolidBrush(System.Drawing.Color.Black), new PointF(50.0f, 150.0f));
            }
        }

        
    }
}
