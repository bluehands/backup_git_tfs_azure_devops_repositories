using Newtonsoft.Json;

//https://www.paessler.com/manuals/prtg/custom_sensors

namespace BackupRepositories.Services.Prtg
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PrtgResult
    {
        public PrtgResult(string channelName, string channelValue)
        {
            Channel = channelName;
            Value = channelValue;
        }

        public PrtgResult()
        {
        }

        public string Channel { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; } = null;
        public string CustomUnit { get; set; } = null;
        public string SpeedSize { get; set; } = null;
        public string SpeedTime { get; set; } = null;
        public string Mode { get; set; } = null;
        public string Float { get; set; } = null;
        public string Warning { get; set; } = null;
        public string ShowChart { get; set; } = null;
        public string ShowTable { get; set; } = null;
        public string LimitMaxError { get; set; } = null;
        public string LimitMaxWarning { get; set; } = null;
        public string LimitMinWarning { get; set; } = null;
        public string LimitMinError { get; set; } = null;
        public string LimitErrorMsg { get; set; } = null;
        public string LimitWarningMsg { get; set; } = null;
        public string LimitMode { get; set; } = null;
        public string NotifyChanged { get; set; } = null;
    }

    public enum PrtgResultUnit
    {
        BytesBandwidth,
        BytesDisk,
        Temperature,
        Percent,
        TimeResponse,
        TimeSeconds,
        Custom,
        Count,
        CPU,
        BytesFile,
        SpeedDisk,
        SpeedNet,
        TimeHours
    }

    public enum PrtgResultSpeedSize
    {
        One,
        Kilo,
        Mega,
        Giga,
        Tera,
        Byte,
        KiloByte,
        MegaByte,
        GigaByte,
        TeraByte,
        Bit,
        KiloBit,
        MegaBit,
        GigaBit,
        TeraBit
    }

    public enum PrtgResultMode
    {
        Absolute,
        Difference
    }

    public enum PrtgSpeedTime
    {
        Second,
        Minute,
        Hour,
        Day
    }

    public static class PrtgResultExtensions
    {
        public static PrtgResult SetCustomUnit(this PrtgResult prtgResult, string unit)
        {
            prtgResult.CustomUnit = unit;
            return prtgResult;
        }

        public static PrtgResult SetUnit(this PrtgResult prtgResult, PrtgResultUnit unit)
        {
            prtgResult.Unit = unit.ToString();
            return prtgResult;
        }

        public static PrtgResult SetSpeedSize(this PrtgResult prtgResult, PrtgResultSpeedSize unit)
        {
            prtgResult.SpeedSize = unit.ToString();
            return prtgResult;
        }

        public static PrtgResult SetSpeedTime(this PrtgResult prtgResult, PrtgSpeedTime unit)
        {
            prtgResult.SpeedTime = unit.ToString();
            return prtgResult;
        }

        public static PrtgResult SetMode(this PrtgResult prtgResult, PrtgResultMode resultMode)
        {
            prtgResult.Mode = resultMode.ToString();
            return prtgResult;
        }

        public static PrtgResult SetFloat(this PrtgResult prtgResult)
        {
            prtgResult.Float = "1";
            return prtgResult;
        }

        public static PrtgResult SetWarning(this PrtgResult prtgResult)
        {
            prtgResult.Warning = "1";
            return prtgResult;
        }

        public static PrtgResult SetShowChart(this PrtgResult prtgResult, bool show = true)
        {
            prtgResult.ShowChart = show 
                ? "1" 
                : "0";

            return prtgResult;
        }

        public static PrtgResult SetShowTable(this PrtgResult prtgResult, bool show = true)
        {
            prtgResult.ShowTable = show 
                ? "1" 
                : "0";
            return prtgResult;
        }

        public static PrtgResult SetLimitMaxError(this PrtgResult prtgResult, int value)
        {
            prtgResult.LimitMaxError = "\""+ value +"\"";
            return prtgResult;
        }
        public static PrtgResult SetLimitMaxWarning(this PrtgResult prtgResult, int value)
        {
            prtgResult.LimitMaxWarning = "\"" + value + "\"";
            return prtgResult;
        }
        public static PrtgResult SetLimitMinWarning(this PrtgResult prtgResult, int value)
        {
            prtgResult.LimitMinWarning = "\"" + value + "\"";
            return prtgResult;
        }
        public static PrtgResult SetLimitMinError(this PrtgResult prtgResult, int value)
        {
            prtgResult.LimitMinError = "\"" + value + "\"";
            return prtgResult;
        }
        public static PrtgResult SetLimitErrorMsg(this PrtgResult prtgResult, string message)
        {
            prtgResult.LimitErrorMsg = message;
            return prtgResult;
        }
        public static PrtgResult SetLimitWarningMsg(this PrtgResult prtgResult, string message)
        {
            prtgResult.LimitWarningMsg = message;
            return prtgResult;
        }
        public static PrtgResult SetLimitMode(this PrtgResult prtgResult, bool isActive)
        {
            prtgResult.LimitMode = isActive 
                ? "1" 
                : "0";
            return prtgResult;
        }

        public static PrtgResult SetNotifyChanged(this PrtgResult prtgResult)
        {
            prtgResult.NotifyChanged = "";
            return prtgResult;
        }
    }

}