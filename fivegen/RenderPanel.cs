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


namespace fivegen
{
    // EXAMPLE CODE TO SEE IF IT WORKS
    /* 
        Code snippets taken and modified from https://github.com/sharpdx/SharpDX-Samples/tree/master/Desktop/Direct3D11/MiniTri
     */
    public partial class RenderPanel : Panel
    {
        private DeviceContext context;
        private SwapChain swapChain;
        private RenderTargetView renderView;

        private CompilationResult vertexShaderByteCode;
        private VertexShader vertexShader;
        private CompilationResult pixelShaderByteCode;
        private PixelShader pixelShader;
        private Buffer vertices;
        private InputLayout layout;
        private Device device;
        private Texture2D backBuffer;
        private Factory factory;

        private const int SCALING_FACTOR = 5;

        public RenderPanel()
        {
            InitializeComponent();
            // Stops Design view crashing trying to render DirectX stuff
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv") return;
            var desc = new SwapChainDescription()
            {
                BufferCount = 3,
                ModeDescription = new ModeDescription(this.ClientSize.Width * SCALING_FACTOR, this.ClientSize.Height * SCALING_FACTOR, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = this.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
            context = device.ImmediateContext;

            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);

            vertexShaderByteCode = ShaderBytecode.CompileFromFile("../../MiniTri.fx", "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            vertexShader = new VertexShader(device, vertexShaderByteCode);

            pixelShaderByteCode = ShaderBytecode.CompileFromFile("../../MiniTri.fx", "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            pixelShader = new PixelShader(device, pixelShaderByteCode);

            layout = new InputLayout(
                device,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            // Instantiate Vertex bufffer from vertex data
            vertices = Buffer.Create(device, BindFlags.VertexBuffer, new[]
                 {
                    new Vector4(-0.5f, 0.5f, 1f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(0.5f, 0.5f, 1f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-0.5f, -0.5f, 1f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

                    new Vector4(0.5f, 0.5f, 1f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(0.5f, -0.5f, 1f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-0.5f, -0.5f, 1f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                 });

            // Prepare All the stages
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 32, 0));
            context.VertexShader.Set(vertexShader);
            context.Rasterizer.SetViewport(new Viewport(0, 0, this.ClientSize.Width * SCALING_FACTOR, this.ClientSize.Height * SCALING_FACTOR, 0.0f, 1.0f));
            context.PixelShader.Set(pixelShader);
            context.OutputMerger.SetTargets(renderView);
        }

        ~RenderPanel()
        {
            // Release all resources
            vertexShaderByteCode.Dispose();
            vertexShader.Dispose();
            pixelShaderByteCode.Dispose();
            pixelShader.Dispose();
            vertices.Dispose();
            layout.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            context.ClearState();
            context.Flush();
            device.Dispose();
            context.Dispose();
            swapChain.Dispose();
            factory.Dispose();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            // Stops Design view crashing trying to render DirectX stuff
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv")
            {
                pe.Graphics.Clear(System.Drawing.Color.Chartreuse);
                pe.Graphics.DrawString("DIRECT X RENDERING PANEL", new Font("Arial", 16), new SolidBrush(System.Drawing.Color.Black), new PointF(50.0f, 150.0f));
            } else
            {
                context.ClearRenderTargetView(renderView, SharpDX.Color.Black);
                context.Draw(6, 0);
                swapChain.Present(0, PresentFlags.None);
            }
        }
    }
}
