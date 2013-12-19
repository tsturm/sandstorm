using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandstormConfiguration
{
    /// <summary>
    /// Storage Class for all config parameters
    /// 
    /// do for every param a own field + property
    /// </summary>
    public class SandstormConfiguration
    {

        #region FIELDS

        private int m_NumberOfParticles;
        private Object m_SharedObject;

        #endregion

        #region PROPERTIES

        public int NumberOfParticles
        {
            get { return m_NumberOfParticles; }
            set { m_NumberOfParticles = value; }
        }

        public Object SharedObjects
        {
            get { return m_SharedObject ?? (m_SharedObject = new Object()); }
            set { m_SharedObject = value;  }
        }

        #endregion

        #region CONSTRUCTOR

        public SandstormConfiguration()
        {
            //constructor to set default values
            NumberOfParticles = 100;

        }

        #endregion
    }
}
