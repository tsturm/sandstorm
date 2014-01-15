using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandstormConfiguration
{
    class SandstormConfigHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public SandstormConfiguration LoadConfiguration()
        {


            //return default config
            return new SandstormConfiguration();
        }
        

        public bool StoreConfiguration(SandstormConfiguration config) 
        {



            //return if export has been ok
            return true;
        }

    }
}
