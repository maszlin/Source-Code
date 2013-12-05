using System;
using System.Collections.Generic;
using System.Text;

namespace NEPS.GTechnology.CopyStreetAddress
{
    class AddressDef
    {       
        private string nSTREET_NAME;
        public string STREET_NAME
        {
            set
            { nSTREET_NAME = value; }

            get
            { return nSTREET_NAME; }
        }

        private string nSTREET_TYPE;
        public string STREET_TYPE
        {
            set
            { nSTREET_TYPE = value; }

            get
            { return nSTREET_TYPE; }
        }

        private string nPOSTAL_NUM;
        public string POSTAL_NUM
        {
            set
            { nPOSTAL_NUM = value; }

            get
            { return nPOSTAL_NUM; }
        }

        private string nSECTION_NAME;
        public string SECTION_NAME
        {
            set
            { nSECTION_NAME = value; }

            get
            { return nSECTION_NAME; }
        }

        private string nCITY_NAME;
        public string CITY_NAME
        {
            set
            { nCITY_NAME = value; }

            get
            { return nCITY_NAME; }
        }

        private string nSTATE_CODE;
        public string STATE_CODE
        {
            set
            { nSTATE_CODE = value; }

            get
            { return nSTATE_CODE; }
        }
    }
}
