using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Connector
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConnectorBusiness.SyncResponsibilities();
            RespConnector.SyncResponsibilities();
        }
    }
}
