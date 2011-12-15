This project is a clone of this : http://adfwia.codeplex.com/

After scouring the web I managed to write a small class library that allows you to use Windows® Image Acquisition 2.0 lib to scan multiple images from scanners with an auto document feeder.

So its plain and simple to use and also contains a Win forms test app.

	private void button2_Click(object sender, EventArgs e)
	{
	        ADFScan  scanner = new ADFScan();
	        scanner.Scanning += new EventHandler<WiaImageEventArgs>(_scanner_Scanning);
	        scanner.ScanComplete += new EventHandler(_scanner_ScanComplete);
	        ScanColor selectedColor = ScanColor.BlackWhite;
	        int dpi = 300;//some scanners have a problem if you set a lower DPI
	        scanner.BeginScan(selectedColor,dpi );
	       //ADFScan will now raise a Scanning event for EACH document scanned.
	       //then scan complete once there are no more documents to scan.
	}
	void _scanner_ScanComplete(object sender, EventArgs e)
	{
	        MessageBox.Show("Scan Complete");
	}
	void _scanner_Scanning(object sender, WiaImageEventArgs e)
	{//e.ScannedImage is a System.Drawing.Image
	           e.ScannedImage.Save(filename, ImageFormat.Jpeg);//FILES ARE RETURNED AS BITMAPS
	}

###Notes

This lib references WIA 2.0, which is available by default on Vista and higher. 

This works for Windows XP SP1 onward but you need to install Windows® Image Acquisition Automation Library v2.0 You may also need to run :
>regsvr32.exe wiaaut.dll 

Tell me if you are using this successfully or anything else you like : 
[http://gideondsouza.com/blog/scanning-multiple-documents-with-wia-2.0-adf-scanner ](http://gideondsouza.com/blog/scanning-multiple-documents-with-wia-2.0-adf-scanner )OR [http://gideondsouza.com/contact](http://gideondsouza.com/contact)
