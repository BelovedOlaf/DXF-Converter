using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using netDxf;
using netDxf.Entities;
using netDxf.Header;
using DxfVersion = netDxf.Header.DxfVersion;
using Line = netDxf.Entities.Line;
using Microsoft.Win32;


namespace DXF_Converter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			/*
			// your DXF file name
			string saveFile = "sample.dxf";

			// create a new document, by default it will create an AutoCad2000 DXF version
			DxfDocument doc = new DxfDocument(DxfVersion.AutoCad2000);
			
			// an entity
			Line entity = new Line(new Vector2(5, 5), new Vector2(10, 5));
			
			// add your entities here
			doc.Comments.Clear();
			doc.DrawingVariables.ClearCustomVariables();
			doc.Entities.Add(entity);
			
			// save to file
			doc.Save(saveFile);
			*/
			/*
			// this check is optional but recommended before loading a DXF file
			DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file);
			// netDxf is only compatible with AutoCad2000 and higher DXF versions
			if (dxfVersion < DxfVersion.AutoCad2000) return;
			// load file
			DxfDocument loaded = DxfDocument.Load(file);
			*/
		}
		private String GroupCodeAndValuetoString(int GroupCode, String value)
		{
			String text = ("   " + GroupCode.ToString()).Remove(0, GroupCode.ToString().Length);
			text += "\n";
			text += value; text += "\n";
			return text;
		}
		private String AddLineInfo(String LayerName, int ColorIndex, double x, double y, double x1, double y1)
		{
			String text = GroupCodeAndValuetoString(0, "LINE");
			text += GroupCodeAndValuetoString(8, LayerName);
			text += GroupCodeAndValuetoString(62, ColorIndex.ToString());
			text += GroupCodeAndValuetoString(10, x.ToString("F8"));
			text += GroupCodeAndValuetoString(20, y.ToString("F8"));
			text += GroupCodeAndValuetoString(11, x1.ToString("F8"));
			text += GroupCodeAndValuetoString(21, y1.ToString("F8"));
			return text;
		}
		private void OpenFileButton_Click(object sender, RoutedEventArgs e)
		{
			int[] GroupCode = new int[100];
			String[] Values = new string[100];

			var dialog = new OpenFileDialog();
			dialog.FileName = "Document"; // Default file name
			dialog.DefaultExt = ".gbin"; // Default file extension
			dialog.Filter = "Text documents (.gbin)|*.gbin"; // Filter files by extension

			//	Process open file dialog box results
			if (dialog.ShowDialog() == true)
			{
				byte[] byteArray = File.ReadAllBytes(dialog.FileName);
				
				string saveFileName = dialog.FileName.Remove(dialog.FileName.Count()-4, 4);
				saveFileName += "dxf_";
				MessageBox.Show(saveFileName);

				String text = "";

				// Add Section Start Info
				{
					text += GroupCodeAndValuetoString(0, "SECTION");
					text += GroupCodeAndValuetoString(2, "ENTITIES");
				}

				// Add Line Info
				{
					text += AddLineInfo("0", 7, 0, 0, 12192, 0);
					text += AddLineInfo("0", 7, 12192, 0, 12192, 2438);
					text += AddLineInfo("0", 7, 12192, 2438, 0, 2438);
					text += AddLineInfo("0", 7, 0, 2438, 0, 0);
				}

				// Add Section End Info 
				{
					text += GroupCodeAndValuetoString(0, "ENDSEC");
					text += GroupCodeAndValuetoString(0, "EOF");
				}
				
				File.WriteAllText(saveFileName, text);
			}
		}
	}
}
