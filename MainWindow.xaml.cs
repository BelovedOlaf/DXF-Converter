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
					for (int i = 296; i < byteArray.Length; i += 88)
						text += AddLineInfo("0", 7,
						BitConverter.ToDouble(byteArray.Skip(i).Take(8).ToArray(), 0),
						BitConverter.ToDouble(byteArray.Skip(i+8).Take(8).ToArray(), 0),
						BitConverter.ToDouble(byteArray.Skip(i+16).Take(8).ToArray(), 0),
						BitConverter.ToDouble(byteArray.Skip(i+24).Take(8).ToArray(), 0));

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
