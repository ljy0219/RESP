using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Business
{
    public class HttpResponseObject<T>
    {
        public bool IsSuccess { get; set; }
        public T Content { get; set; }
        public string ErrorMsg { get; set; }
    }
}
