using System;
using System.Collections.Generic;
using System.Text;
using WIA;
using System.IO;
using System.Drawing;

namespace ADFScanner
{
    public class ADFScan
    {
        public void BeginScan(ScanColor color, int dotsperinch)
        {
            Scan(color, dotsperinch);
        }
        public event EventHandler<WiaImageEventArgs> Scanning;
        public event EventHandler ScanComplete;
       
        void Scan(ScanColor clr, int dpi)
        {
            string deviceid;
            //Choose Scanner
            CommonDialogClass class1 = new CommonDialogClass();
            Device d = class1.ShowSelectDevice(WiaDeviceType.UnspecifiedDeviceType, true, false);
            if (d != null)
            {
                deviceid = d.DeviceID;
            }
            else
            {
                //no scanner chosen
                return;
            }
            WIA.CommonDialog WiaCommonDialog = new CommonDialogClass();
            bool hasMorePages = true;
            int x = 0;
            int numPages = 0;
            while (hasMorePages)
            {
                //Create DeviceManager
                DeviceManager manager = new DeviceManagerClass();
                Device WiaDev = null;
                foreach (DeviceInfo info in manager.DeviceInfos)
                {
                    if (info.DeviceID == deviceid)
                    {
                        WIA.Properties infoprop = null;
                        infoprop = info.Properties;
                        //connect to scanner
                        WiaDev = info.Connect();
                        break;
                    }
                }
                //Start Scan
                WIA.ImageFile img = null;
                WIA.Item Item = WiaDev.Items[1] as WIA.Item;
                //set properties //BIG SNAG!! if you call WiaDev.Items[1] apprently it erases the item from memory so you cant call it again
                Item.Properties["6146"].set_Value((int)clr);//Item MUST be stored in a variable THEN the properties must be set.
                Item.Properties["6147"].set_Value(dpi);
                Item.Properties["6148"].set_Value(dpi);
                try
                {//WATCH OUT THE FORMAT HERE DOES NOT MAKE A DIFFERENCE... .net will load it as a BITMAP!
                    img = (ImageFile)WiaCommonDialog.ShowTransfer(Item, WIA.FormatID.wiaFormatJPEG, false);
                    //process image:
                    //Save to file and open as .net IMAGE
                    string varImageFileName = Path.GetTempFileName();
                    if (File.Exists(varImageFileName))
                    {
                        //file exists, delete it
                        File.Delete(varImageFileName);
                    }
                    img.SaveFile(varImageFileName);
                    Image ret = Image.FromFile(varImageFileName);
                    EventHandler<WiaImageEventArgs> temp = Scanning;
                    if (temp != null)
                    {
                        temp(this, new WiaImageEventArgs(ret));
                    }
                    numPages++;
                    img = null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Item = null;
                    //determine if there are any more pages waiting
                    Property documentHandlingSelect = null;
                    Property documentHandlingStatus = null;

                    foreach (Property prop in WiaDev.Properties)
                    {
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_SELECT)
                            documentHandlingSelect = prop;
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_STATUS)
                            documentHandlingStatus = prop;
                    }
                    hasMorePages = false; //assume there are no more pages
                    if (documentHandlingSelect != null)
                    //may not exist on flatbed scanner but required for feeder
                    {
                        //check for document feeder
                        if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_SELECT.FEEDER) != 0)
                        {
                            hasMorePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_STATUS.FEED_READY) != 0);
                        }
                    }
                    x++;
                }
            }
            EventHandler tempCom = ScanComplete;
            if (tempCom != null)
            {
                tempCom(this, EventArgs.Empty);
            }
        }
        //internal classes
        #region InternalClasses
        class WIA_DPS_DOCUMENT_HANDLING_SELECT
        {
            public const uint FEEDER = 0x00000001;
            public const uint FLATBED = 0x00000002;
        }
        class WIA_DPS_DOCUMENT_HANDLING_STATUS
        {
            public const uint FEED_READY = 0x00000001;
        }
        class WIA_PROPERTIES
        {
            public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
            public const uint WIA_DIP_FIRST = 2;
            public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            //
            // Scanner only device properties (DPS)
            //
            public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
            public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
        }
        class WIA_ERRORS
        {
            public const uint BASE_VAL_WIA_ERROR = 0x80210000;
            public const uint WIA_ERROR_PAPER_EMPTY = BASE_VAL_WIA_ERROR + 3;
        }
        #endregion
    }
}

