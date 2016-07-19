using log4net;
using log4net.Config;

namespace DynamicDevices.DiskWriter
{
    public class Logger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            XmlConfigurator.Configure();
        }

        public void Error(string format, params object[] args)
        {
            Log.Error(string.Format(format, args));
        }
    }
}
