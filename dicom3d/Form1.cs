using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kitware.VTK;

namespace dicom3d
{
    public partial class Form1 : Form
    {
        public RenderWindowControl renderWindowControl = new RenderWindowControl();

        vtkRenderWindow renderWindow3D;
        vtkRenderer renderer3D;
        vtkDICOMImageReader reader3D;


        public Form1()
        {
            InitializeComponent();

            //renderWindowControl = new RenderWindowControl()
            //{
            //    Parent = panel1,
            //    AddTestActors = true
            //};

            renderWindowControl.Parent = panel1;
            renderWindowControl.AddTestActors = true;
        }

        public void Render3D(string folder)
        {
            reader3D = vtkDICOMImageReader.New();
            reader3D.SetDirectoryName(folder);
            reader3D.Update();

            renderer3D = vtkRenderer.New();
            renderWindow3D = vtkRenderWindow.New();

            renderWindowControl.Dock = DockStyle.Fill;
            panel1.Controls.Add(renderWindowControl);

            renderWindow3D = renderWindowControl.RenderWindow;
            renderWindow3D.AddRenderer(renderer3D);


            Coloring();
        }

        vtkVolume vol;

        public void Coloring(int shft = 0)
        {
            //vtkFixedPointVolumeRayCastMapper texMapper = vtkFixedPointVolumeRayCastMapper.New();
            vtkSmartVolumeMapper texMapper = vtkSmartVolumeMapper.New();
            vol = vtkVolume.New();
            vtkColorTransferFunction ctf = vtkColorTransferFunction.New();
            vtkPiecewiseFunction spwf = vtkPiecewiseFunction.New();
            vtkPiecewiseFunction gpwf = vtkPiecewiseFunction.New();

            texMapper.SetInputConnection(reader3D.GetOutputPort());

            //Set the color curve for the volume
            //ctf.AddHSVPoint(0, .67, .07, 1);
            //ctf.AddHSVPoint(94, .67, .07, 1);
            //ctf.AddHSVPoint(139, 0, 0, 0);
            //ctf.AddHSVPoint(160, .28, .047, 1);
            //ctf.AddHSVPoint(254, .38, .013, 1);

            ctf.AddRGBPoint(0.0, 0.0, 0.0, 0.0);
            ctf.AddRGBPoint(64.0, 1.0, 0.0, 0.0);
            ctf.AddRGBPoint(128.0, 0.0, 0.0, 1.0);
            ctf.AddRGBPoint(192.0, 0.0, 1.0, 0.0);
            ctf.AddRGBPoint(255.0, 0.0, 0.2, 0.0);

            //Set the opacity curve for the volume
            spwf.AddPoint(584+shft, 0);
            spwf.AddPoint(651+shft, .1);
            //spwf.AddPoint(255, 1);


            //spwf.AddPoint(4, 0);
            //spwf.AddPoint(51, .7);
            //spwf.AddPoint(155, 0.5);
            //spwf.AddPoint(255, 0.2);
            //spwf.AddPoint(1055, 0);


            //Set the gradient curve for the volume
            //gpwf.AddPoint(0, .2);
            gpwf.AddPoint(10, 1);
            gpwf.AddPoint(225, 0.5);
            gpwf.AddPoint(1235, 0.2);
            gpwf.AddPoint(3235, 0);

            vol.GetProperty().SetColor(ctf);
            vol.GetProperty().SetScalarOpacity(spwf);
            //vol.GetProperty().SetGradientOpacity(gpwf);

            vol.GetProperty().ShadeOn();
            vol.GetProperty().SetInterpolationTypeToLinear();

            vol.SetMapper(texMapper);
            
            //green background
            renderer3D.SetBackground(0.3, 0.6, 0.3);
            //Go through the Graphics Pipeline
            renderer3D.AddVolume(vol);

            renderWindow3D.Render();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //ReadDICOMSeries();
                Render3D(@"E:\T-Soles\Datasets\Caroline_GymZonder");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
            }
        }

        private void panel1_mouseWheel(object sender, MouseEventArgs e)
        {
            Coloring(e.Delta);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            vol.UpdateScalarOpacityforSampleSize(renderer3D, 100);

            renderer3D.Render();
            Coloring(100);
        }

    }
}
