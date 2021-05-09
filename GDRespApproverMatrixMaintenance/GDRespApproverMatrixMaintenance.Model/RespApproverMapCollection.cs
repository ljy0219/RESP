using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Model
{
    public class DataCollection<T>
    {
       public IEnumerable<T> Collection { get; set; }
       public Int32 TotalCount { get; set; }
    }
}
