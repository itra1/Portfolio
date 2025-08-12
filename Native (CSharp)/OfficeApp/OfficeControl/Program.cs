//#define DEVELOP

using OfficeControl.Controllers;

Apps apps = new();

//Application.ApplicationExit += (p1,p2) =>
//{
//	apps.CloseAll();
//};

//Excel doc = (Excel)apps.OpenApp(OfficeControl.Common.OfficeAppType.Excel, @"C:/Users/it_ra/AppData/LocalLow/CNP/VideoWall/office/c6c70b11bc4f5733cb80e9687aefaf093a455e84");
//Word doc = (Word)apps.OpenApp(OfficeControl.Common.OfficeAppType.Word, @"C:/Users/it_ra/AppData/LocalLow/CNP/VideoWall/office/2a324f561ade696c576d1b857696c6322a55d061");
//var doc = (PowerPoint)apps.OpenApp(OfficeControl.Common.OfficeAppType.PowerPoint, @"C:\Users\it_ra\AppData\LocalLow\CNP\VideoWall\office\5b14ed6f43af046b0ce7c2dde620d19b41053467");

#if !DEVELOP

PackageManager packageManager = new();
PipeClient pipeClient = new(args[0]);

#endif

while (true)
{
	Console.ReadLine();
}